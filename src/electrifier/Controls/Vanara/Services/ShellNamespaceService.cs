using Vanara.Windows.Shell;
using Vanara.PInvoke;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Drawing;
using System.Diagnostics;
using static Vanara.PInvoke.Shell32;
using Microsoft.UI.Xaml.Media.Imaging;
using JetBrains.Annotations;
using electrifier.Controls.Vanara.Contracts;
using System.Collections;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using Microsoft.UI.Xaml.Controls;

namespace electrifier.Controls.Vanara.Services;

public partial class ShellNamespaceService : IShellNamespaceService
{
    #region static constants
    public static readonly HRESULT HResultElementNotFound = IShellNamespaceService.HResultElementNotFound;
    public static ShellFolder HomeShellFolder => new(IShellNamespaceService.HomeShellFolder);
    #endregion

    // INFO: 15-11-24: I'll use a single Icon Size for testing purposes
    internal static TempShellIconExtractor IconExtractor { get; } = new(ShellFolder.Desktop);
    public static IReadOnlyList<Bitmap> IconExtractorBitmaps => IconExtractor.ImageList;
    public int IconSize => IconExtractor.ImageSize;

    public List<BrowserStockIcon> StockIcons
    {
        get => [];
        init => new List<BrowserStockIcon>();
    }

    protected ArrayWithOffset newStockIcons=new ArrayWithOffset();

    /// <summary>A static reference of our own.
    /// <remarks>When used as service, this static might be unnecessary. However, this
    /// makes sure the Stock Icon cache gets initialized as soon as possible.</remarks>
    /// TODO: This is a currently unresolved race condition
    /// </summary>
    [UsedImplicitly] public static ShellNamespaceService Instance = new();

    /// <summary>ShellNamespaceService() Warn: Actually does not really conform Service Models.</summary>
    public ShellNamespaceService()
    {
        StockIcons =
        [
            new(SHSTOCKICONID.SIID_DOCASSOC),
            new(SHSTOCKICONID.SIID_DOCNOASSOC),
            new(SHSTOCKICONID.SIID_FOLDER),
            new(SHSTOCKICONID.SIID_FOLDERBACK),
            new(SHSTOCKICONID.SIID_FOLDERFRONT),
            new(SHSTOCKICONID.SIID_APPLICATION),
            new(SHSTOCKICONID.SIID_AUDIOFILES),
            new(SHSTOCKICONID.SIID_LINK),
        ];

        newStockIcons = new(StockIcons, 1);
    }

    ///// <summary>Initialize default <see cref="StockIcon">Stock Icons</see>.</summary>
    ///// <remarks>TODO: INFO: Investigate <seealso href="https://github.com/dahall/Vanara/blob/Windows.Shell.Common/StockIcon.cs"></seealso></remarks>
    ///// <returns></returns>
    //public static async Task InitializeStockIconsAsync()
    //{
    //    /* Todo: inspect `SHGetStockIconInfo()` */
    //    //var siFlags = SHGSI.SHGSI_LARGEICON | SHGSI.SHGSI_ICON;
    //    //var siStockIconInfo = new SHSTOCKICONINFO();
    //    //SHGetStockIconInfo(Shell32.SHSTOCKICONID.SIID_APPLICATION, siFlags, ref siStockIconInfo).ThrowIfFailed();

    //    // TODO: Use embedded resource, red cross to signal something failed.
    //    using var siFolder = new StockIcon(SHSTOCKICONID.SIID_FOLDER);
    //    {
    //        var idx = siFolder.SystemImageIndex;
    //        var icnHandle = siFolder.IconHandle.ToIcon();
    //        var bmpSource = GetWinUi3BitmapSourceFromIcon(icnHandle);
    //    }

    //    using var siDocument = new StockIcon(SHSTOCKICONID.SIID_DOCASSOC);
    //    {
    //        var idx = siDocument.SystemImageIndex;
    //        var icnHandle = siDocument.IconHandle.ToIcon();
    //        var bmpSource = GetWinUi3BitmapSourceFromIcon(icnHandle);
    //    }
    //}

