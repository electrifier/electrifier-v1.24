using electrifier.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Vanara.Windows.Shell;

namespace electrifier.Views;

public sealed partial class FileManagerPage : Page
{
    public FileManagerViewModel ViewModel
    {
        get; set;
    }

    public int ItemCount
    {
        get => (int)GetValue(ItemCountProperty);
        set => SetValue(ItemCountProperty, value);
    }

    // primary member
    public ShellItem CurrentFolder;
    public static readonly DependencyProperty ItemCountProperty = DependencyProperty.Register(nameof(ItemCount), typeof(int), typeof(FileManagerPage), new PropertyMetadata(default(int)));

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
        ItemCount = 666;  // todo: set real value
    }
}
