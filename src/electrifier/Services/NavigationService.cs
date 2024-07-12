using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.WinUI.UI.Animations;
using electrifier.Contracts.Services;
using electrifier.Contracts.ViewModels;
using electrifier.Helpers;
using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace electrifier.Services;

// For more information on navigation between pages see
// https://github.com/microsoft/TemplateStudio/blob/main/docs/WinUI/navigation.md
public class NavigationService : INavigationService
{
    private readonly IPageService _pageService;
    private object? _lastParameterUsed;
    private Frame? _frame;

    [MemberNotNullWhen(true, nameof(Frame), nameof(_frame))]
    public bool CanGoBack => Frame != null && Frame.CanGoBack;

    [MemberNotNullWhen(true, nameof(Frame), nameof(_frame))]
    public bool CanGoForward => Frame != null && Frame.CanGoForward;

    public Frame? Frame
    {
        get
        {
            if (_frame == null)
            {
                _frame = App.MainWindow.Content as Frame;
                RegisterFrameEvents();
            }

            return _frame;
        }

        set
        {
            UnregisterFrameEvents();
            _frame = value;
            RegisterFrameEvents();
        }

    }

    public event NavigatedEventHandler? Navigated;

    public NavigationService(IPageService pageService)
    {
        _pageService = pageService;
    }


    public bool GoBack()
    {
        if (_frame != null)
        {
            if (CanGoBack)
            {
                var vmBeforeNavigation = _frame.GetPageViewModel();
                _frame.GoBack();
                if (vmBeforeNavigation is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }

                return true;
            }
        }

        return false;
    }

    public bool GoForward()
    {
        if (_frame != null)
        {

            if (CanGoForward)
            {
                var vmBeforeNavigation = _frame.GetPageViewModel();
                _frame.GoForward();
                if (vmBeforeNavigation is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Navigates to the page with the given key.
    /// </summary>
    /// <param name="pageKey"></param>
    /// <param name="parameter"></param>
    /// <param name="clearNavigation"></param>
    /// <returns>bool Navigation has been successful.</returns>
    public bool NavigateTo(string pageKey, object? parameter = null, bool clearNavigation = false)
    {
        var pageType = _pageService.GetPageType(pageKey);

        if (_frame != null && (_frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(_lastParameterUsed))))
        {
            _frame.Tag = clearNavigation;
            var vmBeforeNavigation = _frame.GetPageViewModel();
            var navigated = _frame.Navigate(pageType, parameter);
            if (navigated)
            {
                _lastParameterUsed = parameter;
                if (vmBeforeNavigation is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }
            }

            return navigated;
        }

        return false;
    }

    public bool NavigateToWorkbench()
    {
        var viewModel = App.GetService<WorkbenchViewModel>();
        var fullName = viewModel.GetType().FullName;

        if (fullName is not null)
        {
            return NavigateTo(fullName);
        }
        return false;
    }
    public void SetListDataItemForNextConnectedAnimation(object item)
    {
        Frame.SetListDataItemForNextConnectedAnimation(item);
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (sender is Frame frame)
        {
            var clearNavigation = (bool)frame.Tag;
            if (clearNavigation)
            {
                frame.BackStack.Clear();
            }

            if (frame.GetPageViewModel() is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(e.Parameter);
            }

            Navigated?.Invoke(sender, e);
        }
    }

    private void RegisterFrameEvents()
    {
        if (_frame != null)
        {
            _frame.Navigated += OnNavigated;
        }
    }

    private void UnregisterFrameEvents()
    {
        if (_frame != null)
        {
            _frame.Navigated -= OnNavigated;
        }
    }
}
