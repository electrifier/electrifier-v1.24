using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace electrifier.Controls.Vanara;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Shell32TreeViewItem
{
    private static readonly BitmapImage DefaultFileImage = new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico"));
    private static readonly BitmapImage DefaultFolderImage = new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico"));
    private static readonly BitmapImage libraryBitmapImageImage = new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Library.ico"));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(Shell32TreeViewItem), new PropertyMetadata(default(bool)));

    public ObservableCollection<Shell32TreeViewItem> Children
    {
        get;
    }
    public string DisplayName
    {
        get;
    }

    private bool IsEnumerated
    {
        get; set;
    }

    public bool IsExpanded;

    // TODO: Add observable flags for async property loading
    public bool HasUnrealizedChildren
    {
        get;
        private set;
    }
    public ShellItem ShellItem
    {
        get;
    }

    public ImageSource ImageIconSource
    {
        get;
    }

    //public bool IsExpanded
    //{
    //    get => (bool)GetValue(IsExpandedProperty);
    //    set => SetValue(IsExpandedProperty, value);
    //}

    public Shell32TreeViewItem(ShellItem shItem)
    {
        ShellItem = shItem ?? throw new ArgumentNullException(nameof(shItem));
        DisplayName = ShellItem.Name ?? ShellItem.ToString();
        Children = new ObservableCollection<Shell32TreeViewItem>();
        HasUnrealizedChildren = true;


        switch (ShellItem)
        {
            // var lib = ShellLibrary()
            //case ShellFolder { IsLibrary: true }:
            //    ImageIconSource = librarayBitmapImageImage;
            //    break;
            case ShellFolder { IsFolder: true }:
                ImageIconSource = DefaultFolderImage;
                break;
            default:
                ImageIconSource = DefaultFileImage;
                break;
        }

        //ImageIconSource = shItem.IsFolder
        //    ? DefaultFolderImage
        //    : DefaultFileImage;

        _ = Task.Run(InitializeAsync);
    }

    public async Task InitializeAsync()
    {
        var attributes = await Task.Run(() => ShellItem.Attributes);
        var StorageCapMask = await Task.Run(() => attributes & ShellItemAttribute.StorageCapMask);

        HasUnrealizedChildren = attributes.HasFlag(ShellItemAttribute.HasSubfolder);
    }

    /*
    internal IEnumerable<ShellItem> EnumerateChildren(FolderItemFilter filter)
       {
           IsEnumerated = false;

           try
           {
               return ShellItem is not ShellFolder folder
                   ? Enumerable.Empty<ShellItem>()
                   : folder.EnumerateChildren(filter);
           }
           finally
           {
               IsEnumerated = true;
               HasUnrealizedChildren = false;
           }
       }

     */

    private string GetDebuggerDisplay()
    {
        return nameof(Shell32TreeViewItem) + ToString();
    }
}
