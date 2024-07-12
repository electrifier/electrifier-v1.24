using System.Diagnostics;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public sealed partial class Shell32GridView : UserControl
{
    public List<ExplorerBrowserItem> Items = [];

    public Shell32GridView()
    {
        InitializeComponent();
        DataContext = this;
    }

    public void SetItemsSource(List<ExplorerBrowserItem> itemSourceCollection)
    {
        try
        {
            //var oc = new ObservableCollection<ExplorerBrowserItem>();
            Items.AddRange(itemSourceCollection);

            NativeGridView.ItemsSource = Items;

            //var acv = new AdvancedCollectionView(itemSourceCollection, true);
            //acv.SortDescriptions.Add(new SortDescription("IsFolder", SortDirection.Descending));
            //acv.SortDescriptions.Add(new SortDescription("DisplayName", SortDirection.Ascending));
            //NativeGridView.ItemsSource = acv;
            //NativeGridView.ItemsSource = oc;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void GridView_OnItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not ExplorerBrowserItem ebItem)
        {
            return;
        }

        var shItem = ebItem.ShellItem;
        // TODO: ebItem.Owner.TryNavigate(shItem);
    }

    private string GetDebuggerDisplay()
    {
        return nameof(Shell32GridView) + ToString();
    }
}
