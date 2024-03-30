/*
    Copyright 2024 Thorsten Jung, aka tajbender
        https://www.electrifier.org

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI;
using electrifier.Services;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Storage.Search;
using Windows.Storage;

namespace electrifier.ViewModels;

public partial class FileManagerViewModel : ObservableRecipient
{
    // AdvancedCollectionView can be bound to anything that uses collections.
    public AdvancedCollectionView GridAdvancedCollectionView
    {
        get;
    } = new AdvancedCollectionView(/* TreeViewItemsCollection */);
    public ObservableCollection<DosShellItem> GridViewItemsCollection { get; } = new ObservableCollection<DosShellItem>();
    // AdvancedCollectionView can be bound to anything that uses collections.
    public AdvancedCollectionView TreeAdvancedCollectionView
    {
        get;
    } = new AdvancedCollectionView(/* TreeViewItemsCollection */);
    public ObservableCollection<DosShellItem> TreeViewItemsCollection { get; } = new ObservableCollection<DosShellItem>();
    /// <summary>
    /// Count of Files
    /// </summary>
    public int FileCount
    {
        get;
    }
    /// <summary>
    /// Count of Folders
    /// </summary>
    public int FolderCount
    {
        get;
    }
    /// <summary>
    /// Count of Items
    /// </summary>
    public int ItemCount
    {
        get;
    }

    /// <summary>
    /// FileManagerViewModel
    /// </summary>
    public FileManagerViewModel()
    {
        // Set up the AdvancedCollectionView with live shaping enabled to filter and sort the original list
        TreeAdvancedCollectionView = new AdvancedCollectionView(TreeViewItemsCollection, true);
        GridAdvancedCollectionView = new AdvancedCollectionView(GridViewItemsCollection, true);
        // And sort ascending by the property "Name"
        TreeAdvancedCollectionView.SortDescriptions.Add(new SortDescription("Name", SortDirection.Ascending));
        GridAdvancedCollectionView.SortDescriptions.Add(new SortDescription("Name", SortDirection.Ascending));
        // TODO: TreeView -> OnSelection: _ = ShellGridViewItems_GetItemsAsync(KnownLibraryId.Documents);
        _ = ShellTreeViewItems_GetItemsAsync(KnownLibraryId.Documents);
        _ = ShellGridViewItems_GetItemsAsync(KnownLibraryId.Documents);

        //        GridViewItemsCollection.CollectionChanged += (s, e) => ShellGridViewItems_ContainerContentChanging(this, new C{ });

        //ShellGridViewItems_ContainerContentChanging

        //GridViewItemsCollection.CollectionChanged += (s, e) =>
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
        //                TreeViewItemsCollection.Add(item);
        //            }
        //        }
        //    }
        //};
    }


    /// <summary>
    /// Enumerate the items in the specified library
    /// </summary>
    /// <param name="libraryId"><see cref="KnownLibraryId" /></param>
    /// <returns></returns>
    private async Task ShellTreeViewItems_GetItemsAsync(KnownLibraryId libraryId)
    {
        var library = await StorageLibrary.GetLibraryAsync(libraryId);
        var saveFolder = library.SaveFolder;

        if (saveFolder != null)
        {
            _ = ShellTreeViewItems_GetItemsAsync(saveFolder);
        }
    }

    /// <summary>
    /// Enumerate the items in the specified folder
    /// </summary>
    /// <param name="storageFolder"></param>
    /// <returns></returns>
    private async Task ShellTreeViewItems_GetItemsAsync(StorageFolder storageFolder)
    {
        var childrenQueryResult = storageFolder.CreateFolderQueryWithOptions(new QueryOptions());
        var storageFolders = await childrenQueryResult.GetFoldersAsync();

        foreach (var storageItem in storageFolders)
        {
            TreeViewItemsCollection.Add(await LoadShellItemInfo(storageItem));
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storageFolder"></param>
    /// <returns></returns>
    private async Task ShellGridViewItems_GetItemsAsync(StorageFolder storageFolder)
    {
        var fileQuery = storageFolder.CreateFileQueryWithOptions(new QueryOptions());
        var storageFolders = await fileQuery.GetFilesAsync();

        foreach (var storageItem in storageFolders)
        {
            GridViewItemsCollection.Add(await LoadShellItemInfo(storageItem));
        }

        /*
                StorageFolder picturesFolder;
                picturesFolder = KnownFolders.DocumentsLibrary;
                var result = picturesFolder.CreateFileQueryWithOptions(new QueryOptions());
                var storageFiles = await result.GetFilesAsync();
                foreach (var storageItem in storageFiles)
                {
                    GridViewItemsCollection.Add(await LoadShellItemInfo(storageItem));
                }
                ImageGridView.ItemsSource = GridViewItemsCollection;
        */
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
#pragma warning disable IDE0051 // Remove unused private members
    private void ShellGridViewItems_ContainerContentChanging(
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter
            ListViewBase sender,
#pragma warning restore IDE0060 // Remove unused parameter
            ContainerContentChangingEventArgs args)
    {
        Debug.Print("ShellGridViewItems_ContainerContentChanging");
        Debug.Assert(args != null);
        Debug.Print("args: {args}");
        Debug.Print($"InRecycleQueue: {args.InRecycleQueue}");
        Debug.Print($"Phase: {args.Phase}");
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <exception cref="ArgumentNullException"></exception>
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
