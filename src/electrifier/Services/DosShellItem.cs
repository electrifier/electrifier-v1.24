using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Resources;
using System.Runtime.CompilerServices;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.Storage;

namespace electrifier.Services;

/// <summary>
/// https://learn.microsoft.com/en-us/uwp/api/windows.storage.search.commonfolderquery?view=winrt-22621
/// </summary>
public class DosShellItem : INotifyPropertyChanged
{
    public ObservableCollection<DosShellItem> Children;

    //doc: https://docs.microsoft.com/en-us/uwp/api/windows.storage.search.queryoptions
    public QueryOptions EnumerationQueryOptions
    {
        get;
    }
    public bool HasChildren => Children.Count > 0;
    public bool IsFile => !IsFolder;
    //public bool IsLibrary => StorageItem is StorageFolder folder && folder.IsOfType(StorageItemTypes.Library);
    public bool IsFolder
    {
        get;
    }
    public string Name => StorageItem?.Name ?? "[Unknown Item]";
    public string Path => StorageItem?.Path ?? "[Invalid Path]";
    public ImageIcon ShellIcon
    {
        get;
    }
    public IStorageItem? StorageItem
    {
        get;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    // TODO: Move to DosShellItemFactory
    //        public static DosShellItem CreateRootItem() =>
    //        new DosShellItem(new StorageFolder("C:\\"));

    // TODO: public readonly QueryOptions defaultFolderQueryOptions = new(CommonFileQuery.OrderByName, null);  
    // => Helpers.DefaultQueryOptionsCommonFile

    /// <summary>
    /// 
    /// </summary>
    /// <param name="storageItem"></param>
    /// <param name="forcedFolderQueryOptions">forcedFolderQueryOptions != null => Start enumerating Children</param>
    /// <exception cref="ArgumentNullException">StorageItem is null</exception>
    public DosShellItem(IStorageItem storageItem, QueryOptions? forcedFolderQueryOptions = null)
    {
        StorageItem = storageItem ?? throw new ArgumentNullException(nameof(storageItem));
        Children = new ObservableCollection<DosShellItem>();

        // Determine if the item is a folder
        IsFolder = StorageItem.IsOfType(StorageItemTypes.Folder);
        // Set temporary icon
        ShellIcon = IsFolder ? DosShellItemHelpers.DefaultFolderIcon : DosShellItemHelpers.DefaultUnknownFileIcon;

        if (forcedFolderQueryOptions != null)
        {
            EnumerationQueryOptions = forcedFolderQueryOptions;
        }
        else
        {
            EnumerationQueryOptions = new QueryOptions(CommonFolderQuery.DefaultQuery);
        }

        _ = GetChildsAsync();

        #region old stuff
        //        EnumerationQueryOptions = forcedFolderQueryOptions != null ? forcedFolderQueryOptions // : DosShellItemHelpers.DefaultFolderIcon;


        //if (forcedFolderQueryOptions != null)
        //{
        //    //  Enumerate Children
        //    _ = GetChildsAsync();
        //}



        //Children.Add(new DosShellItem(storageItem)); // TODO: Get reference to root object and add their children

        //if ((IsFolder) && (enumerateChilds))
        //{
        //    // => Children = new ObservableCollection<DosShellItem>();
        //    _ = GetChildsAsync();
        //}
        #endregion
    }

    // Copilot: KnownLibraryIdGetLibraryId() => StorageItem is StorageFolder folder ? folder.LibraryRelativePath : KnownLibraryId.Unknown;
    public DosShellItem(KnownLibraryId libraryId, QueryOptions? forcedFolderQueryOptions = null)
    {
        var library = StorageLibrary.GetLibraryAsync(libraryId);
        Children = new ObservableCollection<DosShellItem>();

        if (forcedFolderQueryOptions != null)
        {
            EnumerationQueryOptions = forcedFolderQueryOptions;
        }
        else
        {
            EnumerationQueryOptions = new QueryOptions(CommonFolderQuery.DefaultQuery);
        }

        // StorageItem = library as IStorageItem ?? throw new ArgumentException(nameof(libraryId), "can't create StorageItem from LibraryId");
        if (library is IStorageItem storageItem)
        {
            StorageItem = storageItem;

            // Determine if the item is a folder
            IsFolder = StorageItem.IsOfType(StorageItemTypes.Folder);
            // Set temporary icon
            ShellIcon = IsFolder ? DosShellItemHelpers.DefaultFolderIcon : DosShellItemHelpers.DefaultUnknownFileIcon;

            _ = GetChildsAsync();
        }
        else
        {
            IsFolder = false;

            ShellIcon = DosShellItemHelpers.DefaultUnknownFileIcon;
        }
    }
    //    {
    //        //if(KnownLibraryId.Unknown == library)
    //        //{
    //        //    throw new ArgumentException("Unknown library");
    //        //}
    //        
    //        //StorageItem = library.GetLibraryAsync();
    //        //Children = new ObservableCollection<DosShellItem>();
    //
    //
    //
    //        //StorageItem = parentShellItem.StorageItem;
    //
    //        //this(StorageItem, forcedFolderQueryOptions);
    //    }

    /*
    var forcedFolderQueryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);

    // Create query and retrieve files
    var query = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(forcedFolderQueryOptions);
    IReadOnlyList<StorageFile> childReadOnlyList = await query.GetFilesAsync();
    // Process results
    foreach (StorageFile file in childReadOnlyList)
    {
        // Process file
    } */

    //#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    //    private async Task QueryChildItemsAsync()
    //#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    //    {
    //        if (IsFolder)
    //        {
    //            //var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, null);
    //
    //            // Create query and retrieve files
    //            //        var result = Windows.Storage.Search.StorageFileQueryResult(this.StorageItem as StorageFolder, forcedFolderQueryOptions);
    //
    //
    //            //= KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(forcedFolderQueryOptions);
    //            //IReadOnlyList<StorageFile> childReadOnlyList = await query.GetChildsAsync();
    //
    //            //IReadOnlyList<StorageFile> childReadOnlyList = new IReadOnlyList<StorageFile>();
    //
    //            // Process results
    //            //foreach (StorageFile file in childReadOnlyList)
    //            //{
    //            //    Children.Add(new DosShellItem(file));
    //            //}
    //
    //            /*      if (enumerateChilds)
    //                    {
    //                        _ = GetChildsAsync();
    //                    } */
    //            /*      var folder = (StorageFolder)StorageItem;
    //                    var items = await folder.ShellGridViewItems_GetItemsAsync();
    //                    foreach (var item in items)
    //                    {
    //
    //                        Children.Add(new DosShellItem(item));
    //                    } */
    //        }
    //        else
    //        {
    //            return;
    //        }
    //    }

    public async Task GetChildsAsync()
    {
        if (IsFolder)
        {
            var folder = (StorageFolder)StorageItem;
            var items = await folder.GetItemsAsync();

            foreach (var item in items)
            {
                Children.Add(new DosShellItem(item));
            }
        }
    }

    public async Task<BitmapImage> GetImageThumbnailAsync()
    {
        if (IsFolder)
        {
            return new BitmapImage(new System.Uri("ms-appx:///Assets/Views/Workbench/Shell32 Folder containing File.ico"));
        }
        else
        {
            try
            {
                var bitmapImage = new BitmapImage();

                using (var thumbnail = await ((StorageFile)StorageItem).GetThumbnailAsync(ThumbnailMode.SingleItem))
                {
                    bitmapImage.SetSource(thumbnail);
                    thumbnail.Dispose();
                }

                return bitmapImage;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
