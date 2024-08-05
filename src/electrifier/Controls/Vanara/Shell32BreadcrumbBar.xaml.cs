using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using electrifier.Controls.Vanara;
using Vanara.PInvoke;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace electrifier.Controls.Vanara;
public sealed partial class Shell32BreadcrumbBar : UserControl
{
    public Shell32BreadcrumbBar()
    {
        this.InitializeComponent();

        NativeBreadcrumbBar.ItemsSource = new ObservableCollection<Shell32.Folder> { };
        NativeBreadcrumbBar.ItemClicked += NativeBreadcrumbBar_ItemClicked;
    }

    private void NativeBreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        var items = NativeBreadcrumbBar.ItemsSource as ObservableCollection<Shell32.Folder>;
        for (int i = items.Count - 1; i >= args.Index + 1; i--)
        {
            items.RemoveAt(i);
        }
    }
}
