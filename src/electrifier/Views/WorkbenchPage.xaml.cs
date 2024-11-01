using electrifier.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
namespace electrifier.Views;
public sealed partial class WorkbenchPage : Page
{
    public WorkbenchPage()
    {
        App.GetService<WorkbenchViewModel>();
        InitializeComponent();
    }
    private void ArenaGrid_OnDropCompleted(UIElement sender, DropCompletedEventArgs args)
    {
        throw new NotImplementedException();
    }
}
