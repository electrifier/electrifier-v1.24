using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Diagnostics;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

// todo: For EnumerateChildren-Calls, add HWND handle
// todo: See ShellItemCollection, perhaps use this instead of ObservableCollection
// https://github.com/dahall/Vanara/blob/master/Windows.Shell.Common/ShellObjects/ShellItemArray.cs

namespace electrifier.Controls.Vanara;

public partial class ShellNamespaceTreeControl : UserControl
{
    public TreeView NativeTreeView => TreeView;

    public ShellNamespaceTreeControl()
    {
        InitializeComponent();
        DataContext = this;

        var desk = BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Desktop);
        desk.ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_ComputerFolder) );

        var itms = new List<BrowserItem>
        {
            desk,
            BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Desktop),
            BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_ComputerFolder),
            BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_AccountPictures),
            BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Videos),
        };
        NativeTreeView.ItemsSource = itms;
    }

    // TODO: public object ItemFromContainer => NativeTreeView.ItemFromContainer()
}
