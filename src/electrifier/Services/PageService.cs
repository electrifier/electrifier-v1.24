using CommunityToolkit.Mvvm.ComponentModel;

using electrifier.Contracts.Services;
using electrifier.ViewModels;
using electrifier.Views;

using Microsoft.UI.Xaml.Controls;

namespace electrifier.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = [];

    public PageService()
    {
        Configure<WorkbenchViewModel, WorkbenchPage>();
        Configure<WebViewViewModel, WebViewPage>();
        Configure<SettingsViewModel, SettingsPage>();
        Configure<FileManagerViewModel, FileManagerPage>();
        Configure<TextEditorViewModel, TextEditorPage>();
        Configure<KanbanBoardViewModel, KanbanBoardPage>();
        Configure<KanbanBoardDetailViewModel, KanbanBoardDetailPage>();
        Configure<Microsoft365ViewModel, Microsoft365Page>();
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    private void Configure<TVm, TV>()
        where TVm : ObservableObject
        where TV : Page
    {
        lock (_pages)
        {
            var key = typeof(TVm).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(TV);
            if (_pages.ContainsValue(type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}
