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
    // primary properties
    public string DisplayName
    {
        get;
    }
    public ShellItem ShellItem
    {
        get;
    }
    public List<ExplorerBrowserItem> Children;

    public bool IsFolder
    {
        get;
    }
    public bool IsLibrary
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
    //private bool IsEnumerated
    //{
    //    get; set;
    //}
    public bool IsExpanded
    {
        get; set;
    }
    public bool IsLink
    {
        get;
    }
    public bool IsSelected
    {
        get; set;
    }

    // TODO: TreeViewNode - Property
    // TODO: GridViewItem - Property
    // TODO: ExplorerBrowserItem.TreeNodeSelected = bool; => Initiate selection of this node
    public ExplorerBrowserItem(ShellItem shItem)
    {
        ShellItem = shItem ?? throw new ArgumentNullException(nameof(shItem));
        DisplayName = ShellItem.Name ?? throw new Exception("shItem Display Name");
        Children = [];
        // INFO: Removed IsEnumerated = false;
        IsFolder = shItem.IsFolder;
        IsLink = shItem.IsLink;
        //var shLib = ShellLibrary.Open(shItem.PIDL);
        //if (shLib != null)
        //{
        //    IsLibrary = true;
        //    shLib.Dispose();
        //}


        // TODO: Library default image (DefaultLibraryImage)
        ImageIconSource = shItem is { IsFolder: true } ? ExplorerBrowser.DefaultFolderImage : ExplorerBrowser.DefaultFileImage;

        // dummy values for testing
        IsExpanded = false;
        HasUnrealizedChildren = (shItem.Attributes.HasFlag(ShellItemAttribute.HasSubfolder));
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
