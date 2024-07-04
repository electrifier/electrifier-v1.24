using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace electrifier.Views;

public sealed partial class KanbanBoardPage : Page
{
    public KanbanBoardViewModel ViewModel
    {
        get;
    }

    public KanbanBoardPage()
    {
        ViewModel = App.GetService<KanbanBoardViewModel>();
        InitializeComponent();
    }
}
