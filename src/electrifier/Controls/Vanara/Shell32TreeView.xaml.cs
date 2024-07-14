using CommunityToolkit.WinUI.Collections;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Diagnostics;
using Vanara.Windows.Shell;
using Visibility = Microsoft.UI.Xaml.Visibility;

// TODO: For EnumerateChildren-Calls, add HWND handle
// TODO: See ShellItemCollection, perhaps use this instead of ObservableCollection
// https://github.com/dahall/Vanara/blob/master/Windows.Shell.Common/ShellObjects/ShellItemArray.cs

namespace electrifier.Controls.Vanara;

public sealed partial class Shell32TreeView : UserControl
{
    private readonly List<ExplorerBrowserItem> _items = [];
    private AdvancedCollectionView _advancedCollectionView;

    private TreeView NativeTreeView => TreeView;

    public Visibility FileNameVisibility
    {
        get => (Visibility)GetValue(FileNameVisibilityProperty);
        set => SetValue(FileNameVisibilityProperty, value);
    }

    public static readonly DependencyProperty FileNameVisibilityProperty = DependencyProperty.Register(nameof(FileNameVisibility), typeof(Visibility), typeof(Shell32TreeView), new PropertyMetadata(default(Visibility)));

    public ExplorerBrowserItem? SelectedItem
    {
        get;
        set;
    }

    public TreeViewNode SelectedNode => NativeTreeView.SelectedNode;

    public Visibility TopCommandBarVisibility
    {
        get => (Visibility)GetValue(TopCommandBarVisibilityProperty);
        set => SetValue(TopCommandBarVisibilityProperty, value);
    }

    public static readonly DependencyProperty TopCommandBarVisibilityProperty = DependencyProperty.Register(nameof(TopCommandBarVisibility), typeof(Visibility), typeof(Shell32TreeView), new PropertyMetadata(default(Visibility)));

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

    public void SetItemsSource(ExplorerBrowserItem folder, List<ExplorerBrowserItem> itemSourceCollection)
    {
        Debug.WriteLine($".SetItemsSource(): `{folder.DisplayName}` {itemSourceCollection.Count} items.");

        if (_items.Find(x => x.ShellItem.PIDL.Equals(folder.ShellItem.PIDL)) is { } node)
        {
            node.Children = itemSourceCollection;
            node.IsExpanded = true;
        }
        else
        {
            Debug.Print("SetItemsSource() failed: Folder Target not found!");
        }
    }

    private void NativeTreeView_OnSelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        // TODO: Add Multi-Select abilities
        var selectedNode = NativeTreeView.SelectedNode;

        if (selectedNode != null)
        {
            var nodeContent = selectedNode.Content;

            if (nodeContent is ExplorerBrowserItem ebItem)
            {
                Debug.Print($".NativeTreeView_SelectionChanged: SelectedItem `<{ebItem.DisplayName}>`");

                SelectedItem = ebItem;
            }
        }
    }
}
