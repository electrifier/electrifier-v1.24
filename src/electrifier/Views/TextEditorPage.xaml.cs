using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Vanara.Windows.Shell;

namespace electrifier.Views;

public sealed partial class TextEditorPage : Page
{
    public TextEditorViewModel ViewModel
    {
        get;
    }

    public ShellItem CurrentFolder;

    public string StatusCursorPosition => GetCursorPosition();
    private string GetCursorPosition() => ViewModel.CursorPosition;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextEditorPage"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public TextEditorPage()
    {
        ViewModel = App.GetService<TextEditorViewModel>();
        DataContext = this;
        InitializeComponent();

        CurrentFolder = ShellFolder.Desktop;
    }

    //private void CodeEditorControl_Loaded(object sender, RoutedEventArgs e)
    //{
    //    // Needs to set focus explicitly due to WinUI 3 regression
    //    // https://github.com/microsoft/microsoft-ui-xaml/issues/8816 
    //    
    //    // INFO: Disabled cause of tab navigation: ((Control)sender).Focus(FocusState.Programmatic);
    //}
}
