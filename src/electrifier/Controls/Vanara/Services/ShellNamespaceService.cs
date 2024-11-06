using static Vanara.PInvoke.Shell32.ShellUtil;
using static Vanara.PInvoke.Shell32;
using System.ComponentModel;
using System.Drawing;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using Vanara.Windows.Shell;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace electrifier.Controls.Vanara.Services;

public partial class ShellNamespaceService
{
    /// <summary>HResult code for <code><see cref="COMException"/>: 0x80070490.</code>
    /// <remarks>Fired when `Element not found`.</remarks></summary>
    public static readonly HRESULT HResultElementNotFound = new HRESULT(0x80070490);
    public static readonly ShellFolder HomeShellFolder = new("shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}");
    public IReadOnlyList<Bitmap> IconBitmaps;
    internal TempShellIconExtractor IconExtractor  = new(ShellFolder.Desktop);
    public int IconSize => IconExtractor.ImageSize;
    internal static SoftwareBitmapSource DefaultFolderImageBitmapSource;
    internal static SoftwareBitmapSource DefaultDocumentAssocImageBitmapSource;
    internal StockIcon siFolder = new StockIcon(Shell32.SHSTOCKICONID.SIID_FOLDER);
    internal StockIcon siDocument = new StockIcon(Shell32.SHSTOCKICONID.SIID_DOCNOASSOC);
    internal StockIcon siFolderOpen = new StockIcon(Shell32.SHSTOCKICONID.SIID_FOLDEROPEN);  // overlay: SIID_FOLDERFRONT, SIID_FOLDERBACK
    internal StockIcon siDocumentWithAssociation = new StockIcon(SHSTOCKICONID.SIID_DOCASSOC);
    internal StockIcon siLinkOverlay = new StockIcon(SHSTOCKICONID.SIID_LINK);
    private Task? _stockIconTask;
    public ShellNamespaceService()
    {
        IconBitmaps = IconExtractor.ImageList;
        _stockIconTask = InitializeStockIcons();
    }
    public static async IAsyncEnumerable<ExplorerBrowserItem> ExtractChildItemsAsync(ExplorerBrowserItem? parentItem)
    {
        Debug.Print($".ExtractChildItemsAsync(<{parentItem?.DisplayName}>) extracting...");

        var ct = new CancellationToken();

        if (parentItem is null)
        {
            yield break;
        }

        /*  public bool Wait(TimeSpan timeout) {
                long totalMilliseconds = (long)timeout.TotalMilliseconds;
                if (totalMilliseconds < -1 || totalMilliseconds > int.MaxValue)
                    { throw new ArgumentOutOfRangeException(nameof(timeout)); }

                return Wait((int)totalMilliseconds, new CancellationToken());
            } */

        try
        {
            /* var ext = new ShellIconExtractor(new ShellFolder(parentItem.ShellItem));
               ext.Complete += ShellIconExtractorComplete;
               ext.IconExtracted += ShellIconExtractorIconExtracted;
               ext.Start(); */
            Debug.Assert(parentItem.ShellItem.PIDL != Shell32.PIDL.Null);
            var shItemId = parentItem.ShellItem.PIDL;
            using var shFolder = new ShellFolder(shItemId);

            if ((shFolder.Attributes & ShellItemAttribute.Removable) != 0)
            {
                // TODO: Check for Disc in Drive, fail only if device not present
                // TODO: Add `Eject-Buttons` to TreeView (right side, instead of TODO: Pin header) and GridView
                Debug.WriteLine($"GetChildItems: IsRemovable = true");
                var eventArgs = new NavigationFailedEventArgs();
                yield break;
            }

            var ext = new TempShellIconExtractor(new ShellFolder(parentItem.ShellItem));

            ext.Complete += new((sender, args) =>
            {
                Debug.Assert(sender.Equals(ext));
            });
            ext.IconExtracted += new((sender, args) =>
            {
                var test = args.ItemID;
                var test2 = args.ImageListIndex;
                Debug.Assert(sender.Equals(ext));
                //YieldAwaitable return new ExplorerBrowserItem(args.ItemID, args.ImageListIndex);
            });
            ext.Start();



            // DispatcherTimer

            //var children = shFolder.EnumerateChildren(FolderItemFilter.Folders | FolderItemFilter.NonFolders);
            //var shellItems = children as ShellItem[] ?? children.ToArray();
            ////itemCount = shellItems.Length;
            //parentItem.Children = []; // TODO: new ReadOnlyDictionary<ExplorerBrowserItem, int>();

            //if (shellItems.Length > 0)
            //{
            //    foreach (var shItem in shellItems)
            //    {
            //        var ebItem = new ExplorerBrowserItem(shItem);
            //        if (ebItem.IsFolder)
            //        {
            //            ebItem.BitmapSource = _defaultFolderImageBitmapSource;
            //            parentItem.Children?.Insert(0, ebItem);
            //        }
            //        else
            //        {
            //            ebItem.BitmapSource = _defaultDocumentAssocImageBitmapSource;
            //            parentItem.Children?.Add(ebItem);
            //        }
            //    }
            //}
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    /// <summary>
    /// <see href="https://github.com/dahall/Vanara/blob/ac0a1ac301dd4fdea9706688dedf96d596a4908a/Windows.Shell.Common/StockIcon.cs"/>
    /// </summary>
    /// <returns></returns>
    public async Task InitializeStockIcons()
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
