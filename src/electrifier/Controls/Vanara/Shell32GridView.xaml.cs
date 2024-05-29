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

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed partial class Shell32GridView : UserControl
{
    public ObservableCollection<Shell32GridViewItem> GridShellItems
    {
        get;
        private set;
    } = new();

    private readonly ShellFolder CurrentFolder = new(@"c:\");
    public FolderItemFilter Filter { get; private set; } = FolderItemFilter.Folders | FolderItemFilter.NonFolders;

    private readonly HWND windowHandle = default;


    public Shell32GridView(/* hwnd */)
    {
        InitializeComponent();
        DataContext = this;

        //ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));


        // TODO: make this async
        GridShellItems = Navigate(CurrentFolder, FolderItemFilter.Folders | FolderItemFilter.NonFolders);
    }

    public ObservableCollection<Shell32GridViewItem> Navigate(ShellFolder folder, FolderItemFilter filter)
    {
        var items = new ObservableCollection<Shell32GridViewItem>(Array.Empty<Shell32GridViewItem>());

        if (folder is null) { throw new ArgumentNullException(nameof(folder)); }

        var parentItem = Shell32GridViewItem.Parent(folder);
        if (parentItem is not null) { items.Add(parentItem); }

        foreach (var item in folder.EnumerateChildren(filter: filter, parentWindow: windowHandle))
        {
            items.Add(new Shell32GridViewItem(item));
        }

        return items;
    }

    private void OnItemClickHandler(object _, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Shell32GridViewItem item)
        {
            if (item.ShellItem is ShellFolder folder /*&& folder.Parent is not null*/)
            {
                GridShellItems = Navigate(folder, Filter);
            }
        }
    }

    private string GetDebuggerDisplay()
    {
        return nameof(Shell32GridView) + ToString();
    }
}
