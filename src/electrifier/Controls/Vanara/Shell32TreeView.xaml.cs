using System.Collections.ObjectModel;
using Windows.UI.Notifications;
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

    public TreeView myTreeView => TreeView;

    public Shell32TreeView()
    {
        InitializeComponent();
        DataContext = this;

        //var test = this.TreeView.SelectionChanged
        //object slChanged = this.TreeView.SelectionChanged;
        // TreeView_OnSelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs e)

        //this.Loaded += Shell32TreeView_Loaded;
        //this.Loading += Shell32TreeView_Loading;
    }



    public void InitializeRoot(ShellItem currentFolder)
    {
        //if (currentFolder == null)
        //{
        //    var acv = new AdvancedCollectionView(itemSourceCollection, true)
        //    {
        //        Filter = x => ((ExplorerBrowserItem)x).IsFolder
        //    };
        //    acv.SortDescriptions.Add(new SortDescription("DisplayName", SortDirection.Ascending));
        //}
    }

    //private void Shell32TreeView_Loading(FrameworkElement sender, object args) => throw new NotImplementedException();

    //private void Shell32TreeView_Loaded(object sender, RoutedEventArgs e) => throw new NotImplementedException();

    public void SetItemsSource(ShellItem rootItem, List<ExplorerBrowserItem> itemSourceCollection)
    {
        // TODO: add rootItem
        var acv = new AdvancedCollectionView(itemSourceCollection, true)
        {
            Filter = x => ((ExplorerBrowserItem)x).IsFolder
        };
        acv.SortDescriptions.Add(new SortDescription("DisplayName", SortDirection.Ascending));

        TreeView.ItemsSource = acv;

        //if (acv.Count > 0)
        //{
        //    TreeView.ItemsSource = acv;
        //}
        //else
        //{
        //    TreeView.ItemsSource = itemSourceCollection;
        //}
    }

    public class TreeViewSelectionChanged : EventArgs
    {
        public IList<object> AddedItems { get; }
        public IList<object> RemovedItems { get; }
    }
}
