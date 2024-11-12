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

// todo: For EnumerateChildren-Calls, add HWND handle
// todo: See ShellItemCollection, perhaps use this instead of ObservableCollection
// https://github.com/dahall/Vanara/blob/master/Windows.Shell.Common/ShellObjects/ShellItemArray.cs

namespace electrifier.Controls.Vanara;

public partial class ShellNamespaceTreeControl : UserControl
{
    public TreeView NativeTreeView => TreeView;
    public ObservableCollection<BrowserItem> Items;
    private AdvancedCollectionView _advancedCollectionView;

    public ShellNamespaceTreeControl()
    {
        InitializeComponent();
        DataContext = this;

        Items = new ObservableCollection<BrowserItem>();
        _advancedCollectionView = new AdvancedCollectionView(Items, true);
        NativeTreeView.ItemsSource = _advancedCollectionView;

        var desk = BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Desktop);
        desk.ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_ComputerFolder) );
        Items.Add(desk);

        NativeTreeView.SelectionChanged += NativeTreeView_SelectionChanged;

    }

    private void NativeTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        Debug.Print($"NativeTreeView_SelectionChanged()");
        var snd = sender;
        var argsitem = args;

        if (snd != null)
        {
            var additm = args.AddedItems;
            var remitm = args.RemovedItems;
        }
    }

    // TODO: public object ItemFromContainer => NativeTreeView.ItemFromContainer()
}
