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
    public ObservableCollection<ExplorerBrowserItem> ExplorerBrowserItems
    {
        get;
        private set;
    }

    public ShellItem CurrentFolder;

    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;
        ExplorerBrowserItems = new ObservableCollection<ExplorerBrowserItem>();
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
            ExplorerBrowserItems = newItems;
            ShellGridView.SetItemsSource(ExplorerBrowserItems); // TODO: Maybe use bind in xaml
            CurrentFolder = shItem;
        }
    }
}
