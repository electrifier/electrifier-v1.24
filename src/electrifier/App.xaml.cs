using electrifier.Activation;
using electrifier.Contracts.Services;
using electrifier.Controls.Vanara.Contracts;
using electrifier.Controls.Vanara.Services;
using electrifier.Helpers;
using electrifier.Models;
using electrifier.Notifications;
using electrifier.Services;
using electrifier.ViewModels;
using electrifier.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using WinUIEx;
using static Microsoft.Extensions.Hosting.Host;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

namespace electrifier;

/// <summary>
/// The .NET <seealso cref="IHost">Generic Host</seealso> provides dependency injection, configuration, logging, and other services:
/// <a href="https://docs.microsoft.com/dotnet/core/extensions/generic-host"/>,
/// <a href="https://docs.microsoft.com/dotnet/core/extensions/dependency-injection"/>,
/// <a href="https://docs.microsoft.com/dotnet/core/extensions/configuration"/>,
/// <a href="https://docs.microsoft.com/dotnet/core/extensions/logging"/>
/// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
/// </summary>
public partial class App : Application
{
    public static UIElement? AppTitleBar
    {
        get; set;
    }
    public IHost Host
    {
        get;
    }
    public static WindowEx MainWindow { get; } = new MainWindow();
    
    public App()
    {
        InitializeComponent();
        UnhandledException += App_UnhandledException;

        Host = CreateDefaultBuilder().UseContentRoot(AppContext.BaseDirectory)
            .ConfigureServices((context, services) =>
            {
                // Default Activation Handler
                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // Other Activation Handlers
                services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

                // Services
                services.AddSingleton<IAppNotificationService, AppNotificationService>();
                services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
                services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                services.AddTransient<IWebViewService, WebViewService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();

                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // Core Services
                services.AddSingleton<IShellNamespaceService, ShellNamespaceService>();
                services.AddSingleton<IFileService, FileService>();

                // Views and ViewModels
                services.AddTransient<Microsoft365ViewModel>();
                services.AddTransient<Microsoft365Page>();
                services.AddTransient<KanbanBoardDetailViewModel>();
                services.AddTransient<KanbanBoardDetailPage>();
                services.AddTransient<KanbanBoardViewModel>();
                services.AddTransient<KanbanBoardPage>();
                services.AddTransient<TextEditorViewModel>();
                services.AddTransient<TextEditorPage>();
                services.AddTransient<FileManagerViewModel>();
                services.AddTransient<FileManagerPage>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<SettingsPage>();
                services.AddTransient<WebViewViewModel>();
                services.AddTransient<WebViewPage>();
                services.AddTransient<WorkbenchViewModel>();
                services.AddTransient<WorkbenchPage>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();

                // Configuration
                services.Configure<LocalSettingsOptions>(
                    context.Configuration.GetSection(nameof(LocalSettingsOptions)));
            }).Build();

        GetService<IAppNotificationService>().Initialize();
    }

    /// <summary>
    /// OnLaunched
    /// </summary>
    /// <param name="args"></param>
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        GetService<IAppNotificationService>()
            .Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        await GetService<IActivationService>().ActivateAsync(args);
    }

    /// <summary>
    /// Log and handle exceptions as appropriate.
    /// Best practices:
    /// - <see href="https://docs.microsoft.com/windows/apps/design/app-patterns/handling-exceptions">Handling exceptions</see>
    /// - <see href="https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // TODO: Handle all exceptions. Use Shell32-Vanara Dialog to Display
        Debug.Indent();
        Debug.Print("Software Failure. Press left mouse button to continue.");
        Debug.Print($"*** Guru Meditation caused by {sender} ***");
        Debug.Print(e.Message);
        Debug.Unindent();
        Debug.Flush();

        e.Handled = true;
    }

    /// <summary>
    /// GetService
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static T GetService<T>()
        where T : class
    {
        if ((Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }
}
