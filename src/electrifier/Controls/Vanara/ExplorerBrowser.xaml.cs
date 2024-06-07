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
    public ObservableCollection<ExplorerBrowserItem> oc
    {
        get;
        private set;
    }

    public ShellItem CurrentFolder;

    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;
        oc = new ObservableCollection<ExplorerBrowserItem>();
        CurrentFolder = ShellFolder.Desktop;
        
        TryNavigate(CurrentFolder);
    }

    public void TryNavigate(ShellItem shItem)
    {
        var newItems = new ObservableCollection<ExplorerBrowserItem>();

        try
        {
            var rootItem = new ExplorerBrowserItem(this, shItem);

            foreach (var item in rootItem.GetChildItems(shItem))
            {
                newItems.Add(item);
            }
        }
        finally
        {
            if (oc is not null)     // TODO: should be not null able
            {
                oc.Clear();

                foreach (var item in newItems)
                {
                    oc.Add(item);
                }

                ShellGridView.SetItemsSource(oc);
                CurrentFolder = shItem;
            }
        }
    }
}
