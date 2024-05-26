using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using Windows.Foundation.Collections;
using Windows.Foundation;

namespace electrifier.Controls.Vanara;

public sealed partial class Shell32GridView : UserControl
{
    public readonly ObservableCollection<Shell32GridViewItem> GridShellItems;

    private readonly ShellItem CurrentFolder;
    public FolderItemFilter Filter { get; private set; } = FolderItemFilter.Folders | FolderItemFilter.NonFolders;

    public Shell32GridView()
    {
        InitializeComponent();
        GridShellItems = new ObservableCollection<Shell32GridViewItem>();


        CurrentFolder = new ShellFolder(@"c:\");
        //        GridShellItems.Add(new Shell32GridViewItem(ShellFolder.Desktop));



        // make this async
        Navigate(ShellFolder.Desktop, FolderItemFilter.Folders | FolderItemFilter.NonFolders);


        //this.OnLoaded = (sender, e) =>
        //{
        //    Navigate(ShellFolder.Desktop);
        //};



    }

    public IEnumerable<Shell32GridViewItem> Navigate(ShellFolder folder, FolderItemFilter filter)
    {
        if (CurrentFolder is null)
        {
            throw new ArgumentNullException(nameof(CurrentFolder));
        }

        if (CurrentFolder is not ShellFolder)
        {
            return Array.Empty<Shell32GridViewItem>();
        }

        var items = EnumerateItems(CurrentFolder, filter);

        //: ((ShellFolder)CurrentFolder).EnumerateChildren(filter);


        //var children = ((ShellFolder)CurrentFolder).EnumerateChildren(filter);
        // 
        if (CurrentFolder.Parent is not null)
        {
            var parent = Shell32GridViewItem.Shell32GridViewParentItem(CurrentFolder);
            items.Add(parent);
        }

        // Shell32GridViewParentItem 
        return items;
    }

    public List<Shell32GridViewItem> EnumerateItems(ShellItem shFolder, FolderItemFilter filter = (FolderItemFilter.Folders | FolderItemFilter.NonFolders))
    {
        var items = new List<Shell32GridViewItem>();

        try
        {
            //if (shFolder is not ShellFolder)
            //{
            //    return Array.Empty<Shell32GridViewItem>();
            //}

            //GridShellItems.Clear();
            //CurrentFolder = shFolder;

            //foreach (var item in shFolder.EnumerateChildren())
            //{
            //    GridShellItems.Add(new Shell32GridViewItem(item));
            //}
        }
        catch (Exception)
        {
            throw;
        }

        //GridShellItems.Clear();
        //CurrentFolder = shFolder;

        //if (CurrentFolder is not ShellFolder)
        //{
        //    return Array.Empty<Shell32GridViewItem>();
        //}

        //foreach (var item in shFolder.EnumerateChildren())
        //{
        //    GridShellItems.Add(new Shell32GridViewItem(item));
        //}
        return items;
    }



    void OnItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Shell32GridViewItem item)
        {
            if (item.ShellItem is ShellFolder folder)
            {
                Navigate(folder, this.Filter);
            }
        }
    }
}





/// <summary>
/// Represents a single Item for the Shell32GridView
/// </summary>
/// <param name="shItem"></param>
public record Shell32GridViewItem(ShellItem shItem)
{
    public string DisplayName
    {
        get; private set;
    }

    public bool IsFolder
    {
        get; private set;
    }

    public ShellItem ShellItem
    {
        get;
    }

    public Shell32GridViewItem() : this(ShellFolder.Desktop)
    {
        var shItem = ShellFolder.Desktop ?? throw new Exception("Desktop");
        DisplayName = shItem.Name ?? throw new Exception(nameof(DisplayName));

        //IsFolder = shItem is ShellFolder;
        //ParsingName = shItem.ParsingName;
    }


    internal Shell32GridViewItem(ShellItem shItem, string displayName) : this(shItem)
    {
        DisplayName = displayName;
        //ShellItem = shItem ?? throw new ArgumentNullException(nameof(shItem));
        //DisplayName = shItem.Name;
        //IsFolder = shItem is ShellFolder;
        //ParsingName = shItem.ParsingName;
    }

    internal static Shell32GridViewItem Shell32GridViewParentItem(ShellItem shItem)
    {
        if (shItem is null)
        {
            throw new ArgumentNullException(nameof(shItem));
        }

        if (shItem.Parent is null)
        {
            throw new ArgumentNullException(nameof(shItem.Parent));
        }

        var newItem = new Shell32GridViewItem(shItem.Parent);
        newItem.DisplayName = "Parent Folder";
        newItem.IsFolder = true;
        //ParsingName = shItem.Parent.ParsingName;

        return newItem;
    }
}
