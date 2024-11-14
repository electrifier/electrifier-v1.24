using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Diagnostics;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using Windows.Foundation;
using CommunityToolkit.WinUI.Collections;
using System.Collections.ObjectModel;
using electrifier.Controls.Vanara.Services;

// todo: For EnumerateChildren-Calls, add HWND handle
// todo: See ShellItemCollection, perhaps use this instead of ObservableCollection
// https://github.com/dahall/Vanara/blob/master/Windows.Shell.Common/ShellObjects/ShellItemArray.cs

namespace electrifier.Controls.Vanara;

public partial class ShellNamespaceTreeControl : UserControl
{
    public TreeView NativeTreeView => TreeView;
    public ObservableCollection<BrowserItem> Items;
    private AdvancedCollectionView _advancedCollectionView;
    public static ShellNamespaceService NamespaceService => App.GetService<ShellNamespaceService>();

    public ShellNamespaceTreeControl()
    {
        InitializeComponent();
        DataContext = this;
        Items = new ObservableCollection<BrowserItem>();
        _advancedCollectionView = new AdvancedCollectionView(Items, true);

        Loading += ShellNamespaceTreeControl_Loading;
        NativeTreeView.ItemsSource = _advancedCollectionView;
        NativeTreeView.SelectionChanged += NativeTreeView_SelectionChanged;
    }

    private void ShellNamespaceTreeControl_Loading(FrameworkElement sender, object args)
    {
        Items.Add(BrowserItem.FromShellFolder(ShellNamespaceService.HomeShellFolder));
        Items.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Desktop));
        Items.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Desktop));
        Items.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Downloads));
        Items.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Documents));
        Items.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Pictures));
        Items.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Music));
        Items.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Videos));
    }

    private void NativeTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        if (sender == null)
        {
            Debug.Print($"WARN NativeTreeView_SelectionChanged({sender: null})");
        }
        if (args.AddedItems.FirstOrDefault() is BrowserItem addedItem)
        {
            Debug.Print($".NativeTreeView_SelectionChanged({addedItem})");
        }
    }

    // TODO: public object ItemFromContainer => NativeTreeView.ItemFromContainer()
}
