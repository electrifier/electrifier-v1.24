using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI;
using electrifier.Services;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Search;

namespace electrifier.ViewModels;

public partial class FileManagerViewModel : ObservableRecipient
{
    // AdvancedCollectionView can be bound to anything that uses collections.
    public AdvancedCollectionView ShellGridCollectionViewItems
    {
        get;
    }
    public ObservableCollection<DosShellItem> ShellGridViewItems { get; } = new ObservableCollection<DosShellItem>();
    public ObservableCollection<DosShellItem> ShellTreeViewItems { get; } = new ObservableCollection<DosShellItem>();
    public uint FolderCount
    {
        get;
    }
    public uint FileCount
    {
        get;
    }
    public uint ItemCount
    {
        get;
    }

    public FileManagerViewModel()
    {
        // Set up the AdvancedCollectionView with live shaping enabled to filter and sort the original list
        ShellGridCollectionViewItems = new AdvancedCollectionView(ShellGridViewItems, true);
        // And sort ascending by the property "Name"
        ShellGridCollectionViewItems.SortDescriptions.Add(new SortDescription("Name", SortDirection.Ascending));
        // TODO: TreeView -> OnSelection: _ = ShellGridViewItems_GetItemsAsync(KnownLibraryId.Documents);

        _ = ShellTreeViewItems_GetItemsAsync(KnownLibraryId.Documents);
        _ = ShellGridViewItems_GetItemsAsync(KnownLibraryId.Documents);


        //ShellGridViewItems.CollectionChanged += (s, e) =>
        //{
        //    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        //    {
        //        var addedItems = e.NewItems;

        //        if (addedItems is null)
        //        {
        //            throw new ArgumentNullException(nameof(addedItems));
        //        }

        //        if (addedItems.Count == 0)
        //        {
        //            return;
        //        }

        //        foreach (DosShellItem item in addedItems)
        //        {
        //            if (item.IsFolder)
        //            {
        //                ShellTreeViewItems.Add(item);
        //            }
        //        }
        //    }
        //};
    }



    private async Task ShellTreeViewItems_GetItemsAsync(KnownLibraryId libraryId)
    {
        var library = await StorageLibrary.GetLibraryAsync(libraryId);
        var saveFolder = library.SaveFolder;

        if (saveFolder != null)
        {
            _ = ShellTreeViewItems_GetItemsAsync(saveFolder);
        }
    }

    private async Task ShellTreeViewItems_GetItemsAsync(StorageFolder storageFolder)
    {
        var childrenQueryResult = storageFolder.CreateFolderQueryWithOptions(new QueryOptions());
        var storageFolders = await childrenQueryResult.GetFoldersAsync();

        foreach (var storageItem in storageFolders)
        {
            ShellTreeViewItems.Add(await LoadShellItemInfo(storageItem));
        }
    }

    private async Task ShellGridViewItems_GetItemsAsync(KnownLibraryId libraryId)
    {
        var library = await StorageLibrary.GetLibraryAsync(libraryId);
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

        foreach (var storageItem in storageFolders)
        {
            ShellGridViewItems.Add(await LoadShellItemInfo(storageItem));
        }

        /*
                StorageFolder picturesFolder;
                picturesFolder = KnownFolders.DocumentsLibrary;
                var result = picturesFolder.CreateFileQueryWithOptions(new QueryOptions());
                var storageFiles = await result.GetFilesAsync();
                foreach (var storageItem in storageFiles)
                {
                    ShellGridViewItems.Add(await LoadShellItemInfo(storageItem));
                }
                ImageGridView.ItemsSource = ShellGridViewItems;
        */
    }


    private void ShellGridViewItems_ContainerContentChanging(
#pragma warning disable IDE0060 // Remove unused parameter
            ListViewBase sender,
#pragma warning restore IDE0060 // Remove unused parameter
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
                        Image img = sender as Image; 
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
                        throw new ArgumentNullException(nameof(args), ".Item is null");
                    }
                }
            }
        }
    }

    public static async Task<DosShellItem> LoadShellItemInfo(IStorageItem item)
    {
        DosShellItem shellItem = new(item ?? throw new ArgumentNullException(nameof(item)));
        Debug.Assert(shellItem != null);

        await item.GetBasicPropertiesAsync();

        return shellItem;
    }
}
