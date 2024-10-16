using Vanara.InteropServices;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara.Services;

public static class Shell32FolderService
{
    /// <summary>
    /// Create <see cref="ExplorerBrowserItem"/> from <see cref="Shell32.KNOWNFOLDERID"/>
    /// </summary>
    public static ExplorerBrowserItem KnownFolderItem(Shell32.KNOWNFOLDERID folderId) => new(new ShellFolder(folderId));
    public static ExplorerBrowserItem Separator(bool visibility) => null;
}
