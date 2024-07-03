using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.UI.Notifications;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Vanara.Windows.Shell;

// TODO: For EnumerateChildren-Calls, add HWND handle
// TODO: See ShellItemCollection, perhaps use this instead of ObservableCollection
// https://github.com/dahall/Vanara/blob/master/Windows.Shell.Common/ShellObjects/ShellItemArray.cs

namespace electrifier.Controls.Vanara;

public sealed partial class Shell32TreeView : UserControl
{
//    public SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);
    //public delegate void SelectionChangedEventHandler   //.SelectionChanged;

    private readonly List<ExplorerBrowserItem> _items;
    private AdvancedCollectionView _advancedCollectionView;

    public TreeView myTreeView => TreeView;

    public Shell32TreeView()
    {
        InitializeComponent();
        DataContext = this;

        _items = new List<ExplorerBrowserItem>();
        _advancedCollectionView = new AdvancedCollectionView(_items, true)
        {
            Filter = x => ((ExplorerBrowserItem)x).IsFolder
        };

        _advancedCollectionView.SortDescriptions.Add(new SortDescription("DisplayName", SortDirection.Ascending));
        TreeView.ItemsSource = _advancedCollectionView;


        //var test = this.TreeView.SelectionChanged
        //object slChanged = this.TreeView.SelectionChanged;
        // TreeView_OnSelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs e)

        //this.Loaded += Shell32TreeView_Loaded;
        //this.Loading += Shell32TreeView_Loading;
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
            Filter = x => ((ExplorerBrowserItem)x).IsFolder
        };

        _advancedCollectionView.SortDescriptions.Add(new SortDescription("DisplayName", SortDirection.Ascending));
        TreeView.ItemsSource = _advancedCollectionView;
    }

    //private void Shell32TreeView_Loading(FrameworkElement sender, object args) => throw new NotImplementedException();

    //private void Shell32TreeView_Loaded(object sender, RoutedEventArgs e) => throw new NotImplementedException();

    public void SetItemsSource(ExplorerBrowserItem parentItem, List<ExplorerBrowserItem> itemSourceCollection)
    {
        //var rootItem = _items[0];
        //ExplorerBrowserItem destinationTargetItem = null;
        //if (rootItem.ShellItem == parentItem.ShellItem)
        //    destinationTargetItem = rootItem;
        //else
        //{
        //    destinationTargetItem = rootItem;
        //}
        //destinationTargetItem.Children = itemSourceCollection;

        var targetItem = _items.Find(x => parentItem.ShellItem.Equals(x.ShellItem));
        if (targetItem != null)
        {
            targetItem.Children = itemSourceCollection;
        }
        else
        {
            var newTargetItem = _items[0].Children.Find(x => parentItem.ShellItem.Equals(x.ShellItem));
            if (newTargetItem != null)
            {
                newTargetItem.Children = itemSourceCollection;
                newTargetItem.IsExpanded = true;
            }
            else
            {
                Debug.Print("TreeView.SetItemsSource found no targetItem to add folder items to.");
            }
        }

        //parentItem.Children = itemSourceCollection;
        //_items.Add(parentItem);
        //var myItem = _items[0];
        //myItem.Children = itemSourceCollection;

        UpdateCollectionView();

//        var  = TreeView.FindChildOrSelf<ExplorerBrowserItem>(parentItem);
        //FindChildOrSelf(parentItem);
        // TODO: add this collection to rootItem

    }

    public class TreeViewSelectionChanged(IList<object> addedItems, IList<object> removedItems) : EventArgs
    {
        public IList<object> AddedItems { get; } = addedItems ?? throw new ArgumentNullException(nameof(addedItems));
        public IList<object> RemovedItems { get; } = removedItems ?? throw new ArgumentNullException(nameof(removedItems));
    }
}
