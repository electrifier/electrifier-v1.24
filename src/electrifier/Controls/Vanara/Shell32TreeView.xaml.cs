using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

// todo: For EnumerateChildren-Calls, add HWND handle
// todo: See ShellItemCollection, perhaps use this instead of ObservableCollection
// https://github.com/dahall/Vanara/blob/master/Windows.Shell.Common/ShellObjects/ShellItemArray.cs

namespace electrifier.Controls.Vanara;

public sealed partial class Shell32TreeView : UserControl
{
    public TreeView NativeTreeView => TreeView;

    public Shell32TreeView()
    {
        InitializeComponent();
        DataContext = this;

        var itms = new List<BrowserItem>
        {
            BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Desktop),
            new(ShellFolder.Desktop.PIDL, true)
        };
        TreeView.ItemsSource = itms;

    }

    // TODO: public object ItemFromContainer => NativeTreeView.ItemFromContainer()
}
