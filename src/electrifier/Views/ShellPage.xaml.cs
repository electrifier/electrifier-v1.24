using electrifier.Contracts.Services;
using electrifier.Helpers;
using electrifier.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace electrifier.Views;

public sealed partial class ShellPage
{
    public ShellViewModel ViewModel
    {
        get;
    }

    /// <summary>
    /// Create a new instance of ShellPage using default values.
    /// </summary>
    /// <param name="viewModel"><see cref="ShellViewModel"/></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        InitializeComponent();

        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);

        // TODO: Set the title bar icon by updating /Assets/WindowIcon.ico.
        // A custom title bar is required for full window theme and Mica support.
        // https://docs.microsoft.com/windows/apps/develop/title-bar?tabs=winui3#full-customization
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);

        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));


        NavigationBreadcrumbBar.ItemsSource = new string[]
            { "Home", "Documents" };
        //var targetPageType = typeof(WorkbenchPage);
        //string targetPageKey = targetPageType.FullName;  // .ToString()
        //string targetPageArguments = string.Empty;
        ////rootPage.Navigate(targetPageType, targetPageArguments);
        //ViewModel.NavigationService.NavigateTo(targetPageKey, targetPageArguments);
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        //App.AppTitleBar = AppTitleBarTextBlock;
    }

    private void AppTitleBar_BackButton_Click(object sender, RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.GoBack();
    }
    private void AppTitleBar_ForwardButton_Click(object sender, RoutedEventArgs e)
    {
        var navigationService = App.GetService<INavigationService>();

        navigationService.GoForward();
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        AppTitleBar.Margin = new Thickness()
        {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom
        };
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        var navigationService = App.GetService<INavigationService>();

        var result = navigationService.GoBack();

        args.Handled = result;
    }
    private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Needs to set focus explicitly due to WinUI 3 regression https://github.com/microsoft/microsoft-ui-xaml/issues/8816 
        ((Control)sender).Focus(FocusState.Programmatic);
    }
}
