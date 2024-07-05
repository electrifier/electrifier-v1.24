using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Controls;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

// https://github.com/dahall/Vanara/blob/master/Windows.Forms/Controls/ExplorerBrowser.cs
// TODO: See also https://github.com/dahall/Vanara/blob/ac0a1ac301dd4fdea9706688dedf96d596a4908a/Windows.Shell.Common/StockIcon.cs
public sealed partial class ExplorerBrowser : INotifyPropertyChanged
{
    // TODO: Probably use ShellItemArray for ShellItem Collections
    public List<ExplorerBrowserItem> CurrentFolderItems
    {
        get;
        private set;
    }

    public ShellItem CurrentFolder;

    public ImageCache ImageCache
    {
        get;
        set;
    }

    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;
        CurrentFolder = ShellFolder.Desktop;
        CurrentFolderItems = [];
        ImageCache = new ImageCache();

        // wire events
        ShellTreeView.NativeTreeView.SelectionChanged += ShellTreeView_SelectionChanged;

        // Initialize root TreeView item(s)
        var ebCurrentFolderItem = new ExplorerBrowserItem(this, CurrentFolder);
        ShellTreeView.InitializeRoot(ebCurrentFolderItem);
        //ShellTreeView.NativeTreeView.SelectedItem = ebCurrentFolderItem;

        TryNavigate(CurrentFolder);
    }

    public void TryNavigate(ShellItem shItem)
    {
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

            var childItems = rootItem.GetChildItems(shItem);
            foreach (var item in childItems)
            {
                rootItem.Children.Add(item);
            }

            // TODO: Rebuild CurrentFolderItems.Clear(); to build complete item list
            CurrentFolderItems = rootItem.Children;

            // Update TreeView and ListView
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
