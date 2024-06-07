using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Vanara.Windows.Shell;

// TODO: For EnumerateChildren-Calls, add HWND handle
// TODO: See ShellItemCollection, perhaps use this instead of ObservableCollection
// https://github.com/dahall/Vanara/blob/master/Windows.Shell.Common/ShellObjects/ShellItemArray.cs

namespace electrifier.Controls.Vanara;

public sealed partial class Shell32TreeView : UserControl
{
    //public readonly ObservableCollection<ExplorerBrowserItem> RootShellItems;

    public Shell32TreeView()
    {
        InitializeComponent();
        DataContext = this;

        this.Loaded += OnLoaded;

        //RootShellItems = new ObservableCollection<ExplorerBrowserItem>
        //{
        //    new(ShellFolder.Desktop)
        //};

    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // TODO: Add root items using an event handler

        //CurrentFolder = RootShellItems.FirstOrDefault();
        //CurrentFolder.IsExpanded = true;
    }

    public void SetItemsSource(ShellItem rootItem, ObservableCollection<ExplorerBrowserItem> itemSourceCollection)
    {
        // TODO: add rootItem
        var acv = new AdvancedCollectionView(itemSourceCollection, true)
        {
            Filter = x => ((ExplorerBrowserItem)x).IsFolder
        };
        acv.SortDescriptions.Add(new SortDescription("DisplayName", SortDirection.Ascending));

        TreeView.ItemsSource = acv;
    }
}
