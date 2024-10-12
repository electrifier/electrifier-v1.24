using CommunityToolkit.WinUI.Animations;
using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace electrifier.Views;

public sealed partial class KanbanBoardDetailPage : Page
{
    public KanbanBoardDetailViewModel ViewModel
    {
        get;
    }

    public KanbanBoardDetailPage()
    {
        ViewModel = App.GetService<KanbanBoardDetailViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        this.RegisterElementForConnectedAnimation("animationKeyContentGrid", ItemHero);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        if (e.NavigationMode == NavigationMode.Back)
        {
            //var navigationService = App.GetService<INavigationService>();
            //
            //if (ViewModel.Item != null)
            //{
            //    navigationService.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
            //}
        }
    }
}
