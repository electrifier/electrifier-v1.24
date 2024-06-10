using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Vanara.Windows.Shell;
using static electrifier.Controls.Vanara.Shell32TreeView;

namespace electrifier.Controls.Vanara;

// https://github.com/dahall/Vanara/blob/master/Windows.Forms/Controls/ExplorerBrowser.cs

// TODO: See also https://github.com/dahall/Vanara/blob/ac0a1ac301dd4fdea9706688dedf96d596a4908a/Windows.Shell.Common/StockIcon.cs



public sealed partial class ExplorerBrowser : UserControl
{
    // TODO: Use ShellItemArray for ShellItem Collections
    public List<ExplorerBrowserItem> CurrentFolderItems
    {
        get;
        private set;
    }

    public ShellItem CurrentFolder;

    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;
        CurrentFolderItems = new List<ExplorerBrowserItem>();
        CurrentFolder = ShellFolder.Desktop;

        var currentFolderExplorerBrowserItem = new ExplorerBrowserItem(this, CurrentFolder);
        ShellTreeView.InitializeRoot(currentFolderExplorerBrowserItem);
        ShellTreeView.myTreeView.SelectionChanged += MyTreeView_SelectionChanged;
        //ShellTreeView.myTreeView.SelectedItem = currentFolderExplorerBrowserItem;

        TryNavigate(CurrentFolder);
    }

    public void TryNavigate(ShellItem shItem)
    {

        //var newItems = new List<ExplorerBrowserItem>();
        var rootItem = new ExplorerBrowserItem(this, shItem);

        try
        {
            foreach (var item in rootItem.GetChildItems(shItem))
            {
                rootItem.Children.Add(item);
                //newItems.Add(item);     // TODO: Add directly to ExplorerBrowserItem
            }
        }
        finally
        {
            CurrentFolderItems = rootItem.Children;
            ShellTreeView.SetItemsSource(rootItem, CurrentFolderItems);
            ShellGridView.SetItemsSource(CurrentFolderItems); // TODO: Maybe use bind in xaml
            CurrentFolder = shItem;
        }
    }

    private void MyTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        var selectedNode = ShellTreeView.myTreeView.SelectedNode;
        var selectedItem = ShellTreeView.myTreeView.SelectedItem;
        var addedItems = args.AddedItems;
        var removedItems= args.RemovedItems;

        Debug.Print($"MyTreeView_SelectionChanged SelectedItem: { selectedItem } ");
        
        if (selectedNode != null)
        {
            var nodeContent = selectedNode.Content;

            if (nodeContent is ExplorerBrowserItem ebItem)
            {
                Debug.Print($"nameof({MyTreeView_SelectionChanged}) - {ebItem}");
                TryNavigate(ebItem.ShellItem);
            }
        }
    }



    #region The following is original copy & paste from Vanara
    /// <summary>Event argument for The Navigated event</summary>
    public class NavigatedEventArgs : EventArgs
    {
        /// <summary>The new location of the explorer browser</summary>
        public ShellItem? NewLocation { get; set; }
    }

    /// <summary>Event argument for The Navigating event</summary>
    public class NavigatingEventArgs : EventArgs
    {
        /// <summary>Set to 'True' to cancel the navigation.</summary>
        public bool Cancel { get; set; }

        /// <summary>The location being navigated to</summary>
        public ShellItem? PendingLocation { get; set; }
    }

    /// <summary>Event argument for the NavigatinoFailed event</summary>
    public class NavigationFailedEventArgs : EventArgs
    {
        /// <summary>The location the browser would have navigated to.</summary>
        public ShellItem? FailedLocation { get; set; }
    }
    #endregion

}
