using CommunityToolkit.WinUI.Collections;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Diagnostics;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using Visibility = Microsoft.UI.Xaml.Visibility;

// TODO: For EnumerateChildren-Calls, add HWND handle
// TODO: See ShellItemCollection, perhaps use this instead of ObservableCollection
// https://github.com/dahall/Vanara/blob/master/Windows.Shell.Common/ShellObjects/ShellItemArray.cs

namespace electrifier.Controls.Vanara;

public sealed partial class Shell32TreeView : UserControl
{
    private readonly List<ExplorerBrowserItem> _items = [];

    public TreeView NativeTreeView => TreeView;

    public ExplorerBrowserItem? SelectedItem
    {
        get => (ExplorerBrowserItem?)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(nameof(SelectedItem), typeof(ExplorerBrowserItem), typeof(Shell32TreeView),
            new PropertyMetadata(default(ExplorerBrowserItem?)));

    public TreeViewNode SelectedNode => NativeTreeView.SelectedNode;

    public Shell32TreeView()
    {
        InitializeComponent();
        DataContext = this;
    }

    // TODO: public object ItemFromContainer => NativeTreeView.ItemFromContainer()

    public async Task InitializeRoot(List<ExplorerBrowserItem> rootItems)
    {
        if (rootItems == null) 
        {
            throw new ArgumentNullException(nameof(rootItems));
        }

        if ((rootItems.Count == 0))
        {
            throw new ArgumentException(".InitializeRoot() ERROR: Array of RootItems is empty.");
        }

        _items.AddRange(rootItems);
        NativeTreeView.ItemsSource = _items;
        //SelectedItem = rootItems;
    }

    //private void NativeTreeView_OnSelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    //{
    //    // TODO: Add Multi-Select abilities
    //    var selectedNode = NativeTreeView.SelectedNode;

    //    if (selectedNode != null)
    //    {
    //        var nodeContent = selectedNode.Content;

    //        if (nodeContent is ExplorerBrowserItem ebItem)
    //        {
    //            Debug.Print($".NativeTreeView_SelectionChanged: SelectedItem `{ebItem.DisplayName}`");

    //            SelectedItem = ebItem;
    //            // TODO: Initiate Navigation. Change `CurrentFolderBrowserItem` on ExplorerBrowser
    //        }
    //    }
    //}
}
