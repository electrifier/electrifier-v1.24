using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace electrifier.Controls.Vanara;

//[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public partial class Shell32GridView : UserControl
{
    public GridView NativeGridView => GridView;

    public Shell32GridView()
    {
        InitializeComponent();
        DataContext = this;

        NativeGridView.ShowsScrollingPlaceholders = true;
        //NativeGridView.ScrollBarVisibility = ScrollBarVisibility.Auto;
    }
}
