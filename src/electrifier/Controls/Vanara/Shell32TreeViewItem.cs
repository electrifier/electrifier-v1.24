using System.Collections.ObjectModel;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

public class Shell32TreeViewItem
{
    public ObservableCollection<Shell32TreeViewItem> Children
    {
        get;
    }
    public string DisplayName
    {
        get;
    }

    private bool IsEnumerated
    {
        get; set;
    }

    internal IEnumerable<ShellItem> EnumerateChildren(FolderItemFilter filter)
    {
        try
        {
            return ShellItem is not ShellFolder folder
                ? Enumerable.Empty<ShellItem>()
                : folder.EnumerateChildren(filter);
        }
        finally
        {
            IsEnumerated = true;
            HasUnrealizedChildren = false;
        }
    }

    // TODO: Add observable flags for async property loading
    public bool HasUnrealizedChildren
    {
        get;
        private set;
    }
    public ShellItemImages Images => ShellItem.Images; // GetImageAsync, GetImage 
    public ShellItem ShellItem
    {
        get;
    }
    
    public Shell32TreeViewItem(ShellItem shItem)
    {
        ShellItem = shItem ?? throw new ArgumentNullException(nameof(shItem));
        DisplayName = ShellItem.Name ?? ShellItem.ToString();
        Children = new ObservableCollection<Shell32TreeViewItem>();
        HasUnrealizedChildren = true;

        _ = Task.Run(InitializeAsync);
    }

    public async Task InitializeAsync()
    {
        var attributes = await Task.Run(() => ShellItem.Attributes);
        var StorageCapMask = await Task.Run(() => attributes & ShellItemAttribute.StorageCapMask);

        HasUnrealizedChildren = attributes.HasFlag(ShellItemAttribute.HasSubfolder);
    }
}
