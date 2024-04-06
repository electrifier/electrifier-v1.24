using electrifier.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace electrifier.Views;

public sealed partial class TextEditorPage : Page
{
    public TextEditorViewModel ViewModel
    {
        get;
    }

    public TextEditorPage()
    {
        ViewModel = App.GetService<TextEditorViewModel>();
        InitializeComponent();
    }
}
