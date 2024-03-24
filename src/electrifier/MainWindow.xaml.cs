﻿using electrifier.Helpers;

using Microsoft.UI;             // Needed for WindowId.
using Microsoft.UI.Dispatching; // Needed for DispatcherQueue.
using Microsoft.UI.Windowing;   // Needed for AppWindow.
using Windows.UI.ViewManagement;
using WinRT.Interop;            // Needed for XAML/HWND interop.

namespace electrifier;

// More information how to customize the https://learn.microsoft.com/en-us/windows/apps/develop/title-bar

public sealed partial class MainWindow : WindowEx
{
    private readonly AppWindow m_AppWindow;

    private readonly Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;

    private readonly UISettings m_Settings;

    public MainWindow()
    {
        InitializeComponent();

        m_AppWindow = GetAppWindowForCurrentWindow();
        var titleBar = m_AppWindow.TitleBar;
        // Hide system title bar.
        titleBar.ExtendsContentIntoTitleBar = true;

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "electrifier";

        // Theme change code picked from https://github.com/microsoft/WinUI-Gallery/pull/1239
        dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        m_Settings = new UISettings();
        // cannot use FrameworkElement.ActualThemeChanged event
        m_Settings.ColorValuesChanged += Settings_ColorValuesChanged;
    }

    private AppWindow GetAppWindowForCurrentWindow()
    {
        var hWnd = WindowNative.GetWindowHandle(this);
        var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);

        return AppWindow.GetFromWindowId(wndId);
    }

    // this handles updating the caption button colors correctly
    // when windows system theme is changed while the app is open
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        // This calls comes off-thread, hence we will need to dispatch it to current app's thread
        dispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        });
    }
}
