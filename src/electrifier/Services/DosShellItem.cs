//using muxc = Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
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
    public ObservableCollection<DosShellItem> Childs;
    public string Name => StorageItem.Name;
    public string Path => StorageItem.Path;
    public bool IsFile => !IsFolder;
    public bool IsFolder
    {
        get;
    }
    public ImageIcon ShellIcon
    {
        get;        // TODO: initialize with default icon
    }
    public IStorageItem StorageItem
    {
        get;
    }

    // TODO: Move to DosShellItemFactory
    //        public static DosShellItem CreateRootItem() =>
    //        new DosShellItem(new StorageFolder("C:\\"));

    public static readonly ImageIcon DefaultUnknownFileIcon = new()
    {
        Source = new BitmapImage(new System.Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico"))
    };

    public DosShellItem(IStorageItem storageItem, bool enumerateChilds = false)
    {
        StorageItem = storageItem;
        IsFolder = storageItem.IsOfType(StorageItemTypes.Folder);
        ShellIcon = DefaultUnknownFileIcon;
        Childs = new ObservableCollection<DosShellItem>();

        if (enumerateChilds)
        {
            _ = GetChildsAsync();
        }
    }

    public async Task GetChildsAsync()
    {
        if (IsFolder)
        {
            var folder = (StorageFolder)StorageItem;
            var items = await folder.GetItemsAsync();
            foreach (var item in items)
            {
                Childs.Add(new DosShellItem(item));
            }
        }
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