    // TODO: Add await event handler to every ebItem, so Icon Extractor can call back the item
    public static async Task<ShellDataTable> RequestChildItemsAsync(ShellFolder shFolder,
        FolderItemFilter itemFilter = (FolderItemFilter.Folders | FolderItemFilter.NonFolders),
        EventHandler? allFastRowsAddedHandler = null, EventHandler? tableLoadedHandler = null)
    {
        Debug.Print($".RequestChildItemsAsync(<{shFolder}>) extracting...");
        var ct = new CancellationToken();

        List<Ole32.PROPERTYKEY> propKeys =
        [
            Ole32.PROPERTYKEY.System.FileFRN /* This is the unique file ID, also known as the File Reference Number. */
        ];

        var shDataTable = new ShellDataTable(shFolder, itemFilter);
        shDataTable.AllFastRowsAdded += allFastRowsAddedHandler;
        shDataTable.TableLoaded += tableLoadedHandler;
        await shDataTable.PopulateTableAsync(propKeys, ct);

        Debug.Print($".RequestChildItemsAsync(<{shFolder}>): {shDataTable.Rows.Count}");

        return shDataTable;
    }

    public async Task<SoftwareBitmapSource> ExtractShellIcon()
    {
        return null;
    }

    public struct BrowserStockIcon(
        Shell32.SHSTOCKICONID shStockIconId,
        ShellIconType shellIconType = ShellIconType.Large,
        bool isLinkOverlay = false,
        bool isSelected = false)
    {
        public readonly bool IsLinkOverlay = isLinkOverlay;
        public readonly bool IsSelected = isSelected;
        public readonly ShellIconType ShellIconType = shellIconType;
        public readonly StockIcon StockIcon = new(shStockIconId, size: shellIconType, isLinkOverlay, isSelected);
        public readonly SHSTOCKICONID StockIconId = shStockIconId;
        //public readonly SHSTOCKICONINFO StockIconInfo = new(SHGetStockIconInfo(shStockIconId, ).ThrowIfFailed("Creating"));

        //public readonly Task<SoftwareBitmapSource?> GetSoftwareBitmapSource() => _softwareBitmapTask;
        private SoftwareBitmapSource _softwareBitmap;

        //public event HasValidationErrorsChangedEventArgs();

        //public Task<SoftwareBitmapSource> Prefetch(ref ShellIconExtractor iconExtractor)
        //{
        //    return new(_softwareBitmap);
        //    //return Task.
        //}
    }

    //IconExtractor.
            //if (GetSoftwareBitmapSource() is null) // lock SoftwareBitmapSource
            //{
            //    //lock (GetSoftwareBitmapSource())
            //    //{
            //    //    //this._softwareBitmapTask = GetWinUi3BitmapSourceFromIcon(new());
            //    //}

            //    //var tsk = this.ExtractShellIcon();
            //    //await tsk;
            //}

/*          using var siDocument = new StockIcon(Shell32.SHSTOCKICONID.SIID_DOCASSOC);
           {
               var idx = siDocument.SystemImageIndex;
               var icnHandle = siDocument.IconHandle.ToIcon();
               var bmpSource = GetWinUi3BitmapSourceFromIcon(icnHandle);
               IShellNamespaceService.DocumentBitmapSource = await bmpSource;   // TODO: Use embedded resource, red cross to signal something failed.
           } */


            //private Task<SoftwareBitmapSource> BitmapSource =>
            //ShellNamespaceService.GetWinUi3BitmapSourceFromIcon(); //new SoftwareBitmapSource();
            /// Task<SoftwareBitmapSource>
            //public static BrowserStockIcon Prefetch(ref BrowserStockIcon thisStockIcon)
            //{
            //    Debug.Print($"BrowserStockIcon.Prefetch({thisStockIcon}))");
            //    return thisStockIcon;
            //}

            //public readonly SHSTOCKICONINFO StockIconInfo = new(SHGetStockIconInfo(shStockIconId, ).ThrowIfFailed("Creating"));

            //public static async Task<BrowserStockIcon> Prefetch(SHSTOCKICONID stockIcon)
            //{
            //    var bmpSrc = new SoftwareBitmapSource();
            //    return bmpSrc;
            //}

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
