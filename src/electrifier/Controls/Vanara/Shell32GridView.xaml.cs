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

    public ShellItem CurrentFolder
    {
        //get => this.;
        set => Navigate(value);
    }

    private readonly HWND windowHandle = default;

    public Shell32GridView(/* hwnd */)
    {
        InitializeComponent();
        DataContext = this;
        //WindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);

        //ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));


        // TODO: make this async
        //GridShellItems = EnumerateItems(CurrentFolder, Filter);
    }

    public void Navigate(ShellItem targetItem /*, FolderItemFilter? filter */)
    {
        if (targetItem is null) { throw new ArgumentNullException(nameof(targetItem)); }
        //if (filter is null) { filter = _filter; }

        var newEnumerateItems =
            EnumerateItems(targetItem, FolderItemFilter.Storage /* TODO: , filter*/);

        this.GridShellItems = newEnumerateItems;
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
        if (e.ClickedItem is not Shell32GridViewItem item)
        {
            return;
        }

        if (item.ShellItem is ShellFolder folder /*&& folder.Parent is not null*/)
        {
            // TODO: fire event to request navigation
            //GridShellItems = Navigate(folder, Filter);
        }
    }

    private string GetDebuggerDisplay()
    {
        return nameof(Shell32GridView) + ToString();
    }
}
