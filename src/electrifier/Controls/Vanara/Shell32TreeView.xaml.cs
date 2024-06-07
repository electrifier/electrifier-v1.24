using System.Collections.ObjectModel;
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
    public Shell32TreeView()
    {
        InitializeComponent();
        DataContext = this;

        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {

    }

    public void SetItemsSource(ShellItem rootItem, ObservableCollection<ExplorerBrowserItem> itemSourceCollection)
    {
        // TODO: add rootItem
        var acv = new AdvancedCollectionView(itemSourceCollection, true)
        {
            Filter = x => ((ExplorerBrowserItem)x).IsFolder
        };
        acv.SortDescriptions.Add(new SortDescription("DisplayName", SortDirection.Ascending));

        if (acv.Count > 0)
        {
            TreeView.ItemsSource = acv;
        }
        else
        {
            TreeView.ItemsSource = itemSourceCollection;
        }
    }

    private void TreeView_OnSelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs e)
    {
        var firstItem = e.AddedItems.First() as ExplorerBrowserItem;

        if (firstItem is not { } ebItem)
        {
            return;
        }

        ebItem.Owner.TryNavigate(ebItem.ShellItem);
    }
}
