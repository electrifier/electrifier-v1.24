using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Vanara.Windows.Shell;
using static Vanara.PInvoke.Shell32;

namespace electrifier.Controls.Vanara;

// https://github.com/dahall/Vanara/blob/master/Windows.Forms/Controls/ExplorerBrowser.cs
// TODO: See also https://github.com/dahall/Vanara/blob/ac0a1ac301dd4fdea9706688dedf96d596a4908a/Windows.Shell.Common/StockIcon.cs

public sealed partial class ExplorerBrowser : INotifyPropertyChanged
{
    // TODO: Use ShellItemArray for ShellItem Collections
    public List<ExplorerBrowserItem> CurrentFolderItems
    {
        get;
        private set;
    }

    public ShellItem CurrentFolder;
    private ExplorerBrowserItem ebCurrentFolderItem;

    private readonly ImageCache _imageCache;

    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;
        CurrentFolder = ShellFolder.Desktop;
        CurrentFolderItems = [];
        _imageCache = new ImageCache();

        ebCurrentFolderItem = new ExplorerBrowserItem(this, CurrentFolder);
        ShellTreeView.InitializeRoot(ebCurrentFolderItem);
        //ShellTreeView.NativeTreeView.SelectedItem = ebCurrentFolderItem;
        ShellTreeView.NativeTreeView.SelectionChanged += ShellTreeView_SelectionChanged;

        TryNavigate(CurrentFolder);
    }

    public void TryNavigate(ShellItem shItem)
    {
        CurrentFolderItems.Clear();

        if (!shItem.IsFileSystem)
        {
            Debug.Fail($"TryNavigate: IsFileSystem of item {shItem} is false.");
            return;
        }
        if (!shItem.IsFolder)
        {
            Debug.Write($"TryNavigate: IsFolder of item {shItem} is false.");
            return;
        }

        using var shFolder = new ShellFolder(shItem);
        try
        {
            var rootItem = new ExplorerBrowserItem(this, shItem); //shFolder

            var list = rootItem.GetChildItems(shItem);
            for (var index = 0; index < list.Count; index++)
            {
                var item = list[index];
                rootItem.Children.Add(item);
            }

            CurrentFolderItems = rootItem.Children;
            ShellTreeView.SetItemsSource(rootItem, CurrentFolderItems);
            ShellGridView.SetItemsSource(CurrentFolderItems); // TODO: binding
        }
        finally
        {
            SetField(ref CurrentFolder, shFolder);
            // TODO: Raise navigated event
            Debug.Write($"TryNavigate: Done {shItem}.");
        }
    }

    private void ShellTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        var selectedNode = ShellTreeView.NativeTreeView.SelectedNode;
        var selectedItem = ShellTreeView.NativeTreeView.SelectedItem;
        var addedItems = args.AddedItems;
        var removedItems = args.RemovedItems;

        Debug.Print($"ShellTreeView_SelectionChanged SelectedItem: {selectedItem} ");

        if (selectedNode != null)
        {
            var nodeContent = selectedNode.Content;

            if (nodeContent is ExplorerBrowserItem ebItem)
            {
                Debug.Print($"nameof({ShellTreeView_SelectionChanged}) - {ebItem}");
                TryNavigate(ebItem.ShellItem);
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

#region The following is original copy & paste from Vanara
/// <summary>Event argument for The Navigated event</summary>
public class NavigatedEventArgs : EventArgs
{
    /// <summary>The new location of the explorer browser</summary>
    public ShellItem? NewLocation
    {
        get; set;
    }
}

/// <summary>Event argument for The Navigating event</summary>
public class NavigatingEventArgs : EventArgs
{
    /// <summary>Set to 'True' to cancel the navigation.</summary>
    public bool Cancel
    {
        get; set;
    }

    /// <summary>The location being navigated to</summary>
    public ShellItem? PendingLocation
    {
        get; set;
    }
}

/// <summary>Event argument for the NavigatinoFailed event</summary>
public class NavigationFailedEventArgs : EventArgs
{
    /// <summary>The location the browser would have navigated to.</summary>
    public ShellItem? FailedLocation
    {
        get; set;
    }
}
#endregion
