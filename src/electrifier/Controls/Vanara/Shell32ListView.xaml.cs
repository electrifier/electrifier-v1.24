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

public partial class Shell32ListView : UserControl
{
    //public ListView NativeListView => ListView;

    public Shell32ListView()
    {
        InitializeComponent();
        DataContext = this;

        //NativeGridView.ShowsScrollingPlaceholders = true;
        //NativeGridView.ScrollBarVisibility = ScrollBarVisibility.Auto;
    }
}
