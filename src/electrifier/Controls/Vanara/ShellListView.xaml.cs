using CommunityToolkit.WinUI.Collections;
using electrifier.Controls.Vanara.Helpers;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;

// todo: For EnumerateChildren-Calls, add HWND handle
// todo: See ShellItemCollection, perhaps use this instead of ObservableCollection
// https://github.com/dahall/Vanara/blob/master/Windows.Shell.Common/ShellObjects/ShellItemArray.cs

namespace electrifier.Controls.Vanara;

public partial class ShellListView : UserControl
{
    public ItemsView NativeItemsView => ItemsView;
    public ObservableCollection<BrowserItem> Items = new();
    public readonly AdvancedCollectionView AdvancedCollectionView;

    public ShellListView()
    {
        InitializeComponent();
        DataContext = this;
        AdvancedCollectionView = new AdvancedCollectionView(Items, true);
        NativeItemsView.ItemsSource = AdvancedCollectionView;
    }
}