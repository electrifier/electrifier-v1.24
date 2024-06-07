using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using Windows.Foundation.Collections;
using Windows.Foundation;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
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
        var clicked = e.ClickedItem;

        if (clicked is not ExplorerBrowserItem ebItem)
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
