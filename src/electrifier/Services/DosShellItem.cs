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
    private const string shell32DefaultUnknownFileIcon = "ms-appx:///Assets /Views/Workbench/Shell32 Default unknown File.ico";

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

        ShellIcon = new ImageIcon();

            //ResourceManager.GetForCurrentView().GetString("Files/foo.png");
        // shell32DefaultUnknownFileIcon


        /////////////////////////////////////////////////////////////
        ///

        /*
        var uri = new System.Uri("ms-appx:///images/logo.png");
        var file = Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
         */


        //        new ResourceManager().MainResourceMap.GetValue("Files/foo.png").ValueAsString





        //////////////////////////////////////////////////
        ///
        //var bitmap = new BitmapImage();
        ////await bitmapSource.SetSourceAsync(bitmapStream);
        //ShellIcon = new ImageIcon
        //{
        //    Source = bitmap
        //};
        //ShellIcon.Source = "../Assets/Views/Workbench/Shell32 Default unknown File.ico";
        /*
         * 
         * ..\Assets\Views\Workbench\Shell32 Default Folder.ico
        var bitmapSource = new BitmapSource();
        await bitmapSource.SetSourceAsync(bitmapStream);
        var icon = new muxc.ImageIcon() { Source = bitmapSource };
        */
        //        ShellIcon.Source = new ImageSource()
        //                 ShellIcon = new ImageIcon();
        //                ShellIcon
        // "..\Assets\Views\Workbench\Shell32 Default unknown File.ico"

    }

    //protected async Task LoadDefaultImages()
    //{

    //    // using muxc = Microsoft.UI.Xaml.Controls;

    //    //var bitmapSource = new BitmapSource();
    //    //await bitmapSource.SetSourceAsync(bitmapStream);

    //    try
    //    {
    //                var uri = new System.Uri("ms-appx://../Assets/Views/Workbench/Shell32 Default unknown File.ico");
    //        //        var uri = new System.Uri("ms-appx://..\\Assets\\Views\\Workbench\\Shell32 Default Folder.ico");

    //        // ..\Assets\Views\Workbench\Shell32 Default Folder.ico
    //        //var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);



    //        var bmpImage = new BitmapImage(uri);


    //        var icon = new muxc.ImageIcon() { Source = bmpImage };

    //        var icon2 = new muxc.ImageIcon();


    //        /*
    //                var uri = new System.Uri("ms-appx:///images/logo.png");
    //                var file = Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);

    //        */

    //    }
    //    catch(System.Exception ex)
    //    {
    //        var text = ex.Message;
    //    }
    //}



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


/// <summary>
/// -> <seealso cref="SoftwareBitmapSource"/>
/// </summary>
//public ImageIcon ShellIcon
//{
//    get => _shellIcon;
//    private set
//    {
//    }
//}
//private readonly ImageIcon _shellIcon = new();




//ShellIcon = new ImageIcon { Source = "Assets/Views/Workbench/Shell32 Default unknown File.ico" };
// Source="ms-appx:///Assets/globe.png"
//ShellIcon = new ImageIcon {
//    Source = "ms-appx:///Assets /Views/Workbench/Shell32 Default unknown File.ico"
//};
