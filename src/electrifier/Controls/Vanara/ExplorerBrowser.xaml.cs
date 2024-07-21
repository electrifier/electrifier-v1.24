using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.Windows.Shell;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Media;
using Vanara.PInvoke;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace electrifier.Controls.Vanara;

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
    public static readonly DependencyProperty CurrentFolderItemsProperty = DependencyProperty.Register(
        nameof(CurrentFolderItems),
        typeof(ObservableCollection<ExplorerBrowserItem>),
        typeof(ExplorerBrowser),
        new PropertyMetadata(null, new PropertyChangedCallback(OnCurrentFolderItemsChanged))
    );
    private static void OnCurrentFolderItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        //ImageWithLabelControl iwlc = d as ImageWithLabelControl; //null checks omitted
        var s = e.NewValue; //null checks omitted
        Debug.Print($".OnCurrentFolderItemsChanged(): {s}");
    }

    private ShellIconExtractor? _iconExtractor;
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

    public Microsoft.UI.Xaml.Visibility GridViewVisibility
    {
        get; set;
    }

    public Microsoft.UI.Xaml.Visibility TopCommandBarVisibility
    {
        get; set;
    }

    public Microsoft.UI.Xaml.Visibility BottomAppBarVisibility
    {
        get; set;
    }

    public Microsoft.UI.Xaml.Visibility BottomCommandBarVisibility
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
        //var userFilesItem = new ExplorerBrowserItem(new ShellLibrary(Shell32.KNOWNFOLDERID.FOLDERID_UsersFiles));

        ShellTreeView.NativeTreeView.SelectionChanged += NativeTreeViewOnSelectionChanged;
        ShellGridView.NativeGridView.SelectionChanged += NativeGridView_SelectionChanged;
        //TODO: Should be ShellTreeView.SelectionChanged += ShellTreeView_SelectionChanged;

        _ = InitializeViewModel();
    }

    private async Task InitializeViewModel()
    {
        ShellGridView.DataContext = this;
        ShellTreeView.DataContext = this;

        var rootItems = new List<ExplorerBrowserItem?>
        {
            CurrentFolderBrowserItem,
        };

        var desktopItem = new ExplorerBrowserItem(new ShellItem(ShellFolder.Desktop.PIDL));
        rootItems.Add(desktopItem);

        await ShellTreeView.InitializeRoot(rootItems);
        ExtractChildItems(CurrentFolderBrowserItem, null, ShellIconExtractor_Complete );
    }

    public void ExtractChildItems(ExplorerBrowserItem targetFolder,
        EventHandler<ShellIconExtractedEventArgs>? iconExtOnIconExtracted,
        EventHandler? iconExtOnComplete)
    {
        Debug.Print($".ExtractChildItems<ctor> <{targetFolder?.DisplayName}> OnExtracted=<{iconExtOnIconExtracted}> OnComplete=<{nameof(iconExtOnComplete)}>");
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
            var shFolder = new ShellFolder(shItemId);
            var shellIconExtractor = new ShellIconExtractor(shFolder);
            shellIconExtractor.IconExtracted += (sender, args) =>
            {
                var shItem = new ShellItem(args.ItemID);
                var browserItem = new ExplorerBrowserItem(shItem)
                {
                    ImageIconSource = shItem.IsFolder ? DefaultFolderImage : DefaultFileImage,
                };

                DispatcherQueue.TryEnqueue(() =>
                {
                    CurrentFolderItems.Add(browserItem);
                });
            };
            //shellIconExtractor.IconExtracted += iconExtOnIconExtracted;  // TODO: Remove this stuff, throw event instead?!?
            shellIconExtractor.Complete += iconExtOnComplete;            // TODO: Remove this stuff, throw event instead?!?
            shellIconExtractor.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void ShellIconExtractor_Complete(object? sender, EventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            var itmCount = CurrentFolderItems?.Count ?? 0;
            Debug.Print($".IconExtOnComplete() = {itmCount} items for sender <{sender})>");
        });
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
            else
            {
                Debug.Fail($"ERROR: NativeTreeViewOnSelectionChanged() addedItem {selectedItem.ToString()} is NOT of type <ExplorerBrowserItem>!");
                throw new ArgumentOutOfRangeException(
                    "$ERROR: NativeTreeViewOnSelectionChanged() addedItem {selectedItem.ToString()} is NOT of type <ExplorerBrowserItem>!");
            }
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
            else
            {
                Debug.Fail(
                    $"ERROR: NativeGridView_SelectionChanged() addedItem {newTarget.ToString()} is NOT of type <ExplorerBrowserItem>!");
                throw new ArgumentOutOfRangeException(
                    "$ERROR: NativeGridView_SelectionChanged() addedItem {selectedItem.ToString()} is NOT of type <ExplorerBrowserItem>!");
            }

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
                ExtractChildItems(ebItem, null, ShellIconExtractor_Complete );

                //ExtractChildItems(newTargetItem, null, ShellIconExtractor_Complete );

                //ExtractChildItems(CurrentFolderBrowserItem, null, ShellIconExtractor_Complete);
            }
            catch
            {
                Debug.Fail($"ERROR: Navigate() failed");
                throw;
            }
        }
        else
        {
            Debug.Write($".info Navigate(ShellItem? newTargetItem): is not a folder.");
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
