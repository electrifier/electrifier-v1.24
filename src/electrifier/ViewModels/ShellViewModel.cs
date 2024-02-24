using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using electrifier.Contracts.Services;
using electrifier.Views;

using Microsoft.UI.Xaml.Navigation;

namespace electrifier.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{

    [ObservableProperty]
    private bool isBackEnabled;

    [ObservableProperty]
    private bool isForwardEnabled;

    [ObservableProperty]
    private object? selected;

    public INavigationService NavigationService
    {
        get;
    }

    public INavigationViewService NavigationViewService
    {
        get;
    }

    public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
        // TODO: https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.navigation.framenavigationoptions?view=windows-app-sdk-1.4
        //navigationService.CanForwardChanged += (s, e) => IsForwardEnabled = e;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;

        if (NavigationViewService.TryGetSelectedItem(e.SourcePageType, out var selectedItem))
        {
            Selected = selectedItem;
            return;
        }


        //        if (e.SourcePageType == typeof(SettingsPage))
        //        {
        //            Selected = NavigationViewService.SettingsItem;
        //            return;
        //        }
        //
        //        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        //        if (selectedItem != null)
        //        {
        //            Selected = selectedItem;
        //        }
    }
}
