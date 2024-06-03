﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Shell32TreeViewItem
{
    private static readonly BitmapImage DefaultFileImage = new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico"));
    private static readonly BitmapImage DefaultFolderImage = new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico"));
    private static readonly BitmapImage libraryBitmapImageImage = new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Library.ico"));

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

    private string GetDebuggerDisplay()
    {
        return nameof(Shell32TreeViewItem) + ToString();
    }
}
