using System.Runtime.InteropServices;
using electrifier.Controls.Vanara.Services;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara.Contracts;

public interface IShellNamespaceService
{
    /// <summary><see cref="HRESULT"/> code of <see cref="COMException"/><i>('0x80070490');</i>
    /// <remarks>Fired when <b>`Element not found`</b> while enumerating the Shell32 Namespace.</remarks>
    /// <remarks>As far as I know, this also gets fired when <b>No Disk in Drive</b> error occurs.</remarks></summary>
    public static readonly HRESULT HResultElementNotFound = new(0x80070490);
    /// <summary><see cref="ShellFolder"/> of virtual `<b>Home</b>` directory.
    /// <remarks>This equals Shell 32 URI: <code>shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}</code></remarks></summary>
    public static ShellFolder HomeShellFolder => new("shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}");

    /// <summary>
    /// todo: die ganzen StockIcons in ein struct packen.
    /// Indexer ist `Shell32.SHSTOCKICONID`
    /// get-Methode, die erst die Icons holt wenn danach gefragt wird.
    /// </summary>
    internal static StockIcon SiDocument = new(Shell32.SHSTOCKICONID.SIID_DOCNOASSOC);
    internal static SoftwareBitmapSource DocumentBitmapSource = new();
    internal static StockIcon SiDocumentWithAssociation = new(Shell32.SHSTOCKICONID.SIID_DOCASSOC);
    internal static SoftwareBitmapSource DocumentWithAssociationBitmapSource = new();
    internal static StockIcon SiFolder = new(Shell32.SHSTOCKICONID.SIID_FOLDER);
    internal static SoftwareBitmapSource FolderBitmapSource = new();
    //internal static StockIcon SiLibrary = new(Shell32.SHSTOCKICONID.SIID_LINK);
    internal static SoftwareBitmapSource LibraryFrontBitmapSource = new();
    internal static StockIcon SiFolderBack = new(Shell32.SHSTOCKICONID.SIID_FOLDERBACK);
    internal static SoftwareBitmapSource FolderBackBitmapSource = new();
    internal static StockIcon SiFolderFront = new(Shell32.SHSTOCKICONID.SIID_FOLDERFRONT);
    internal static SoftwareBitmapSource FolderFrontBitmapSource = new();
    internal static StockIcon SiFolderOpen = new(Shell32.SHSTOCKICONID.SIID_FOLDEROPEN);
    internal static SoftwareBitmapSource FolderOpenBitmapSource = new();
    internal static StockIcon SiLinkOverlay = new(Shell32.SHSTOCKICONID.SIID_LINK);
    internal static SoftwareBitmapSource LinkBitmapSource = new();
    internal static StockIcon SiError = new(Shell32.SHSTOCKICONID.SIID_ERROR);
    internal static SoftwareBitmapSource ErrorBitmapSource = new();

    public List<ShellNamespaceService.BrowserStockIcon> StockIcons { get; init; }
}