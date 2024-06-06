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

        //this.Loaded += OnLoaded;

        // TODO: Add root items using an event handler
        RootShellItems = new ObservableCollection<Shell32TreeViewItem>
        {
            new(ShellFolder.Desktop)
        };
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        EnumerateRootItems();

        //CurrentFolder = RootShellItems.FirstOrDefault();
        //CurrentFolder.IsExpanded = true;
    }

    internal void EnumerateRootItems()
    {
        foreach (var rootShellItem in RootShellItems)
        {
            // TODO: sort children using ShellItem.Compare for sorting  // CompareTo(ShellItem)
            var children =
                rootShellItem.EnumerateChildren(filter: FolderItemFilter.Folders)
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
