using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

// https://github.com/dahall/Vanara/blob/master/Windows.Forms/Controls/ExplorerBrowser.cs
// TODO: See also https://github.com/dahall/Vanara/blob/ac0a1ac301dd4fdea9706688dedf96d596a4908a/Windows.Shell.Common/StockIcon.cs
public sealed partial class ExplorerBrowser : INotifyPropertyChanged
{
    public List<ExplorerBrowserItem> CurrentFolderItems
    {
        get; private set;
    }

    public ShellItem CurrentFolder;

    public ImageCache ImageCache
    {
        get; set;
    }

    public Microsoft.UI.Xaml.Visibility GridViewVisibility
    {
        get; set;
    }

    public Microsoft.UI.Xaml.Visibility TopCommandBarVisibility
    {
        get; set;
    }

    public Microsoft.UI.Xaml.Visibility BottomAppBarVisibility
    {
        get; set;
    }

    public Microsoft.UI.Xaml.Visibility BottomCommandBarVisibility
    {
        get; set;
    }

    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;
        ImageCache = new ImageCache();

        // Initialize root TreeView item(s)
        CurrentFolder = ShellFolder.Desktop;
        CurrentFolderItems = [];
        var ebCurrentFolderItem = new ExplorerBrowserItem(CurrentFolder);
        ShellTreeView.InitializeRoot(ebCurrentFolderItem);
        ShellTreeView.NativeTreeView.SelectedItem = ebCurrentFolderItem;

        // Wire Events
        Loading += ExplorerBrowser_Loading;
        ShellTreeView.NativeTreeView.SelectionChanged += ShellTreeView_SelectionChanged;
    }

    private void ExplorerBrowser_Loading(FrameworkElement sender, object args)
    {
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

        var targetFolder = new ShellFolder(shItem);

        //  Navigate2Target(new ShellItem(shItem.PIDL)); => TODO: Check why a copy of ShItem won't result in expanded TreeNode
        Navigate2Target(targetFolder);
    }

    private void Navigate2Target(ShellFolder targetFolder)
    {
        Debug.Assert(targetFolder is not null);
        var iconExt = new ShellIconExtractor(targetFolder);
        iconExt.IconExtracted += IconExtOnIconExtracted;
        iconExt.Complete += IconExtOnComplete;

        iconExt.Start();


        void IconExtOnIconExtracted(object? sender, ShellIconExtractedEventArgs e)
        {
            var shItem = new ShellItem(e.ItemID);
            Debug.Print($".IconExtOnIconExtracted(): {shItem}");
            CurrentFolderItems.Add(new ExplorerBrowserItem(shItem));
        }

        void IconExtOnComplete(object? sender, EventArgs e)
        {
            var cnt = CurrentFolderItems.Count;
            Debug.Print($".IconExtOnComplete(): {cnt} items");
            if (GridViewVisibility == Microsoft.UI.Xaml.Visibility.Visible)
            {
                Debug.Print($".GridViewVisibility = {Microsoft.UI.Xaml.Visibility.Visible}");
                ShellGridView.SetItemsSource(CurrentFolderItems); // TODO: Throws Exception
            }
        }

        //using var shFolder = new ShellFolder(shItem);
        //try
        //{
        //    var parentItem = new ExplorerBrowserItem(shItem);

        //    var childItems = parentItem.GetChildItems(shItem);
        //    foreach (var item in childItems)
        //    {
        //        parentItem.Children.Add(item);
        //    }

        //    // TODO: Rebuild CurrentFolderItems.Clear(); to build complete item list
        //    CurrentFolderItems = parentItem.Children;

        //    // Update TreeView and ListView
        //    ShellTreeView.SetItemsSource(parentItem, CurrentFolderItems);

        //    if (GridViewVisibility == Microsoft.UI.Xaml.Visibility.Visible)
        //    {
        //        ShellGridView.SetItemsSource(CurrentFolderItems); // TODO: binding
        //    }
        //}
        //finally
        //{
        //    SetField(ref CurrentFolder, shFolder);
        //    // TODO: Raise navigated event
        //    Debug.Write($"TryNavigate: Done {shItem}.");
        //}
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

    #region Property stuff

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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

    #endregion Property stuff
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
