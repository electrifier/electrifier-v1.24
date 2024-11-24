using electrifier.Controls.Vanara.Services;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using System.Drawing;
using Vanara.PInvoke;

namespace electrifier.Controls.Vanara.Helpers;
internal class BrowserItemFactory
{
    public static readonly Dictionary<Shell32.SHSTOCKICONID, SoftwareBitmapSource> StockIconDictionary = [];

    public static Task<SoftwareBitmapSource> GetStockIconBitmapSource(Shell32.SHSTOCKICONID shStockIconId,
        Shell32.SHGSI gsiFlags = (Shell32.SHGSI.SHGSI_LARGEICON | Shell32.SHGSI.SHGSI_ICON)) =>
        GetStockIconBitmapSource(shStockIconId, new Shell32.SHSTOCKICONINFO(), gsiFlags);

    public static async Task<SoftwareBitmapSource> GetStockIconBitmapSource(Shell32.SHSTOCKICONID shStockIconId,
        Shell32.SHSTOCKICONINFO stockIconInfo, Shell32.SHGSI gsiFlags = (Shell32.SHGSI.SHGSI_LARGEICON | Shell32.SHGSI.SHGSI_ICON))
    {
        try
        {
            if (StockIconDictionary.TryGetValue(shStockIconId, out var source))
            {
                Debug.Print($".GetStockIconBitmapSource({shStockIconId}) cache hit");
                return source;
            }

            var hr = Shell32.SHGetStockIconInfo(shStockIconId, gsiFlags, ref stockIconInfo);
                
                
                //.ThrowIfFailed($"SHGetStockIconInfo({shStockIconId})");
            var hIcon = stockIconInfo.hIcon;
            Icon? icnHandle;

            if ((icnHandle = hIcon.ToIcon()) is not null)
            {
                var bmpSource = ShellNamespaceService.GetWinUi3BitmapSourceFromIcon(icnHandle);
                await bmpSource;
                var softBitmap = bmpSource.Result;      //.SetBitmapAsync(bmpSource.Result); TODO: Check this!

                if (softBitmap != null)
                {
                    _ = StockIconDictionary.TryAdd(shStockIconId, softBitmap);
                    return softBitmap;
                }
            }

            throw new ArgumentOutOfRangeException($".GetStockIconBitmapSource(): Can't get StockIcon for SHSTOCKICONID: {shStockIconId}");
        }
        catch (Exception)
        {
            throw; // TODO handle exception
        }
    }
}
