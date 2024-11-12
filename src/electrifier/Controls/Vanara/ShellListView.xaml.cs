using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.WinUI.Collections;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

// todo: For EnumerateChildren-Calls, add HWND handle
// todo: See ShellItemCollection, perhaps use this instead of ObservableCollection
// https://github.com/dahall/Vanara/blob/master/Windows.Shell.Common/ShellObjects/ShellItemArray.cs


namespace electrifier.Controls.Vanara;

public partial class ShellListView : UserControl
{
    public ListView NativeListView => ListView;
    public ObservableCollection<BrowserItem> Items;
    private AdvancedCollectionView _advancedCollectionView;

    public ShellListView()
    {
        InitializeComponent();
        DataContext = this;
        Items = new ObservableCollection<BrowserItem>();
        _advancedCollectionView = new AdvancedCollectionView(Items, true);
        NativeListView.ItemsSource = _advancedCollectionView;


        Items.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_ComputerFolder));
        Items.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_ComputerFolder));
        Items.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_ComputerFolder));
        Items.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_ComputerFolder));

        //NativeGridView.ShowsScrollingPlaceholders = true;
        //NativeGridView.ScrollBarVisibility = ScrollBarVisibility.Auto;
    }
}
