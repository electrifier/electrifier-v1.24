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
using static Vanara.PInvoke.Gdi32;
using static Vanara.PInvoke.Shell32;

// TODO: For EnumerateChildren-Calls, add HWND handle
// TODO: See ShellItemCollection, perhaps use this instead of ObservableCollection
// https://github.com/dahall/Vanara/blob/master/Windows.Shell.Common/ShellObjects/ShellItemArray.cs

namespace electrifier.Controls.Vanara;

public sealed partial class Shell32TreeView : UserControl
{
    public readonly ObservableCollection<Shell32TreeViewItem> RootShellItems;

    public Shell32TreeView()
    {
        InitializeComponent();
        DataContext = this;

        this.Loaded += OnLoaded;

        // TODO: Add root items using an event handler
        RootShellItems = new ObservableCollection<Shell32TreeViewItem>
        {
            new(ShellFolder.Desktop)
        };

        // TODO: Add event handler for item expansion
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        IOrderedEnumerable<ShellItem> children;
        foreach (var rootShellItem in RootShellItems)
        {
            // TODO: sort children using ShellItem.Compare for sorting  // CompareTo(ShellItem)
            children = rootShellItem.EnumerateChildren(filter: FolderItemFilter.Folders)
                .OrderBy(keySelector: item => (item.Attributes & ShellItemAttribute.Browsable) != 0)
                .ThenBy(keySelector: item => item.Name);

            foreach (var shItem in children)
            {
                var tvItem = new Shell32TreeViewItem(shItem);
                //tvItem.Expanded += OnItemExpanded;
                rootShellItem.Children.Add(tvItem);
            }
        }
    }
}
public class Shell32TreeViewItem
{
    public ObservableCollection<Shell32TreeViewItem> Children
    {
        get;
    }
    public string DisplayName
    {
        get;
    }

    //public bool DisplayNameVisibility
    //{
    //    get; set;
    //}

    public IEnumerable<ShellItem> EnumerateChildren(FolderItemFilter filter)
    {
        try
        {
            return ShellItem is not ShellFolder folder
                ? Enumerable.Empty<ShellItem>()
                : folder.EnumerateChildren(filter);
        }
        finally
        {
            HasUnrealizedChildren = false;
        }
    }

    // TODO: Add observable flags for async property loading
    public bool HasUnrealizedChildren
    {
        get;
        private set;
    }
    public ShellItemImages Images => ShellItem.Images; // GetImageAsync, GetImage 
    public ShellItem ShellItem
    {
        get;
    }

    public Shell32TreeViewItem(ShellItem shItem)
    {
        ShellItem = shItem ?? throw new ArgumentNullException(nameof(shItem));
        DisplayName = ShellItem.Name ?? ShellItem.ToString();
        Children = new ObservableCollection<Shell32TreeViewItem>();
        HasUnrealizedChildren = true;

        _ = Task.Run(InitializeAsync);
    }

    public async Task InitializeAsync()
    {
        var attributes = await Task.Run(() => ShellItem.Attributes);
        var StorageCapMask = await Task.Run(() => attributes & ShellItemAttribute.StorageCapMask);

        HasUnrealizedChildren = attributes.HasFlag(ShellItemAttribute.HasSubfolder);
    }

    //public async TreeViewExpandingEventArgs
    public async Task OnItemExpanded(TreeViewExpandingEventArgs e)
    {
        if (HasUnrealizedChildren)
        {
            var children = EnumerateChildren(FolderItemFilter.Folders);
            foreach (var shItem in children)
            {
                var tvItem = new Shell32TreeViewItem(shItem);
                //tvItem.Expanded += OnItemExpanded;
                Children.Add(tvItem);
            }
        }
    }

    //async Task<Shell32TreeViewItem> GetChildAsync(ShellItem shItem)
    //{
    //    return await Task.Run(() => new Shell32TreeViewItem(shItem));
    //}

    /// <summary>
    /// Gets an image that represents this item. The default behavior is to load a thumbnail. If there is no thumbnail for the current
    /// item, it retrieves the icon of the item. The thumbnail or icon is extracted if it is not currently cached.
    /// </summary>
    /// <param name="size">A structure that specifies the size of the image to be received.</param>
    /// <param name="flags">One or more of the option flags.</param>
    /// <param name="forcePreVista">If set to <see langword="true"/>, ignore the use post vista interfaces like <see cref="IShellItemImageFactory"/>.</param>
    /// <returns>The resulting image.</returns>
    /// <exception cref="PlatformNotSupportedException"></exception>
    //public async Task<SafeHBITMAP> GetImageAsync(SIZE size, ShellItemGetImageOptions flags = 0, bool forcePreVista = false) => await TaskAgg.Run(() => GetImage(size, flags, forcePreVista), System.Threading.CancellationToken.None);

    //    public async Task<SafeHBITMAP> GetImageAsync(SIZE size, ShellItemGetImageOptions flags = 0, bool forcePreVista = false)
    //    {
    //        return ShellItem.GetImageAsync(size, flags, forcePreVista);
    //    }

}
