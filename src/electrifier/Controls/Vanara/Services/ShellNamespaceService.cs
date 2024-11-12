using static Vanara.PInvoke.Shell32.ShellUtil;
using static Vanara.PInvoke.Shell32;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using System.Diagnostics;
using static Vanara.PInvoke.Ole32;

namespace electrifier.Controls.Vanara.Services;

public partial class ShellNamespaceService
{
    /// <summary>HResult code of <code><see cref="COMException"/>('0x80070490')</code>
    /// <remarks>Fired when `Element not found`.</remarks></summary>
    public static readonly HRESULT HResultElementNotFound = new(0x80070490);
    /// <summary><see cref="ShellFolder"/> of virtual `Home` directory.
    /// <remarks>This equals Shell 32 URI: <code>shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}</code></remarks></summary>
    public static readonly ShellFolder HomeShellFolder = new("shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}");
    internal readonly TempShellIconExtractor IconExtractor;
    public IReadOnlyList<Bitmap> IconExtractorBitmaps;
    public int IconSize => IconExtractor.ImageSize;
    internal static SoftwareBitmapSource DefaultFolderImageBitmapSource;
    internal static SoftwareBitmapSource DefaultDocumentAssocImageBitmapSource;
    /// <summary>
    /// todo: die ganzen StockIcons in ein struct packen.
    /// Indexer ist `Shell32.SHSTOCKICONID`
    /// get-Methode, die erst die Icons holt wenn danach gefragt wird.
    /// </summary>
    internal StockIcon SiFolder = new(Shell32.SHSTOCKICONID.SIID_FOLDER);
    internal StockIcon SiDocument = new(Shell32.SHSTOCKICONID.SIID_DOCNOASSOC);
    internal StockIcon SiFolderOpen = new(Shell32.SHSTOCKICONID.SIID_FOLDEROPEN);  // overlay: SIID_FOLDERFRONT, SIID_FOLDERBACK
    internal StockIcon SiDocumentWithAssociation = new(SHSTOCKICONID.SIID_DOCASSOC);
    internal StockIcon SiLinkOverlay = new(SHSTOCKICONID.SIID_LINK);
    public Task? StockIconTask;
    public ShellNamespaceService()
    {
        IconExtractor = new(ShellFolder.Desktop);
        IconExtractorBitmaps = IconExtractor.ImageList;
        StockIconTask = InitializeStockIcons();
    }

    // TODO: Add await event handler to every ebItem, so Icon Extractor can call back the item
    public static async Task<ShellDataTable> RequestChildItemsAsync(ShellFolder shFolder,
        FolderItemFilter itemFilter = (FolderItemFilter.Folders | FolderItemFilter.NonFolders),
        EventHandler? allFastRowsAddedHandler = null, EventHandler? tableLoadedHandler = null)
    {
        Debug.Print($".RequestChildItemsAsync(<{shFolder}>) extracting...");
        var ct = new CancellationToken();

        var propKeys = new List<Ole32.PROPERTYKEY>()
        {
            Ole32.PROPERTYKEY.System.FileFRN, /* This is the unique file ID, also known as the File Reference Number. */
        };

        var shDataTable = new ShellDataTable(shFolder, itemFilter);
        shDataTable.AllFastRowsAdded += allFastRowsAddedHandler;
        shDataTable.TableLoaded += tableLoadedHandler;
        await shDataTable.PopulateTableAsync(propKeys, ct);

        Debug.Print($".RequestChildItemsAsync(<{shFolder}>): {shDataTable.Rows.Count}");

        return shDataTable;
    }

    /// <summary>
    /// <see href="https://github.com/dahall/Vanara/blob/Windows.Shell.Common/StockIcon.cs"/>
    /// </summary>
    /// <returns></returns>
    public static async Task InitializeStockIcons()
    {
        try
        {
            using var siFolder = new StockIcon(Shell32.SHSTOCKICONID.SIID_FOLDER);
            {
                var idx = siFolder.SystemImageIndex;
                var icnHandle = siFolder.IconHandle.ToIcon();
                var bmpSource = GetWinUi3BitmapSourceFromIcon(icnHandle);
                DefaultFolderImageBitmapSource = await bmpSource;
            }

            using var siDocument = new StockIcon(Shell32.SHSTOCKICONID.SIID_DOCNOASSOC);
            {
                var idx = siDocument.SystemImageIndex;
                var icnHandle = siDocument.IconHandle.ToIcon();
                var bmpSource = GetWinUi3BitmapSourceFromIcon(icnHandle);
                DefaultDocumentAssocImageBitmapSource = await bmpSource;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task<SoftwareBitmapSource?> GetWinUi3BitmapSourceFromIcon(Icon bitmapIcon)
    {
        ArgumentNullException.ThrowIfNull(bitmapIcon);

        return await GetWinUi3BitmapSourceFromGdiBitmap(bitmapIcon.ToBitmap());
    }
    public static async Task<SoftwareBitmapSource?> GetWinUi3BitmapSourceFromGdiBitmap(Bitmap gdiBitmap)
    {
        ArgumentNullException.ThrowIfNull(gdiBitmap);

        // get pixels as an array of bytes
        // TODO: See in vanara IconExtractor in terms of getting byte data array
        var data = gdiBitmap.LockBits(new Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, gdiBitmap.PixelFormat);
        var bytes = new byte[data.Stride * data.Height];
        Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
        gdiBitmap.UnlockBits(data);

        // get WinRT SoftwareBitmap
        var softwareBitmap = new Windows.Graphics.Imaging.SoftwareBitmap(
            Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8,
            gdiBitmap.Width,
            gdiBitmap.Height,
            Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied);
        softwareBitmap.CopyFromBuffer(bytes.AsBuffer());

        // build WinUI3 SoftwareBitmapSource
        var source = new SoftwareBitmapSource();
        await source.SetBitmapAsync(softwareBitmap);
        return source;
    }
}
