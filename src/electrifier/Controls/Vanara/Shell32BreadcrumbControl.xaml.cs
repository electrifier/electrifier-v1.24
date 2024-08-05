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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace electrifier.Controls.Vanara;
public sealed partial class Shell32BreadcrumbControl : UserControl
{
    public Shell32BreadcrumbControl()
    {
        this.InitializeComponent();
    }

    public class Folder
    {
        public string Name { get; set; }
    }

    //BreadcrumbBar2.ItemsSource = new ObservableCollection<Folder>{
    //    new Folder { Name = "Home"},
    //    new Folder { Name = "Folder1" },
    //    new Folder { Name = "Folder2" },
    //    new Folder { Name = "Folder3" },
    //};
    //BreadcrumbBar2.ItemClicked += BreadcrumbBar2_ItemClicked;

    private void BreadcrumbBar2_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        var items = BreadcrumbBar2.ItemsSource as ObservableCollection<Folder>;
        for (int i = items.Count - 1; i >= args.Index + 1; i--)
        {
            items.RemoveAt(i);
        }
    }
}
