using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public class ExplorerBrowserItem
{
    // TODO: Use shell32 stock icons
    private static readonly BitmapImage DefaultFileImage =
        new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico"));

    private static readonly BitmapImage DefaultFolderImage =
        new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico"));

    private static readonly BitmapImage DefaultLibraryImage =
        new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Library.ico"));

    // primary properties
    public string DisplayName
    {
        get;
    }
    public ShellItem ShellItem
    {
        get;
    }
    public ExplorerBrowser Owner
    {
        get;
    }
    public List<ExplorerBrowserItem> Children;

    // secondary properties
    public bool IsFolder
    {
        get;
    }
    public bool HasUnrealizedChildren
    {
        get;
        private set;
    }
    public ImageSource ImageIconSource
    {
        get;
    }
    private bool IsEnumerated
    {
        get; set;
    }

    public bool IsExpanded
    {
        get; set;
    }

    public bool IsSelected
    {
        get; set;
    }

    // TODO: TreeViewNode - Property
    // TODO: GridViewItem - Property
    // TODO: ExplorerBrowserItem.TreeNodeSelected = bool; => Initiate selection of this node
    public ExplorerBrowserItem(ExplorerBrowser owner, ShellItem shItem, string? overrideDisplayName = default)
    {
        Owner = owner;
        ShellItem = shItem ?? throw new ArgumentNullException(nameof(shItem));
        DisplayName = overrideDisplayName ?? (ShellItem.Name ?? throw new Exception("shItem Display Name"));
        Children = [];
        
        // secondary properties
        HasUnrealizedChildren = shItem.IsFolder;
        IsFolder = shItem.IsFolder;
        ImageIconSource = shItem is { IsFolder: true } ? DefaultFolderImage : DefaultFileImage;
        IsExpanded = true;
    }

    // TODO: async
    internal static IEnumerable<ShellItem> EnumerateChildren(ShellItem enumerationShellItem, FolderItemFilter filter)
    {
        return enumerationShellItem is not ShellFolder folder ? [] : folder.EnumerateChildren(filter);
    }

    // TODO: async
    public List<ExplorerBrowserItem> GetChildItems(ShellItem enumerationShellItem)
    {
        try
        {
            if ((enumerationShellItem.Attributes & ShellItemAttribute.Removable) != 0)
            {
                Debug.WriteLine($"`{GetDebuggerDisplay}` is <ShellItemAttribute.Removable>: Skipping Enumeration");
                return [];
            }

            // TODO: This takes ages on folder: @"C:\Users\tajbe\OneDrive\Desktop\aktuelle.projekte\Alte Desktop-Icons"
            var children = EnumerateChildren(enumerationShellItem, filter: FolderItemFilter.Storage);
            var childItems = new List<ExplorerBrowserItem>();

            foreach (var child in children)
            {
                var childItem = child as ShellItem;
                var ebItem = new ExplorerBrowserItem(this.Owner, childItem);

                childItems.Add(ebItem);
            }

            IsEnumerated = true; // TODO: SetProperty
            HasUnrealizedChildren = false; // TODO: SetProperty

            return childItems;
        }
        catch (COMException comException)
        {
            Debug.WriteLine($"ExplorerBrowserItem: GetChildItems() failed: {comException.Message}");
            throw;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            throw;
        }
    }

    #region GetDebuggerDisplay()
    private string GetDebuggerDisplay()
    {
        var sb = new StringBuilder();
        sb.Append($"`{DisplayName}` - <{nameof(ExplorerBrowserItem)}>");

        if (IsFolder) { sb.Append(", [folder]"); }

        return sb.ToString();
    }
    #endregion
}
