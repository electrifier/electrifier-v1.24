using Vanara.Windows.Shell;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Drawing;
using System.Diagnostics;
using static Vanara.PInvoke.Shell32;
using Microsoft.UI.Xaml.Media.Imaging;
using JetBrains.Annotations;

namespace electrifier.Controls.Vanara.Services;

public partial class ShellNamespaceService : IShellNamespaceService
{
    /// <summary>
    /// <see cref="HResult">HResult</see> code of <code><see cref="COMException"/>('0x80070490');</code>
    /// <remarks>Fired when <b>`Element not found`</b> while enumerating the Shell32 Namespace.</remarks>
    /// </summary>
    public static readonly HRESULT HResultElementNotFound = new(0x80070490);
    /// <summary>
    /// <see cref="ShellFolder"/> of virtual `<b>Home</b>` directory.
    /// <remarks>This equals Shell 32 URI: <code>shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}</code></remarks>
    /// </summary>
    public static ShellFolder HomeShellFolder => new("shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}");
    internal static readonly TempShellIconExtractor IconExtractor = new(ShellFolder.Desktop);
    public static IReadOnlyList<Bitmap> IconExtractorBitmaps => IconExtractor.ImageList;
    public int IconSize => IconExtractor.ImageSize;
    /// <summary>
    /// todo: die ganzen StockIcons in ein struct packen.
    /// Indexer ist `Shell32.SHSTOCKICONID`
    /// get-Methode, die erst die Icons holt wenn danach gefragt wird.
    /// </summary>
    internal static StockIcon SiDocument = new(Shell32.SHSTOCKICONID.SIID_DOCNOASSOC);
    internal static SoftwareBitmapSource DocumentBitmapSource = new();
    internal static StockIcon SiDocumentWithAssociation = new(SHSTOCKICONID.SIID_DOCASSOC);
    internal static SoftwareBitmapSource DocumentWithAssociationBitmapSource = new();
    internal static StockIcon SiFolder = new(Shell32.SHSTOCKICONID.SIID_FOLDER);
    internal static SoftwareBitmapSource FolderBitmapSource = new();
    internal static StockIcon SiFolderBack = new(Shell32.SHSTOCKICONID.SIID_FOLDERBACK);
    internal static StockIcon SiFolderFront = new(Shell32.SHSTOCKICONID.SIID_FOLDERFRONT);
    internal static StockIcon SiFolderOpen = new(Shell32.SHSTOCKICONID.SIID_FOLDEROPEN);
    internal static StockIcon SiLinkOverlay = new(SHSTOCKICONID.SIID_LINK);
    protected Task? StockIconTask;

    /// <summary>
    /// <remarks>When used as service, this static might be unnecessary.
    /// However, this makes sure the Stock Icon cache gets initialized as soon as possible.</remarks>
    /// </summary>
    [UsedImplicitly]
    public static readonly ShellNamespaceService Instance = new();

    /// <summary>ShellNamespaceService() Warn: Actually does not really conform Service Models.</summary>
    public ShellNamespaceService()
    {
        StockIconTask = InitializeStockIconsAsync();
        return;
    }

    /// <summary>Initialize default <see cref="StockIcon">Stock Icons</see>.</summary>
    /// <remarks>TODO: INFO: Investigate <seealso href="https://github.com/dahall/Vanara/blob/Windows.Shell.Common/StockIcon.cs"></seealso></remarks>
    /// <returns></returns>
    private static async Task InitializeStockIconsAsync()
    {
        /* Todo: inspect `SHGetStockIconInfo()` */
        //var siFlags = SHGSI.SHGSI_LARGEICON | SHGSI.SHGSI_ICON;
        //var siStockIconInfo = new SHSTOCKICONINFO();
        //SHGetStockIconInfo(Shell32.SHSTOCKICONID.SIID_APPLICATION, siFlags, ref siStockIconInfo).ThrowIfFailed();

        try
        {
            using var siFolder = new StockIcon(Shell32.SHSTOCKICONID.SIID_FOLDER);
            {
                var idx = siFolder.SystemImageIndex;
                var icnHandle = siFolder.IconHandle.ToIcon();
                var bmpSource = GetWinUi3BitmapSourceFromIcon(icnHandle);
                ShellNamespaceService.FolderBitmapSource = await bmpSource;
            }

            using var siDocument = new StockIcon(Shell32.SHSTOCKICONID.SIID_DOCASSOC);
            {
                var idx = siDocument.SystemImageIndex;
                var icnHandle = siDocument.IconHandle.ToIcon();
                var bmpSource = GetWinUi3BitmapSourceFromIcon(icnHandle);
                ShellNamespaceService.DocumentBitmapSource = await bmpSource;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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

    /// <summary>Get associated <seealso cref="SoftwareBitmapSource"/> for given <param name="bitmapIcon">Icon</param></summary>
    /// <remarks>TODO: INFO: Investigate <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.imaging.writeablebitmap?view=winrt-26100">uwp/api/windows.ui.xaml.media.imaging.WriteableBitmap (WARN: Links to UWP)</seealso></remarks>
    /// <param name="bitmapIcon">The <seealso cref="Icon">Icon</seealso>.</param>
    /// <returns>Task&lt;SoftwareBitmapSource?&gt;</returns>
    public static async Task<SoftwareBitmapSource?> GetWinUi3BitmapSourceFromIcon(Icon bitmapIcon)
    {
        ArgumentNullException.ThrowIfNull(bitmapIcon);

        return await GetWinUi3BitmapSourceFromGdiBitmap(bitmapIcon.ToBitmap());
    }

    /// <summary>Get associated <seealso cref="SoftwareBitmapSource"/> for given <param name="gdiBitmap">gdiBitmap</param></summary>
    /// <remarks>TODO: INFO: Investigate <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.imaging.writeablebitmap?view=winrt-26100">uwp/api/windows.ui.xaml.media.imaging.WriteableBitmap (WARN: Links to UWP)</seealso></remarks>
    /// <param name="gdiBitmap">The <seealso cref="Bitmap">GDI+ bitmap</seealso>.</param>
    /// <returns>Task&lt;SoftwareBitmapSource?&gt;</returns>
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
