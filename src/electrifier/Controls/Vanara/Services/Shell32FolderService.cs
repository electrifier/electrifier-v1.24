using Vanara.InteropServices;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara.Services;

public static class Shell32FolderService
{
    public static ExplorerBrowserItem KnownFolderItem(Shell32.KNOWNFOLDERID folderId) => new(new ShellFolder(folderId));
}
