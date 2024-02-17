using electrifier.Services;
using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.Storage;
//using Microsoft.UI.Xaml.Media.Imaging;
//using CommunityToolkit.WinUI.Collections;
using CommunityToolkit.WinUI.UI;
using System;
using System.Diagnostics;

namespace electrifier.Views;

public sealed partial class FileManagerPage : Page
{
    public AdvancedCollectionView CollectionView
    {
        get;
    }
    public ObservableCollection<DosShellItem> ShellItems { get; } = new ObservableCollection<DosShellItem>();
    public FileManagerViewModel ViewModel
    {
        get;
    }


    public FileManagerPage()
    {
        ViewModel = App.GetService<FileManagerViewModel>() ?? throw new InvalidOperationException();

        // Set up the AdvancedCollectionView with live shaping enabled to filter and sort the original list
        CollectionView = new AdvancedCollectionView(ShellItems, true);
        // And sort ascending by the property "FileName"
        CollectionView.SortDescriptions.Add(new SortDescription("FileName", SortDirection.Ascending));

        InitializeComponent();
        // AdvancedCollectionView can be bound to anything that uses collections. 
        ImageGridView.ItemsSource = CollectionView;

        _ = GetItemsAsync(KnownLibraryId.Pictures);


        // Let's filter out the integers
        // Let's add a Person to the observable collection
        //        var person = new DosShellItem { Name = "Aardvark" };
        //        ShellItems.Add(person);
        // Our added person is now at the top of the list, but if we rename this person, we can trigger a re-sort
        //        person.Name = "Zaphod"; // Now a re-sort is triggered and person will be last in the list

        // StorageFolder storageFolder = Package.Current.InstalledLocation;
        // StorageLibrary storageFolder = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
        // var library = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
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
                var imageElement = templateRoot.FindName("ItemImageIcon") as ImageIcon;

                if (imageElement != null)
                {
                    if (args.Item is DosShellItem item)
                    {
                        var bitmapImage = await item.GetImageThumbnailAsync();
                        imageElement.Source = bitmapImage;



                        /*
                         *     Image img = sender as Image; 
                //BitmapImage bitmapImage = new BitmapImage();
                //img.Width = bitmapImage.DecodePixelWidth = 80; 
                //// Natural px width of image source.
                //// You don't need to set Height; the system maintains aspect ratio, and calculates the other
                //// dimension, as long as one dimension measurement is provided.
                //bitmapImage.UriSource = new Uri(img.BaseUri,"Assets/StoreLogo.png");
                //img.Source = bitmapImage;*/

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
        var storageFolder = library.SaveFolder;

        if (storageFolder != null)
        {
            _ = GetItemsAsync(storageFolder);
        }
    }

    private async Task GetItemsAsync(StorageFolder storageFolder)
    {
        var folderQuery = storageFolder.CreateItemQuery();
        var items = await folderQuery.GetItemsAsync();

        foreach (var storageItem in items)
        {
            ShellItems.Add(await LoadShellItemInfo(storageItem));
        }

        //ImageGridView.ItemsSource = ShellItems;

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

                foreach (var storageItem in storageFiles)
                {
                    ShellItems.Add(await LoadShellItemInfo(storageItem));
                }

                ImageGridView.ItemsSource = ShellItems;
        */
    }

    public static async Task<DosShellItem> LoadShellItemInfo(IStorageItem item)
    {
        DosShellItem shellItem = new(item ?? throw new ArgumentNullException(nameof(item)));
        Debug.Assert(shellItem != null);

        await item.GetBasicPropertiesAsync();

        return shellItem;
    }
}
