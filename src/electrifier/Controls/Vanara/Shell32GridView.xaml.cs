using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.WinUI.Collections;

namespace electrifier.Controls.Vanara;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public sealed partial class Shell32GridView : UserControl
{
    public Shell32GridView()
    {
        InitializeComponent();
        DataContext = this;
    }

    public void SetItemsSource(ObservableCollection<ExplorerBrowserItem> itemSourceCollection)
    {
        var acv = new AdvancedCollectionView(itemSourceCollection, true)
        {
            Filter = x => !int.TryParse(((ExplorerBrowserItem)x).DisplayName, out _),
        };
        acv.SortDescriptions.Add(new SortDescription("DisplayName", SortDirection.Ascending));

        GridView.ItemsSource = acv;
    }

    private void GridView_OnItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not ExplorerBrowserItem ebItem)
        {
            return;
        }

        var shItem = ebItem.ShellItem;
        ebItem.Owner.TryNavigate(shItem);
    }

    private string GetDebuggerDisplay()
    {
        return nameof(Shell32GridView) + ToString();
    }
}
