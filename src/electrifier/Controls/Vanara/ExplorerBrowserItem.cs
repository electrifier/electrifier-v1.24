﻿using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

public class ExplorerBrowserItem
{
    private static readonly BitmapImage DefaultFileImage = new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico"));
    private static readonly BitmapImage DefaultFolderImage = new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico"));
    private static readonly BitmapImage DefaultLibraryImage = new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Library.ico"));

    public string DisplayName
    {
        get; set;
    }
    public bool IsFolder
    {
        get; private init;
    }
    public ShellItem ShellItem
    {
        get;
    }
    public bool HasUnrealizedChildren
    {
        get;
        private set;
    }
    public ImageSource ImageIconSource
    {
        get;
    }
    private bool IsEnumerated
    {
        get; set;
    }

    private bool IsExpanded
    {
        get; set;
    }

    public ExplorerBrowser Owner
    {
        get;
    }

    public List<ExplorerBrowserItem> Children;


    public ExplorerBrowserItem(ExplorerBrowser owner, ShellItem shItem, string? overrideDisplayName = default)
    {
        Owner = owner;
        ShellItem = shItem ?? throw new ArgumentNullException(nameof(shItem));
        Children = new List<ExplorerBrowserItem>();
        DisplayName = overrideDisplayName ?? (ShellItem.Name ?? throw new Exception("shItem Display Name"));
        IsFolder = shItem.IsFolder;

        ImageIconSource = shItem.IsFolder
            ? DefaultFolderImage
            : DefaultFileImage;
    }

    internal IEnumerable<ShellItem> EnumerateChildren(ShellItem enumerationShellItem, FolderItemFilter filter)
    {
        IsEnumerated = false;

        return enumerationShellItem is not ShellFolder folder
            ? Enumerable.Empty<ShellItem>()
            : folder.EnumerateChildren(filter);

        // TODO:
        //  IsEnumerated = true;
        //  HasUnrealizedChildren = false;
    }

    public List<ExplorerBrowserItem> GetChildItems(ShellItem enumerationShellItem)
    {
        var children = EnumerateChildren(enumerationShellItem, filter: FolderItemFilter.Storage);

        // TODO:
        //  IsEnumerated = true;
        //  HasUnrealizedChildren = false;

        return children.Select(shellItem => new ExplorerBrowserItem(this.Owner, shellItem)).ToList();
    }

    //internal static ExplorerBrowserItem? Parent(ShellItem shItem)
    //{
    //    if (shItem is null)
    //    {
    //        throw new ArgumentNullException(nameof(shItem));
    //    }

    //    if (shItem.Parent is null)
    //    {
    //        return null;
    //    }

    //    return new ExplorerBrowserItem(shItem.Parent, overrideDisplayName: "..")
    //    {
    //        IsFolder = true,
    //    };
    //}

}