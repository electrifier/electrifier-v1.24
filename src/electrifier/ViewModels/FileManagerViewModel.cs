using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Vanara.Windows.Shell;

namespace electrifier.ViewModels;

public partial class FileManagerViewModel : ObservableRecipient
{
    public ShellItem? CurrentFolder
    {
        get => _currentFolder;
        set => SetCurrentFolder(value);
    }

    private readonly ShellItem? _currentFolder = default;

    private void SetCurrentFolder(ShellItem? value)
    {
        var item = value;

        if (item is null)
        {
            Debug.Print("{nameof(this)}.SetCurrentFolder: value is null");
            return;
        }

        //GridView.CurrentFolder = value;
        ////GridView.Navigate

        return;
    }

    /// <summary>FileManagerViewModel</summary>
    public FileManagerViewModel()
    {
        // TODO: this.ShellGridView = this.ShellGridView;

        // TODO: refactor getting child items
        //var Shell32TreeView = this.FindName("ShellTreeView");
        //var Shell32GridView = this.FindName("ShellGridView");

    }

    #region obsolete code
    //public Shell32TreeView ShellTreeView 
    //{
    //    get;
    //}
    //public Shell32GridView ShellGridView 
    //{
    //    get;
    //}

    //private void TreeView_OnItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    //{
    //    var item = args.InvokedItem as Shell32TreeViewItem;
    //    if (item is null)
    //    {
    //        return;
    //    }
    //    if (item.ShellItem is ShellItem shItem)
    //    {
    //        this.CurrentFolder = shItem;
    //    }
    //    //var selectedItem = args.AddedItems.FirstOrDefault();
    //    //if (selectedItem is Shell32TreeViewItem shellItem)
    //    //{
    //    //    OnSelectionChanged?.Invoke(this, args);
    //    //    //throw new NotImplementedException();
    //    //}
    //}
    #endregion
}
