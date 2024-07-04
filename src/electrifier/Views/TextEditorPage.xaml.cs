using electrifier.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace electrifier.Views;

public sealed partial class TextEditorPage : Page
{
    public int TextEditorPageId
    {
        get;
    }

    public TextEditorViewModel ViewModel
    {
        get;
    }

    public string StatusCursorPosition => GetCursorPosition();
    private string GetCursorPosition() => ViewModel.StatusCursorPosition;

    public TextEditorPage()
    {
        ViewModel = App.GetService<TextEditorViewModel>();
        InitializeComponent();
    }

    private void CodeEditorControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Needs to set focus explicitly due to WinUI 3 regression https://github.com/microsoft/microsoft-ui-xaml/issues/8816 
        ((Control)sender).Focus(FocusState.Programmatic);
    }
}
