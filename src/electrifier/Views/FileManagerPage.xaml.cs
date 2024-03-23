using CommunityToolkit.WinUI.Collections;
using CommunityToolkit.WinUI.UI;
using electrifier.Services;
using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using Windows.ApplicationModel;
using Windows.Storage.Search;
using Windows.Storage;

namespace electrifier.Views;

public sealed partial class FileManagerPage : Page
{
    #region ContentAreaBottomAppBar

    public uint FolderCount
    {
        get; private set;
    }

    public string FolderCountText => FolderCount.ToString() + ", 7 hidden";

    public uint FileCount
    {
        get; private set;
    }
    public string FileCountText => FileCount.ToString() + ", 6 hidden";

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

        ShellTreeView.ItemsSource = ViewModel.ShellTreeViewItems;
        ShellGridView.ItemsSource = ViewModel.ShellGridCollectionViewItems;
    }
}
