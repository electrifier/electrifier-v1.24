using System.Diagnostics;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// TODO: For EnumerateChildren-Calls, add HWND handle
// TODO: See ShellItemCollection, perhaps use this instead of ObservableCollection
// https://github.com/dahall/Vanara/blob/master/Windows.Shell.Common/ShellObjects/ShellItemArray.cs

namespace electrifier.Controls.Vanara;

public sealed partial class Shell32TreeView : UserControl
{
    private readonly List<ExplorerBrowserItem> _items = [];
    private AdvancedCollectionView _advancedCollectionView;

    public TreeView NativeTreeView => TreeView;

    public Visibility FileNameVisibility;

    public Shell32TreeView()
    {
        InitializeComponent();
        DataContext = this;

        _advancedCollectionView = new AdvancedCollectionView(_items, true);

        //_advancedCollectionView.SortDescriptions.Add(new SortDescription("IsFolder", SortDirection.Descending));
        //_advancedCollectionView.SortDescriptions.Add(new SortDescription("DisplayName", SortDirection.Ascending));
        //TreeView.ItemsSource = _advancedCollectionView;
    }

    public void InitializeRoot(ExplorerBrowserItem rootItem)
    {
        if (rootItem == null)
        {
            throw new ArgumentNullException(nameof(rootItem));
        }

        _items.Add(rootItem);

        UpdateCollectionView();
    }

    private void UpdateCollectionView()
    {
        _advancedCollectionView = new AdvancedCollectionView(_items, true)
        {
            Filter = (x => ((ExplorerBrowserItem)x).IsFolder == true)
        };

        _advancedCollectionView.SortDescriptions.Add(new SortDescription("DisplayName", SortDirection.Ascending));
        TreeView.ItemsSource = _advancedCollectionView;
    }

    public void SetItemsSource(ExplorerBrowserItem parentItem, List<ExplorerBrowserItem> itemSourceCollection)
    {
        Debug.WriteLine("Entering SetItemsSource()");
        FindParentNode(parentItem);

        //if (result != null)
        {
            Debug.WriteLine("JUChU!");
        }

        var targetItem = _items.Find(x => parentItem.ShellItem.Equals(x.ShellItem));
        if (targetItem != null)
        {
            targetItem.Children = itemSourceCollection;
            targetItem.IsExpanded = true;
        }
        else
        {
            if (FindNodeInCollection())
            {

            }
        }

        UpdateCollectionView();
        return;

        bool FindNodeInCollection()
        {
            var newTargetItem = _items[0].Children.Find(x => parentItem.ShellItem.Equals(x.ShellItem));
            if (newTargetItem != null)
            {
                newTargetItem.Children = itemSourceCollection;
                newTargetItem.IsExpanded = true;
                return true;
            }
            else
            {
                Debug.Print("TreeView.SetItemsSource found no targetItem to add folder items to.");
            }

            return false;
        }
    }

    /// <summary>
    /// Sucht nach dem übergeordneten TreeNode, der dem übergeordneten Ordner entspricht...
    /// </summary>
    /// <param name="source"></param>
    public async void FindParentNode(ExplorerBrowserItem source)
    {
        var rootNode = source.ShellItem;
        //Debug.Assert(rootNode.PIDL.IsParentOf());

        var parentItem = rootNode.Parent;
        // rootNode.PIDL.IsParentOf()

        return ; // return ExplorerBrowserItem? parentItem
    }
    //public event ParentShellItemFound...

    public class TreeViewSelectionChanged(IList<object> addedItems, IList<object> removedItems) : EventArgs
    {
        public IList<object> AddedItems { get; } = addedItems ?? throw new ArgumentNullException(nameof(addedItems));
        public IList<object> RemovedItems { get; } = removedItems ?? throw new ArgumentNullException(nameof(removedItems));
    }
}
