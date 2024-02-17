using electrifier.Services;
using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.Storage;
using Microsoft.UI.Xaml.Media.Imaging;

namespace electrifier.Views;

public sealed partial class FileManagerPage : Page
{
    public FileManagerViewModel ViewModel
    {
        get;
    }
    public ObservableCollection<DosShellItem> ShellItems { get; } = new ObservableCollection<DosShellItem>();

    public FileManagerPage()
    {
        ViewModel = App.GetService<FileManagerViewModel>() ?? throw new InvalidOperationException();

        InitializeComponent();

        // StorageFolder storageFolder = Package.Current.InstalledLocation;
        // StorageLibrary storageFolder = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
        // var library = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);

        _ = GetItemsAsync(KnownLibraryId.Pictures);
        //_ = GetItemsAsync(storageFolder);
    }

    private void ImageGridView_ContainerContentChanging(
        ListViewBase sender,
        ContainerContentChangingEventArgs args)
    {
        if (args.InRecycleQueue)
        {
            var templateRoot = args.ItemContainer.ContentTemplateRoot as Grid;
            var image = templateRoot?.FindName("ImageIcon") as Image;

            if (image != null)
            {
                image.Source = null;
            }

            args.Handled = true;
        }

        if (args.Phase == 0)
        {
            args.RegisterUpdateCallback(ShowImage);
            args.Handled = true;
        }
    }

    private async void ShowImage(
        ListViewBase sender,
        ContainerContentChangingEventArgs args)
    {
        if (args.Phase == 1)
        {
            // It's phase 1, so show this item's image.
            var templateRoot = args.ItemContainer.ContentTemplateRoot as Grid;

            if (templateRoot != null)
            {
                //var imageElement = templateRoot.FindName("ItemImage") as Image;
                var imageElement = templateRoot.FindName("ItemImageIcon") as ImageIcon;

                if (imageElement != null)
                {
                    if (args.Item is DosShellItem item)
                    {
                        var bitmap = new BitmapImage
                        {
                            //imageElement.Source = new BitmapImage();
                            //                        bitmap.UriSource = new Uri("ms-appx:///Assets/Square44x44Logo.scale-200.png");
                            UriSource = new Uri("ms-appx:///../Assets/Square44x44Logo.scale-200.png")
                        };


                        //new Uri(img.BaseUri, "Assets/StoreLogo.png");
                        //img.Source = bitmapImage;


                        /*
                         *     Image img = sender as Image; 
BitmapImage bitmapImage = new BitmapImage();
img.Width = bitmapImage.DecodePixelWidth = 80; 
// Natural px width of image source.
// You don't need to set Height; the system maintains aspect ratio, and calculates the other
// dimension, as long as one dimension measurement is provided.
bitmapImage.UriSource = new Uri(img.BaseUri,"Assets/StoreLogo.png");
img.Source = bitmapImage;*/

                        //imageElement.Source = new ImageSource(item.ShellIcon);

                        //var task = item?.GetImageThumbnailAsync();

                        //if (task != null)
                        //{
                        //    imageElement.Source = await task;
                        //}

                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(args.Item));
                    }
                }
            }
        }
    }

    private async Task GetItemsAsync(KnownLibraryId storageLibrary)
    {
        var library = await StorageLibrary.GetLibraryAsync(storageLibrary);
        StorageFolder storageFolder = library.SaveFolder;

        if (storageFolder != null)
        {
            _ = GetItemsAsync(storageFolder);
        }
    }

    private async Task GetItemsAsync(StorageFolder storageFolder)
    {
        var folderQuery = storageFolder.CreateItemQuery();

        var items = await folderQuery.GetItemsAsync();

        foreach (var storageFile in items)
        {
            ShellItems.Add(await LoadShellItemInfo(storageFile));
        }

        ImageGridView.ItemsSource = ShellItems;

        //        var fileQuery = storageFolder.CreateFileQueryWithOptions(new QueryOptions());
        //        var storageFiles = await folderQuery.GetFilesAsync();
        //        var storageFolders = await storageFolder.GetFoldersAsync();




        //        return storageFolder.CreateFileQueryWithOptions(new QueryOptions()).ToListAsync();

        //        var result = picturesFolder.CreateFileQueryWithOptions(new QueryOptions());
        //        _ = await result.GetFilesAsync();

        /*
                StorageFolder picturesFolder;

                //picturesFolder = Package.Current.InstalledLocation;
                //picturesFolder = KnownFolders.PicturesLibrary;
                picturesFolder = KnownFolders.DocumentsLibrary;
                //picturesFolder = KnownFolders.HomeGroup;

                var result = picturesFolder.CreateFileQueryWithOptions(new QueryOptions());

                var storageFiles = await result.GetFilesAsync();

                foreach (var storageFile in storageFiles)
                {
                    ShellItems.Add(await LoadShellItemInfo(storageFile));
                }

                ImageGridView.ItemsSource = ShellItems;
        */
    }

    //public static async Task<DosShellItem> LoadShellItemInfo(StorageFile file)
    //{
    //    DosShellItem shellItem = new(file ?? throw new ArgumentNullException(nameof(file)));

    //    try
    //    {
    //        var properties = await file.Properties.GetDocumentPropertiesAsync();

    //        if (file.IsOfType(StorageItemTypes.Folder))
    //        {
    //            bool isFolder = true;

    //        }

    //        return shellItem;
    //    }
    //    catch
    //    {
    //        throw;
    //    }
    //}


    public static async Task<DosShellItem> LoadShellItemInfo(IStorageItem item)
    {
        DosShellItem shellItem = new(item ?? throw new ArgumentNullException(nameof(item)));

        try
        {
            var storageitem = item;

            if (item.IsOfType(StorageItemTypes.Folder))
            {
                var isFolder = true;

            }
            await item.GetBasicPropertiesAsync();

            return shellItem;
        }
        catch
        {
            throw;
        }
    }
}