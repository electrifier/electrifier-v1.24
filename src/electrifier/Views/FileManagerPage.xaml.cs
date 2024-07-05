using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Vanara.Windows.Shell;

namespace electrifier.Views;

public sealed partial class FileManagerPage : Page
{
    public FileManagerViewModel ViewModel { get;  set; }

    // primary member
    public ShellItem CurrentFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileManagerPage"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public FileManagerPage()
    {
        ViewModel = App.GetService<FileManagerViewModel>() ?? throw new InvalidOperationException();
        DataContext = this;
        InitializeComponent();

        CurrentFolder = ShellFolder.Desktop;
    }
}
