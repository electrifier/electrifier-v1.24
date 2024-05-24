using CommunityToolkit.Mvvm.ComponentModel;

namespace electrifier.ViewModels;

public partial class FileManagerViewModel : ObservableRecipient
{
    private readonly int itemCount;
    private readonly int folderCount;
    private readonly int fileCount;

    /// <summary>Number of Files</summary>
    public int FileCount => fileCount;
    /// <summary>Number of Folders</summary>
    public int FolderCount => folderCount;
    /// <summary>Count of Items</summary>
    public int ItemCount => itemCount;

    /// <summary>FileManagerViewModel</summary>
    public FileManagerViewModel()
    {

    }
}
