//using muxc = Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace electrifier.Services;

public class DosShellItem : INotifyPropertyChanged
{
    public string FileName => StorageItem.Name;

    public string FileType => StorageItem.Path;

    public bool IsFile => !IsFolder;
    public bool IsFolder
    {
        get;
    }
    public ImageIcon ShellIcon
    {
        get;        // TODO: this is not initialized yet
    }
    public IStorageItem StorageItem
    {
        get;
    }

    public DosShellItem(IStorageItem storageItem)
    {
        StorageItem = storageItem;
        IsFolder = storageItem.IsOfType(StorageItemTypes.Folder);
        ShellIcon = new() { Source = new BitmapImage(new System.Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico")) };
    }

    public async Task<BitmapImage> GetImageThumbnailAsync()
    {
        try
        {
            var bitmapImage = new BitmapImage();

            // TODO: Load folder image from resource
            if (IsFolder)
            {
                //// "..\Assets\Views\Workbench\Shell32 Default unknown File.ico"
                //bitmapImage.SetSourceAsync();
            }
            else
            {
                using var thumbnail = await ((StorageFile)StorageItem).GetThumbnailAsync(ThumbnailMode.SingleItem);
                if (thumbnail != null)
                {
                    bitmapImage.SetSource(thumbnail);
                    thumbnail.Dispose();
                }
            }

            return bitmapImage;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
