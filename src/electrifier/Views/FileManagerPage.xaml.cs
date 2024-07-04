using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Vanara.Windows.Shell;


namespace electrifier.Views;

public sealed partial class FileManagerPage : Page
{
    public FileManagerViewModel ViewModel
    {
        get;
        set;
    }

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

    private void ShellTreeView_OnOnSelectionChanged(object? sender, TreeViewSelectionChangedEventArgs e)
    {
        //var selectedItems = e.AddedItems;
        ////var newSelection? = default;

        //foreach (var item in selectedItems)
        //{
        //    if (item is Shell32TreeViewItem treeViewItem)
        //    {
        //        var targetShellItem = treeViewItem.ShellItem;

        //        if(targetShellItem is null)
        //        {
        //            continue;
        //        }
        //        CurrentFolder = targetShellItem;
        //    }
        //}
    }
}
