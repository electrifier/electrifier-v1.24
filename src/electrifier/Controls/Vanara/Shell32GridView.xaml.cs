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
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Collections;

namespace electrifier.Controls.Vanara;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public sealed partial class Shell32GridView : UserControl
{
    public ObservableCollection<Shell32GridViewItem> GridShellItems
    {
        get;
        private set;
    }

    public FolderItemFilter Filter
    {
        get;
        private set;
    } = FolderItemFilter.Folders | FolderItemFilter.NonFolders;

    public ShellItem? CurrentFolder
    {
        get => default;
        internal set => Navigate(value);
    }

    private readonly HWND windowHandle = default;

    public Shell32GridView(/* hwnd */)
    {
        InitializeComponent();
        DataContext = this;
        GridShellItems = new ObservableCollection<Shell32GridViewItem>();

        CurrentFolder = ShellFolder.Desktop;

        //var collection = new IncrementalLoadingCollection<GridViewItemsSource, GridViewItem>();
        //PeopleListView.ItemsSource = collection;
    }

    public void Navigate(ShellItem? targetItem /*, FolderItemFilter? filter */)
    {
        if (targetItem is null)
        {
            GridShellItems = new ObservableCollection<Shell32GridViewItem>();
            return;
        }

        var newEnumerateItems =
            EnumerateItems(targetItem, Filter);

        GridShellItems = newEnumerateItems;
    }

    private ObservableCollection<Shell32GridViewItem> EnumerateItems(ShellItem navigationTarget, FolderItemFilter filter)
    {
        if (navigationTarget is null) { throw new ArgumentNullException(nameof(navigationTarget)); }

        using var newTarget = navigationTarget as ShellFolder;
        var items = new ObservableCollection<Shell32GridViewItem>(Array.Empty<Shell32GridViewItem>());
        if (newTarget is null) { return items; }

        var parentItem = Shell32GridViewItem.Parent(navigationTarget);
        if (parentItem is not null) { items.Add(parentItem); }

        // TODO: make this async
        var enumeratedChildren =
            newTarget.EnumerateChildren(filter: filter, parentWindow: windowHandle)
            .OrderBy(item => (item.Attributes & ShellItemAttribute.Folder) == 0)
            .ThenBy(item => item.Name);

        foreach (var shItem in enumeratedChildren)
        {
            items.Add(new Shell32GridViewItem(shItem));
        }

        return items;
    }

    private void OnItemClickHandler(object _, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not Shell32GridViewItem gridViewItem)
        {
            return;
        }

        // TODO: fire event to request navigation
        Navigate(gridViewItem.ShellItem);
    }

    private string GetDebuggerDisplay()
    {
        return nameof(Shell32GridView) + ToString();
    }

    public class GridViewItemsSource : IIncrementalSource<Shell32GridViewItem>
    {
        private readonly List<Shell32GridViewItem> gridViewItems;

        public GridViewItemsSource()
        {
            // Creates an example collection.
            gridViewItems = new List<Shell32GridViewItem>();

            for (var i = 1; i <= 200; i++)
            {
                var p = new Shell32GridViewItem(ShellFolder.Desktop);  // { Name = "Person " + i }
                gridViewItems.Add(p);
            }
        }

        public async Task<IEnumerable<Shell32GridViewItem>> GetPagedItemsAsync(int pageIndex, int pageSize)
        {
            // Gets items from the collection according to pageIndex and pageSize parameters.
            var result = (from p in gridViewItems
                          select p).Skip(pageIndex * pageSize).Take(pageSize);

            return result;
        }

        public async Task<IEnumerable<Shell32GridViewItem>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cToken)
        {
            // Gets items from the collection according to pageIndex and pageSize parameters.
            var result = (from p in gridViewItems
                    select p).Skip(pageIndex * pageSize)
                .Take(pageSize);

            return result;
        }
    }
}
