using electrifier.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace electrifier.Views;

public sealed partial class WorkbenchPage : Page
{
    public WorkbenchViewModel ViewModel
    {
        get;
    }

    // TODO: This is a dummy property to demonstrate data binding.
    public bool IsCardEnabled
    {
        get;
        private set;
    } = true;

    public WorkbenchPage()
    {
        ViewModel = App.GetService<WorkbenchViewModel>();
        InitializeComponent();
    }
}
