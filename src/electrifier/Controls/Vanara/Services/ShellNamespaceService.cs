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
    private Task? _stockIconTask;
    public ShellNamespaceService()
    {
        IconExtractor = new(ShellFolder.Desktop);
        IconExtractorBitmaps = IconExtractor.ImageList;
        _stockIconTask = InitializeStockIcons();
    }

    // TODO: Add await event handler to every ebItem, so Icon Extractor can call back the item
    public static Task<ShellDataTable> RequestChildItemsAsync(ShellFolder shFolder,
        FolderItemFilter itemFilter = (FolderItemFilter.Folders | FolderItemFilter.NonFolders),
        EventHandler? allFastRowsAddedHandler = null, EventHandler? tableLoadedHandler = null)
    {
        Debug.Print($".RequestChildItemsAsync(<{shFolder}>) extracting...");
        var ct = new CancellationToken();

        var propKeys = new List<Ole32.PROPERTYKEY>()
        { Ole32.PROPERTYKEY.System.FileFRN, /* This is the unique file ID, also known as the File Reference Number. */ };

        var shDataTable = new ShellDataTable(shFolder, itemFilter);
        shDataTable.AllFastRowsAdded += ShDataTable_AllFastRowsAdded;
        if (allFastRowsAddedHandler != null) shDataTable.AllFastRowsAdded += allFastRowsAddedHandler;
        shDataTable.TableLoaded += ShDataTableOnTableLoaded;
        if (tableLoadedHandler != null) shDataTable.TableLoaded += tableLoadedHandler;
        var populationTask = shDataTable.PopulateTableAsync(propKeys, ct);

        return Task.FromResult(shDataTable);



    //var ct = new CancellationToken();

    //if (parentItem is null)
    //{
    //    yield break;
    //}

    ///*  public bool Wait(TimeSpan timeout) {
    //        long totalMilliseconds = (long)timeout.TotalMilliseconds;
    //        if (totalMilliseconds < -1 || totalMilliseconds > int.MaxValue)
    //            { throw new ArgumentOutOfRangeException(nameof(timeout)); }

    //        return Wait((int)totalMilliseconds, new CancellationToken());
    //    } */

    //try
    //{
    //    /* var ext = new ShellIconExtractor(new ShellFolder(parentItem.ShellItem));
    //       ext.Complete += ShellIconExtractorComplete;
    //       ext.IconExtracted += ShellIconExtractorIconExtracted;
    //       ext.Start(); */
    //    Debug.Assert(parentItem.ShellItem.PIDL != Shell32.PIDL.Null);
    //    var shItemId = parentItem.ShellItem.PIDL;
    //    using var shFolder = new ShellFolder(shItemId);

    //    if ((shFolder.Attributes & ShellItemAttribute.Removable) != 0)
    //    {
    //        // TODO: Check for Disc in Drive, fail only if device not present
    //        // TODO: Add `Eject-Buttons` to TreeView (right side, instead of TODO: Pin header) and GridView
    //        Debug.WriteLine($"GetChildItems: IsRemovable = true");
    //        var eventArgs = new NavigationFailedEventArgs();
    //        yield break;
    //    }

    //    var ext = new TempShellIconExtractor(new ShellFolder(parentItem.ShellItem));

    //    ext.Complete += new((sender, args) =>
    //    {
    //        Debug.Assert(sender.Equals(ext));
    //        Debug.WriteLine($".IAsyncEnumerable<ExplorerBrowserItem> RequestChildItemsAsync(ExplorerBrowserItem? parentItem) completed.");
    //    });
    //    ext.IconExtracted += new((sender, args) =>
    //    {
    //        var shItem = args.ItemID;
    //        var idx = args.ImageListIndex;
    //        Debug.Assert(sender.Equals(ext));
    //        //Yield Awaitable return new ExplorerBrowserItem(args.ItemID, args.ImageListIndex);
    //    });
    //    ext.Start();

    //    // DispatcherTimer
    //}
    //catch (Exception e)
    //{
    //    Console.WriteLine(e);
    //    throw;
    //}
    }

    private static void ShDataTable_AllFastRowsAdded(object? sender, EventArgs e)
    {
        Debug.Print($".ShDataTable_AllFastRowsAdded({sender})...");
    }
    private static void ShDataTableOnTableLoaded(object? sender, EventArgs e)
    {
        Debug.Print($".ShDataTableOnTableLoaded({sender})...");
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

    /// <summary>
    /// ExtractChildItems
    /// TODO: Add Stack, or ShellDataTable
    /// TODO: Pre-Enumerate slow folders while building the tree
    /// </summary>
    public static List<ExplorerBrowserItem> ExtractChildItems(ExplorerBrowserItem parentBrowserItem,
        FolderItemFilter itemFilter = (FolderItemFilter.Folders | FolderItemFilter.NonFolders))
    {
        var shItem = parentBrowserItem.ShellItem;
        var result = new List<ExplorerBrowserItem>();

        if ((shItem.Attributes & ShellItemAttribute.Removable) != 0)
        {
            // TODO: Check for Disc in Drive, fail only if device not present
            // TODO: Add `Eject-Buttons` to TreeView (right side, instead of TODO: Pin header) and GridView
            Debug.WriteLine($"GetChildItems: IsRemovable = true");
            return result;
            //var eventArgs = new NavigationFailedEventArgs();
            //return Task.FromCanceled<>();
            //cancelToken.ThrowIfCancellationRequested(); 
        }

        if (!shItem.IsFolder)
        {
            return result;
        }

        try
        {
            using var shFolder = new ShellFolder(shItem);
            var children = shFolder.EnumerateChildren(itemFilter);
            var shellItems = children as ShellItem[] ?? children.ToArray();
            var cnt = shellItems.Length;

            if (cnt > 0)
            {
                foreach (var item in shellItems)
                {
                    var ebItem = new ExplorerBrowserItem(item.PIDL);
                    if (ebItem.IsFolder)
                    {
                        ebItem.BitmapSource = ShellNamespaceService.DefaultFolderImageBitmapSource;
                        result.Insert(0, ebItem);
                    }
                    else
                    {
                        ebItem.BitmapSource = ShellNamespaceService.DefaultDocumentAssocImageBitmapSource;
                        result.Add(ebItem);
                    }
                }
            }
        }
        catch (COMException comEx)
        {
            Debug.Fail(comEx.Message);
        }
        catch (Exception e)
        {
            Debug.Fail(e.Message);
        }

        return result;
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
