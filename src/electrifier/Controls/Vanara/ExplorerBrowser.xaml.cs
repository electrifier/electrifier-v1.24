using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using System.Collections.ObjectModel;
using Vanara.Windows.Shell;
using Windows.Foundation.Collections;
using Windows.Foundation;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Media.Imaging;

namespace electrifier.Controls.Vanara;

public sealed partial class ExplorerBrowser : UserControl
{
    public ObservableCollection<ExplorerBrowserItem>? oc;

    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;

        // Set up the original list with a few sample items
        oc = new ObservableCollection<ExplorerBrowserItem>
        {
            new ExplorerBrowserItem(ShellFolder.Desktop),
        };


        //oc.Append()

        foreach (var item in oc[0]?.GetChildItems())
        {
            oc.Add(item);
        }

        ShellGridView.SetItemsSource(oc);
    }
}
