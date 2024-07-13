using System.Diagnostics;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

    public TreeView NativeTreeView => TreeView;

    public Visibility TopCommandBarVisibility
    {
        get => (Visibility)GetValue(TopCommandBarVisibilityProperty);
        set => SetValue(TopCommandBarVisibilityProperty, value);
    }

    public Visibility FileNameVisibility;
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
}
