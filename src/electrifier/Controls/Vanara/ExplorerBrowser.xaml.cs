using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

public sealed partial class ExplorerBrowser : UserControl
{
    public ObservableCollection<ExplorerBrowserItem> CurrentFolderItems
    {
        get;
        private set;
    }

    public ShellItem CurrentFolder;

    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;
        CurrentFolderItems = new ObservableCollection<ExplorerBrowserItem>();
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
            CurrentFolderItems = newItems;
            ShellTreeView.SetItemsSource(shItem, CurrentFolderItems);
            ShellGridView.SetItemsSource(CurrentFolderItems); // TODO: Maybe use bind in xaml
            CurrentFolder = shItem;
        }
    }
}
