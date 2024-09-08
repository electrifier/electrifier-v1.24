using System.Diagnostics;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using electrifier.Contracts.Services;
using electrifier.Views;
using Microsoft.UI.Xaml.Navigation;

namespace electrifier.ViewModels;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool isBackEnabled;
    [ObservableProperty]
    private bool isForwardEnabled = true;
    [ObservableProperty]
    private object? selected;

    /// <summary>
    /// Build version description.
    /// </summary>
    //[ObservableProperty]
    public readonly string BuildVersionDescription = "v1.24.803 octavian";

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
    protected void UnselectNavigationItem()
    {
        Selected = null;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;

        if (e.SourcePageType == typeof(SettingsPage))
        {
            Selected = NavigationViewService.SettingsItem;

            return;
        }

        if (NavigationViewService.TryGetSelectedItem(e.SourcePageType, out var selectedItem))
        {
            Selected = selectedItem;
            return;
        }

        UnselectNavigationItem();
    }

    private string GetDebuggerDisplay()
    {
        var dbgDisplay = new StringBuilder();
        _ = dbgDisplay.Append(nameof(ShellViewModel));
        _ = dbgDisplay.Append(' ');
        _ = dbgDisplay.Append(Selected?.ToString() ?? "null");
        return dbgDisplay.ToString();
    }
}
