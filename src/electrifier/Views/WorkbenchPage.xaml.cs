using electrifier.Controls.Vanara.Services;
using electrifier.Controls.Vanara;
using electrifier.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
namespace electrifier.Views;
public sealed partial class WorkbenchPage : Page
{
    //public IAsyncEnumerable<BrowserItem> DriveItems = RequestDriveItemsAsync();

    public WorkbenchPage()
    {
        App.GetService<WorkbenchViewModel>();
        InitializeComponent();

        //var drives = Awaiter(RequestDrivesAsync());
        //foreach (var drive in RequestDrivesAsync())
        //{
        //}

    }

    public static async IAsyncEnumerable<BrowserItem> RequestDriveItemsAsync()
    {
        yield return (BrowserItem.FromShellFolder(new(@"c:\")));
        //yield return (BrowserItem.FromShellFolder(new(@"d:\")));
        //yield return (BrowserItem.FromShellFolder(new(@"f:\")));
    }

    private void ArenaGrid_OnDropCompleted(UIElement sender, DropCompletedEventArgs args)
    {
        throw new NotImplementedException();
    }
}
