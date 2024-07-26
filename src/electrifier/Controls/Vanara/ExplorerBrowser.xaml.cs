using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.Windows.Shell;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml.Media;
using Vanara.PInvoke;
using Microsoft.UI.Xaml.Controls.Primitives;
using Visibility = Microsoft.UI.Xaml.Visibility;

namespace electrifier.Controls.Vanara;

// TODO: INFO: See also https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.image.source?view=windows-app-sdk-1.5#microsoft-ui-xaml-controls-image-source

// https://github.com/dahall/Vanara/blob/master/Windows.Forms/Controls/ExplorerBrowser.cs
// TODO: See also https://github.com/dahall/Vanara/blob/ac0a1ac301dd4fdea9706688dedf96d596a4908a/Windows.Shell.Common/StockIcon.cs
public sealed partial class ExplorerBrowser : INotifyPropertyChanged
{
    // TODO: Use shell32 stock icons
    internal static readonly BitmapImage DefaultFileImage =
        new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico"));

    internal static readonly BitmapImage DefaultFolderImage =
        new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico"));

    internal static readonly BitmapImage DefaultLibraryImage =
        new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Library.ico"));

    /// <summary>
    /// 
    /// </summary>
    public ExplorerBrowserItem? CurrentFolderBrowserItem
    {
        get => GetValue(CurrentFolderBrowserItemProperty) as ExplorerBrowserItem;
        set => SetValue(CurrentFolderBrowserItemProperty, value);
    }
    public static readonly DependencyProperty CurrentFolderBrowserItemProperty = DependencyProperty.Register(
        nameof(CurrentFolderBrowserItem),
        typeof(ObservableCollection<ExplorerBrowserItem>),
        typeof(ExplorerBrowser),
        new PropertyMetadata(null, new PropertyChangedCallback(OnCurrentFolderBrowserItemChanged))
    );
    private static void OnCurrentFolderBrowserItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        //ImageWithLabelControl iwlc = d as ImageWithLabelControl; //null checks omitted
        var s = e.NewValue; //null checks omitted
        if (s is ExplorerBrowserItem ebItem)
        {
            Debug.WriteLine($".OnCurrentFolderBrowserItemChanged(<'{ebItem.DisplayName}'>) DependencyObject <'{d.ToString()}'>");
        }
        else
        {
            Debug.WriteLine($"[E].OnCurrentFolderBrowserItemChanged(): `{s.ToString()}` -> ERROR:UNKNOWN TYPE! Should be <ExplorerBrowserItem>");
        }
    }

    /// <summary>
    /// Represents the current's folder content.
    /// Each Item is an <see cref="ExplorerBrowserItem"/>.
    /// It is then used as DataSource of <see cref="ShellGridView"/>.
    /// </summary>
    public ObservableCollection<ExplorerBrowserItem> CurrentFolderItems
    {
        get => (ObservableCollection<ExplorerBrowserItem>)GetValue(CurrentFolderItemsProperty);
        set => SetValue(CurrentFolderItemsProperty, value);
    }
    private static void OnCurrentFolderItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        //ImageWithLabelControl iwlc = d as ImageWithLabelControl; //null checks omitted
        var s = e.NewValue; //null checks omitted
        Debug.Print($".OnCurrentFolderItemsChanged(): {s}");
    }

    private ShellIconExtractor? _iconExtractor;
    public static readonly DependencyProperty CurrentFolderItemsProperty = DependencyProperty.Register(nameof(CurrentFolderItems), typeof(ObservableCollection<ExplorerBrowserItem>), typeof(ExplorerBrowser), new PropertyMetadata(null, new PropertyChangedCallback(OnCurrentFolderItemsChanged)));
    public static readonly DependencyProperty TreeViewVisibilityProperty = DependencyProperty.Register(nameof(TreeViewVisibility), typeof(Visibility), typeof(ExplorerBrowser), new PropertyMetadata(default(object)));

    public ShellIconExtractor? IconExtractor
    {
        get => _iconExtractor;
        private set
        {
            _iconExtractor?.Cancel();
            _iconExtractor = value;
        }
    }

    public ImageCache? ImageCache
    {
        get; set;
    }

    public Visibility GridViewVisibility
    {
        get; set;
    }

    public Visibility TreeViewVisibility
    {
        get => (Visibility)GetValue(TreeViewVisibilityProperty);
        set => SetValue(TreeViewVisibilityProperty, value);
    }

    public Visibility TopCommandBarVisibility
    {
        get; set;
    }

    public Visibility BottomAppBarVisibility
    {
        get; set;
    }

    public Visibility BottomCommandBarVisibility
    {
        get; set;
    }

    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;

        ImageCache = new ImageCache();
        CurrentFolderItems = [];
        CurrentFolderBrowserItem = new ExplorerBrowserItem(ShellFolder.Desktop);

        ShellTreeView.NativeTreeView.SelectionChanged += NativeTreeViewOnSelectionChanged;
        ShellGridView.NativeGridView.SelectionChanged += NativeGridView_SelectionChanged;
        //TODO: Should be ShellTreeView.SelectionChanged += ShellTreeView_SelectionChanged;

        _ = InitializeViewModel();
    }

    private async Task InitializeViewModel()
    {
        ShellGridView.DataContext = this;
        ShellTreeView.DataContext = this;

        CurrentFolderBrowserItem = new ExplorerBrowserItem(ShellFolder.Desktop);
        var rootItems = new List<ExplorerBrowserItem>
        {
            CurrentFolderBrowserItem,
        };

        var galleryFolder = new ShellFolder(Shell32.KNOWNFOLDERID.FOLDERID_PicturesLibrary);
        rootItems.Add(new ExplorerBrowserItem(galleryFolder));

        InitializeStockIcons();

        ShellTreeView.ItemsSource = rootItems;
        Navigate(CurrentFolderBrowserItem);
        //ExtractChildItems(CurrentFolderBrowserItem, null, NavigateOnIconExtractorComplete );
    }

    public void InitializeStockIcons()
    {
        var siFolder = new StockIcon(Shell32.SHSTOCKICONID.SIID_FOLDER);

        ImageIconSource iis = new ImageIconSource();
        var icnElement = iis.CreateIconElement();

        ImageIcon ii = new ImageIcon();
        

        var siVar = new StockIcon(Shell32.SHSTOCKICONID.SIID_DOCASSOC);

    }

    public void ExtractChildItems(ExplorerBrowserItem targetFolder)
    {
        var itemCount = 0;
        Debug.Print($".ExtractChildItems <{targetFolder?.DisplayName}> extracting...");
        Debug.Assert(targetFolder is not null);
        if (targetFolder is null)
        {
            throw new ArgumentNullException(nameof(targetFolder));
        }

        try
        {
            Debug.Assert(targetFolder.IsFolder);
            Debug.Assert(targetFolder.ShellItem.PIDL != Shell32.PIDL.Null);
            var shItemId = targetFolder.ShellItem.PIDL;

            using var shFolder = new ShellFolder(shItemId);
            var children = shFolder.EnumerateChildren(FolderItemFilter.Folders | FolderItemFilter.NonFolders);
            var shellItems = children as ShellItem[] ?? children.ToArray();
            itemCount = shellItems.Length;
            targetFolder.Children = new List<ExplorerBrowserItem>();

            if (shellItems.Length > 0)
            {
                foreach (var child in shellItems)
                {
                    var ebItem = new ExplorerBrowserItem(child);
                    targetFolder.Children.Add(ebItem);

                    ebItem.ImageIconSource = ebItem.IsFolder ? DefaultFolderImage : DefaultFileImage;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        Debug.Print($".ExtractChildItems <{targetFolder?.DisplayName}> extracted: {itemCount} items.");
    }

    private void RefreshButtonClick(object sender, RoutedEventArgs e)
    {
        // TODO: TryNavigate(CurrentFolderBrowserItem);
    }

    private void NativeTreeViewOnSelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        var selectedItem = args.AddedItems.FirstOrDefault();
        if (selectedItem != null)
        {
            if (selectedItem is ExplorerBrowserItem ebItem)
            {
                Debug.Print($".NativeTreeViewOnSelectionChanged() {ebItem.DisplayName}");

                Navigate(ebItem);

                // TODO: If ebItem.PIDL.Compare(CurrentFolderBrowserItem.ShellItem.PIDL) => Just Refresh()
            }
            // TODO: else
            //{
            //    Debug.Fail($"ERROR: NativeTreeViewOnSelectionChanged() addedItem {selectedItem.ToString()} is NOT of type <ExplorerBrowserItem>!");
            //    throw new ArgumentOutOfRangeException(
            //        "$ERROR: NativeTreeViewOnSelectionChanged() addedItem {selectedItem.ToString()} is NOT of type <ExplorerBrowserItem>!");
            //}
        }
    }

    private void NativeGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var addedItems = e.AddedItems;
        var newTarget = addedItems?.FirstOrDefault();
        if (newTarget == null)
        {
            Debug.Print($".NativeGridView_SelectionChanged(`<newTarget==null>`");
            return;
        }
        else
        {
            if (newTarget is ExplorerBrowserItem ebItem)
            {
                Debug.Print($".NativeGridView_SelectionChanged(`{ebItem.DisplayName}`)");

                Navigate(ebItem);

                // TODO: If ebItem.PIDL.Compare(CurrentFolderBrowserItem.ShellItem.PIDL) => Just Refresh()
            }
            // TODO: else 
            //{
            //    Debug.Fail(
            //        $"ERROR: NativeGridView_SelectionChanged() addedItem {newTarget.ToString()} is NOT of type <ExplorerBrowserItem>!");
            //    throw new ArgumentOutOfRangeException(
            //        "$ERROR: NativeGridView_SelectionChanged() addedItem {selectedItem.ToString()} is NOT of type <ExplorerBrowserItem>!");
            //}

            Debug.Print($".NativeGridView_SelectionChanged({newTarget})");
        }
    }

    public void Navigate(ExplorerBrowserItem ebItem)
    {
        var isFolder = ebItem.IsFolder;

        if (isFolder)
        {
            try
            {
                Debug.Print($".Navigate(`{ebItem.DisplayName}`)");
                CurrentFolderBrowserItem = ebItem;
                CurrentFolderItems.Clear();
                ExtractChildItems(ebItem);

                if (!(ebItem.Children?.Count > 0))
                {
                    return;
                }

                foreach(var childItem in ebItem.Children)
                {
                    CurrentFolderItems.Add(childItem);
                }
            }
            catch
            {
                Debug.Fail($"ERROR: Navigate() failed");
                throw;
            }
        }
        else
        {
            Debug.Write($"[i] Navigate(ShellItem? newTargetItem): is not a folder.");
            // TODO: try to open or execute the item
        }
    }

    #region Property stuff

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion Property stuff
}

#region The following is original copy & paste from Vanara
/// <summary>Event argument for The Navigated event</summary>
public class NavigatedEventArgs : EventArgs
{
    /// <summary>The new location of the explorer browser</summary>
    public ShellItem? NewLocation
    {
        get; set;
    }
}

/// <summary>Event argument for The Navigating event</summary>
public class NavigatingEventArgs : EventArgs
{
    /// <summary>Set to 'True' to cancel the navigation.</summary>
    public bool Cancel
    {
        get; set;
    }

    /// <summary>The location being navigated to</summary>
    public ShellItem? PendingLocation
    {
        get; set;
    }
}

/// <summary>Event argument for the NavigatinoFailed event</summary>
public class NavigationFailedEventArgs : EventArgs
{
    /// <summary>The location the browser would have navigated to.</summary>
    public ShellItem? FailedLocation
    {
        get; set;
    }
}
#endregion
