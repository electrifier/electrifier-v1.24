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

namespace electrifier.Controls.Vanara.Services;

public partial class ShellNamespaceService
{
    public IReadOnlyList<Bitmap> IconBitmaps;
    internal TempShellIconExtractor IconExtractor  = new(ShellFolder.Desktop);
    public int IconSize => IconExtractor.ImageSize;
    private static SoftwareBitmapSource? _defaultFolderImageBitmapSource;
    private static SoftwareBitmapSource? _defaultDocumentAssocImageBitmapSource;

    private Task? _stockIconTask;

    public ShellNamespaceService()
    {
        IconBitmaps = IconExtractor.ImageList;
        _stockIconTask = InitializeStockIcons();
    }

    public void Dispose() { }

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
                _defaultFolderImageBitmapSource = await bmpSource;
            }

            using var siDocument = new StockIcon(Shell32.SHSTOCKICONID.SIID_DOCNOASSOC);
            {
                var idx = siDocument.SystemImageIndex;
                var icnHandle = siDocument.IconHandle.ToIcon();
                var bmpSource = GetWinUi3BitmapSourceFromIcon(icnHandle);
                _defaultDocumentAssocImageBitmapSource = await bmpSource;
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
