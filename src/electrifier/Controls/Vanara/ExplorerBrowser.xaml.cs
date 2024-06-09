using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Vanara.Windows.Shell;
using static electrifier.Controls.Vanara.Shell32TreeView;

namespace electrifier.Controls.Vanara;

// https://github.com/dahall/Vanara/blob/master/Windows.Forms/Controls/ExplorerBrowser.cs

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

        ShellTreeView.InitializeRoot(CurrentFolder);
        ShellTreeView.myTreeView.SelectionChanged += MyTreeView_SelectionChanged;

        TryNavigate(CurrentFolder);
    }

    public void TryNavigate(ShellItem shItem)
    {
        var newItems = new List<ExplorerBrowserItem>();

        try
        {
            var rootItem = new ExplorerBrowserItem(this, shItem);

            foreach (var item in rootItem.GetChildItems(shItem))
            {
                newItems.Add(item);
            }
        }
        finally
        {
            CurrentFolderItems = newItems;
//            ShellTreeView.SetItemsSource(shItem, CurrentFolderItems);
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
                //TryNavigate(ebItem.ShellItem);
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
