using electrifier.Controls.Vanara;
using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Vanara.Windows.Shell;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using electrifier.Contracts.Services;
using electrifier.Helpers;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using Windows.ApplicationModel;
using CommunityToolkit.WinUI;


namespace electrifier.Views;

public sealed partial class FileManagerPage : Page
{
    #region ContentAreaBottomAppBar

    public int ItemCount
    {
        get => ViewModel.ItemCount;
        set => throw new NotImplementedException();
    }

    public int FolderCount
    {
        get => ViewModel.FolderCount;
        set => throw new NotImplementedException();
    }

    public bool HasFolders
    {
        get => FolderCount > 0;
        set => throw new NotImplementedException();
    }

    public int FileCount
    {
        get => ViewModel.FileCount;
        set => throw new NotImplementedException();
    }

    public bool HasFiles
    {
        get => FileCount > 0;
        set => throw new NotImplementedException();
    }

    #endregion ContentAreaBottomAppBar

    public FileManagerViewModel ViewModel
    {
        get;
        set;
    }

    public Shell32TreeView TreeView;

    public ShellItem CurrentFolder;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileManagerPage"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public FileManagerPage()
    {
        ViewModel = App.GetService<FileManagerViewModel>() ?? throw new InvalidOperationException();
        InitializeComponent();
        DataContext = this;
        CurrentFolder = ShellFolder.Desktop;

        //var contentAreaGridView = this.Content;
        //if(ContentAreaGrid is contents)
        //var children = contentAreaGridView.Children;

        //var children = contentAreaGridView.Children;
        //foreach (var child in children)
        //{
        //    if (child is Shell32GridView gridView)
        //    {
        //        GridView = gridView;
        //        break;
        //    }
        //}

        //Shell32GridView = null;  //new Shell32GridView();
        //var child = this;// this.Content.Children();

        //if (child is not null)
        //{
        //    //this.GridView = ;
        //    //child.Navigate(ShellFolder.Desktop);
        //}

    }

    private void ShellTreeView_OnOnSelectionChanged(object? sender, TreeViewSelectionChangedEventArgs e)
    {

        var selectedItems = e.AddedItems;
        //var newSelection? = default;

        foreach (var item in selectedItems)
        {
            if (item is Shell32TreeViewItem treeViewItem)
            {
                var targetShellItem = treeViewItem.ShellItem;

                if(targetShellItem is null)
                {
                    continue;
                }
                CurrentFolder = targetShellItem;

                // Shell32GridView gridView = default;
                // gridView.Navigate(shTreeViewItem.ShellItem);
                // NavigatedEventArgs(ContentAreaBottomAppBar)
                // ViewModel.CurrentFolder = shTreeViewItem.ShellItem;
            }
        }
    }
}
