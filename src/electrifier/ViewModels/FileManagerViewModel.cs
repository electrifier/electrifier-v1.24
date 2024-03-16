using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI;
using electrifier.Services;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage.Search;
using Windows.Storage;

namespace electrifier.ViewModels;

public partial class FileManagerViewModel : ObservableRecipient
{
    public AdvancedCollectionView CollectionView
    {
        get;
    }
    public ObservableCollection<DosShellItem> ShellGridViewItems { get; } = new ObservableCollection<DosShellItem>();
    //public ObservableCollection<DosShellItem> ShellTreeViewItems { get; } = new ObservableCollection<DosShellItem>();

    public IList<DosShellItem> ShellTreeViewItems { get; } = new ObservableCollection<DosShellItem>();

    public FileManagerViewModel()
    {

        // Set up ShellTreeViewItems with the root items
        var rootItem = new DosShellItem(Package.Current.InstalledLocation);
        _ = GetTreeViewItemsAsync(rootItem);
        //        _ = GetTreeViewItemsAsync(new DosShellItem(new StorageItem("C:\\")));
 //       ShellTreeView.ItemsSource = ShellTreeViewItems;

        // Set up the AdvancedCollectionView with live shaping enabled to filter and sort the original list
        CollectionView = new AdvancedCollectionView(ShellGridViewItems, true);
        // And sort ascending by the property "Name"
        CollectionView.SortDescriptions.Add(new SortDescription("Name", SortDirection.Ascending));

        // AdvancedCollectionView can be bound to anything that uses collections. 
        _ = ShellGridViewItems_GetItemsAsync(KnownLibraryId.Documents);
//        ShellGridView.ItemsSource = CollectionView;


    }

    //    private readonly Task enumerateFoldersTask = Task.Run(() => EnumerateFolders(@"C:\", true));
    //
    //    /// <summary>
    //    /// functionality to enumerate folders
    //    /// <throws cref="DirectoryNotFoundException"></throws>
    //    /// <throws cref="UnauthorizedAccessException"></throws>
    //    /// <throws cref="PathTooLongException"></throws>
    //    /// </summary>
    //    /// 
    //    public static string EnumerateFolders(string rootPath, bool enumerateChildFoldersIfAllowed = false)
    //    {
    //        try
    //        {
    //            foreach (var dirName in Directory.EnumerateDirectories(rootPath))
    //            {
    //                return dirName;
    //                //Folders.Add(dirName);
    //
    //                //if (enumerateChildFoldersIfAllowed)
    //                //{
    //                //    EnumerateFolders(dirName, false);
    //                //}
    //            }
    //        }
    //        catch (DirectoryNotFoundException ex)
    //        {
    //            throw new DirectoryNotFoundException(rootPath, ex);
    //        }
    //        catch (UnauthorizedAccessException ex)
    //        {
    //            throw new UnauthorizedAccessException(rootPath, ex);
    //        }
    //        catch (PathTooLongException ex)
    //        {
    //            throw new PathTooLongException(rootPath, ex);
    //        }
    //
    //        return "{ unknown error in $EnumerateFolders }";
    //    }

    private async Task GetTreeViewItemsAsync(DosShellItem rootItem)
    {
        if (rootItem is null)
        {
            throw new ArgumentNullException(nameof(rootItem));
        }

        var storageFolder = rootItem.StorageItem as StorageFolder;

        if (storageFolder != null)
        {
            var item = new DosShellItem(storageFolder);

            rootItem.Children.Add(item);
            await GetTreeViewItemsAsync(storageFolder);
        }
    }

    private async Task GetTreeViewItemsAsync(StorageFolder storageFolder)
    {

        var fileQuery = storageFolder.CreateFileQueryWithOptions(new QueryOptions());
        //        var storageFiles = await fileQuery.GetFilesAsync();
        var storageFolders = await fileQuery.GetFilesAsync();

        ///
        //var folderQuery = storageFolder.CreateItemQuery();
        //var items = await folderQuery.ShellGridViewItems_GetItemsAsync();

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
        //        var items = await folderQuery?.ShellGridViewItems_GetItemsAsync();
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

    private void ShellGridViewItems_ContainerContentChanging(
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
            args.RegisterUpdateCallback(ShellGridViewItems_GetImageAsync);
            args.Handled = true;
        }
    }

    private async void ShellGridViewItems_GetImageAsync(
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

    private async Task ShellGridViewItems_GetItemsAsync(KnownLibraryId storageLibrary)
    {
        var library = await StorageLibrary.GetLibraryAsync(storageLibrary);
        var storageFolder = library.SaveFolder;

        if (storageFolder != null)
        {
            _ = ShellGridViewItems_GetItemsAsync(storageFolder);
        }
    }

    private async Task ShellGridViewItems_GetItemsAsync(StorageFolder storageFolder)
    {
        var fileQuery = storageFolder.CreateFileQueryWithOptions(new QueryOptions());
        var storageFolders = await fileQuery.GetFilesAsync();

        //var folderQuery = storageFolder.CreateItemQuery();
        //var items = await folderQuery.ShellGridViewItems_GetItemsAsync();

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

    //    private async Task GetTreeViewItemsAsync(KnownLibraryId storageLibrary)
    //    {
    //        var library = await StorageLibrary.GetLibraryAsync(storageLibrary);
    //        var storageFolder = library.SaveFolder;

    //        if (storageFolder != null)
    //        {
    //            var shellItem = new DosShellItem(storageFolder);
    //            ShellTreeViewItems.Add(shellItem);
    ////            _ = GetTreeViewItemsAsync(storageFolder);
    //        }
    //    }


    public static async Task<DosShellItem> LoadShellItemInfo(IStorageItem item)
    {
        DosShellItem shellItem = new(item ?? throw new ArgumentNullException(nameof(item)));
        Debug.Assert(shellItem != null);

        await item.GetBasicPropertiesAsync();

        return shellItem;
    }

    private void ImageGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Debug.Assert(e != null);

        var addedItems = e.AddedItems;
        var removedItems = e.RemovedItems;

        if (addedItems.Count > 0)
        {
            var item = e.AddedItems[0] as DosShellItem;
            //ImageGridView.SelectedItem = item;

        }
    }
}
