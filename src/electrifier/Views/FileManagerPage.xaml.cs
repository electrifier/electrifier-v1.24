using Windows.Storage;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls;
using electrifier.ViewModels;
using electrifier.Services;
using CommunityToolkit.WinUI.UI;
using Windows.Storage.Search;
//using CommunityToolkit.WinUI.Collections;

namespace electrifier.Views;

// TODO: Exception thrown: 'System.UnauthorizedAccessException' in System.Private.CoreLib.dll
public sealed partial class FileManagerPage : Page
{
    public AdvancedCollectionView CollectionView { get; }
    public ObservableCollection<DosShellItem> ShellGridViewItems { get; } = new ObservableCollection<DosShellItem>();
    //public ObservableCollection<DosShellItem> ShellTreeViewItems { get; } = new ObservableCollection<DosShellItem>();

    public IList<DosShellItem> ShellTreeViewItems { get; } = new ObservableCollection<DosShellItem>();

    public FileManagerViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileManagerPage"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public FileManagerPage()
    {
        ViewModel = App.GetService<FileManagerViewModel>() ?? throw new InvalidOperationException();

        InitializeComponent();
        _ = GetTreeViewItemsAsync(KnownLibraryId.Pictures);
        ShellTreeView.ItemsSource = ShellTreeViewItems;

        // Set up the AdvancedCollectionView with live shaping enabled to filter and sort the original list
        CollectionView = new AdvancedCollectionView(ShellGridViewItems, true);
        // And sort ascending by the property "Name"
        CollectionView.SortDescriptions.Add(new SortDescription("Name", SortDirection.Ascending));

        // AdvancedCollectionView can be bound to anything that uses collections. 
        _ = GetItemsAsync(KnownLibraryId.Pictures);
        ShellGridView.ItemsSource = CollectionView;


        // --------------------------------------------
        //_ = GetTreeViewItemsAsync(KnownLibraryId.Pictures);

        // add dummy items to ShellTreeViewItems
        // ShellTreeViewItems.Add(new DosShellItem(new StorageItem("C:\\")));
        // ShellTreeViewItems.Add(new DosShellItem("D:\\"));
        //
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
        var fileQuery = storageFolder.CreateFileQueryWithOptions(new QueryOptions());
//        var storageFiles = await fileQuery.GetFilesAsync();
        var storageFolders = await fileQuery.GetFilesAsync();

        ///
        //var folderQuery = storageFolder.CreateItemQuery();
        //var items = await folderQuery.GetItemsAsync();

        foreach (var storageItem in storageFolders)
        {
            ShellGridViewItems.Add(await LoadShellItemInfo(storageItem));
        }

        //ImageGridView.ItemsSource = ShellGridViewItems;

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
                    ShellGridViewItems.Add(await LoadShellItemInfo(storageItem));
                }

                ImageGridView.ItemsSource = ShellGridViewItems;
        */
    }

    private async Task GetTreeViewItemsAsync(KnownLibraryId storageLibrary)
    {
        var library = await StorageLibrary.GetLibraryAsync(storageLibrary);
        var storageFolder = library.SaveFolder;

        if (storageFolder != null)
        {
            _ = GetTreeViewItemsAsync(storageFolder);
        }
    }


    private async Task GetTreeViewItemsAsync(StorageFolder storageFolder)
    {

        var fileQuery = storageFolder.CreateFileQueryWithOptions(new QueryOptions());
        //        var storageFiles = await fileQuery.GetFilesAsync();
        var storageFolders = await fileQuery.GetFilesAsync();

        ///
        //var folderQuery = storageFolder.CreateItemQuery();
        //var items = await folderQuery.GetItemsAsync();

        foreach (var storageItem in storageFolders)
        {
            ShellTreeViewItems.Add(await LoadShellItemInfo(storageItem));
        }
        //        var rootFolder = storageFolder;
        //        if (rootFolder == null)
        //        {
        //            return;
        //        }
        //
        //        var rootItem = new DosShellItem(rootFolder/* , DosShellItemHelpers.DefaultQueryOptionsCommonFile */);
        //        ShellTreeViewItems.Add(rootItem);
        //
        //        ShellTreeView.ItemsSource = ShellTreeViewItems;
        //
        //
        //        rootItem.Children.Add(new DosShellItem(rootFolder/* , DosShellItemHelpers.DefaultQueryOptionsCommonFile */));
        //        rootItem.Children.Add(new DosShellItem(rootFolder/* , DosShellItemHelpers.DefaultQueryOptionsCommonFile */));


        //        var folderQuery = rootFolder.CreateItemQuery();
        //        var items = await folderQuery?.GetItemsAsync();
        //
        //        Debug.Assert(items != null);
        //        ShellTreeViewItems.Clear();
        //        ShellTreeViewItems.Add(new DosShellItem(rootFolder, DosShellItemHelpers.DefaultQueryOptionsCommonFile));

        //var rootItem = ShellTreeViewItems[0] as DosShellItem;
        //Debug.Assert(rootItem != null);
        //foreach (var storageItem in items)
        //{

        //    //rootItem.Children.Add(new DosShellItem(storageItem));
        //    ShellTreeViewItems.Add(await LoadShellItemInfo(storageItem));

        //}
    }




    public static async Task<DosShellItem> LoadShellItemInfo(IStorageItem item)
    {
        DosShellItem shellItem = new(item ?? throw new ArgumentNullException(nameof(item)));
        Debug.Assert(shellItem != null);

        await item.GetBasicPropertiesAsync();

        return shellItem;
    }


    private void ImageGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var addedItems = e.AddedItems;
        var removedItems = e.RemovedItems;


        if (addedItems.Count > 0)
        {
            var item = e.AddedItems[0] as DosShellItem;
            //ImageGridView.SelectedItem = item;

        }
    }

    // Generate ScrollViewerControl_ViewChanged
    private void ScrollViewerControl_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {

    }
}
