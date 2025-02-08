using Windows.UI.ViewManagement;
using electrifier.Helpers;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using WinUIEx;

namespace electrifier;


public sealed partial class MainWindow : WindowEx
{
    private readonly AppWindow _mAppWindow;
    private readonly AppWindowTitleBar _mAppTitleBar;

    private readonly DispatcherQueue _mDispatcherQueue;

    private readonly UISettings _mSettings;

    /// <summary>
    /// More information how to customize the https://learn.microsoft.com/en-us/windows/apps/develop/title-bar
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        _mAppWindow = GetAppWindowForCurrentWindow();
        _mAppTitleBar = _mAppWindow.TitleBar;
        // Hide system title bar.
        _mAppTitleBar.ExtendsContentIntoTitleBar = true;

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/app.ico"));
        Content = null;
        Title = "electrifier";

        _mAppWindow.Changed += AppWindow_Changed;

        // Theme change code picked from https://github.com/microsoft/WinUI-Gallery/pull/1239
        _mDispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _mSettings = new UISettings();
        // cannot use FrameworkElement.ActualThemeChanged event
        _mSettings.ColorValuesChanged += Settings_ColorValuesChanged;
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        var hWnd = WindowNative.GetWindowHandle(this);
        var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);

        return AppWindow.GetFromWindowId(wndId);
    }

    private static void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (args.DidPresenterChange)
        {
            switch (sender.Presenter.Kind)
            {
                case AppWindowPresenterKind.CompactOverlay:
                    // Compact overlay - hide custom title bar
                    // and use the default system title bar instead.
                    //AppTitleBar.Visibility = Visibility.Collapsed;
                    //sender.TitleBar.ResetToDefault();
                    break;

                case AppWindowPresenterKind.FullScreen:
                    // Full screen - hide the custom title bar
                    // and the default system title bar.
                    //AppTitleBar.Visibility = Visibility.Collapsed;
                    //sender.TitleBar.ExtendsContentIntoTitleBar = true;
                    break;

                case AppWindowPresenterKind.Overlapped:
                    // Normal - hide the system title bar
                    // and use the custom title bar instead.
                    //AppTitleBar.Visibility = Visibility.Visible;
                    //sender.TitleBar.ExtendsContentIntoTitleBar = true;
                    break;

                default:
                    // Use the default system title bar.
                    sender.TitleBar.ResetToDefault();
                    break;
            }
        }
    }

    // this handles updating the caption button colors correctly
    // when windows system theme is changed while the app is open
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        // This calls comes off-thread, hence we will need to dispatch it to current app's thread
        _mDispatcherQueue.TryEnqueue(TitleBarHelper.ApplySystemThemeToCaptionButtons);
    }
}
