using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Vanara.Windows.Shell;

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
