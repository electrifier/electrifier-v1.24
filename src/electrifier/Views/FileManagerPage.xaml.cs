using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace electrifier.Views;

public sealed partial class FileManagerPage : Page
{
    #region ContentAreaBottomAppBar

    public int ItemCount => ViewModel.ItemCount;

    public int FolderCount => ViewModel.FolderCount;

    public bool HasFolders => FolderCount > 0;

    public int FileCount => ViewModel.FileCount;
    public bool HasFiles => FileCount > 0;

    #endregion

    public FileManagerViewModel ViewModel
    {
        get;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileManagerPage"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public FileManagerPage()
    {
        ViewModel = App.GetService<FileManagerViewModel>() ?? throw new InvalidOperationException();

        InitializeComponent();
    }
}
