using electrifier.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace electrifier.Views;

// To learn more about WebView2, see https://docs.microsoft.com/microsoft-edge/webview2/.
public sealed partial class Microsoft365Page : Page
{
    public Microsoft365ViewModel ViewModel
    {
        get;
    }

    public Microsoft365Page()
    {
        ViewModel = App.GetService<Microsoft365ViewModel>();
        InitializeComponent();

        ViewModel.WebViewService.Initialize(WebView);
    }
}
