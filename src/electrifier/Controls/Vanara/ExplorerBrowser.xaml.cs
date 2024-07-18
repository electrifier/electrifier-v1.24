using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.Windows.Shell;
using System.Collections.ObjectModel;
using Vanara.PInvoke;

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
            Debug.WriteLine($".OnCurrentFolderBrowserItemChanged(): '{ebItem.DisplayName}' DependencyObject: '{d.ToString()}'");
        }
        else
        {
            Debug.WriteLine($".OnCurrentFolderBrowserItemChanged(): `{s.ToString()}` -> ERROR:UNKNOWN TYPE! Should be <ExplorerBrowserItem>");
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
        Debug.WriteLine($".OnCurrentFolderItemsChanged(): {s}");
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
        
        CurrentFolderBrowserItem = new ExplorerBrowserItem(ShellFolder.Desktop);
        CurrentFolderItems = new ObservableCollection<ExplorerBrowserItem>();

        _ = InitializeViewModel();
        //ShellTreeView.SelectionChanged += ShellTreeView_SelectionChanged;
    }

    private async Task InitializeViewModel()
    {
        ImageCache = new ImageCache();
        ShellGridView.DataContext = this;
        ShellTreeView.DataContext = this;

        var rootItems = new List<ExplorerBrowserItem?>
        {
            CurrentFolderBrowserItem,
        };

        var desktopItem = new ExplorerBrowserItem(new ShellItem(ShellFolder.Desktop.PIDL));
        //var userFilesItem = new ExplorerBrowserItem(new ShellLibrary(Shell32.KNOWNFOLDERID.FOLDERID_UsersFiles));
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

        Debug.Assert(targetFolder.IsFolder);
        Debug.Assert(targetFolder.ShellItem.PIDL != Shell32.PIDL.Null);
        var shItemId = targetFolder.ShellItem.PIDL;
        var shFolder = new ShellFolder(shItemId);
        var shellIconExtractor = new ShellIconExtractor(shFolder);
        shellIconExtractor.IconExtracted += (sender, args) =>
        {
            var shItem = new ShellItem(args.ItemID);
            var ebItem = new ExplorerBrowserItem(shItem);

            DispatcherQueue.TryEnqueue(() =>
            {
                CurrentFolderItems.Add(ebItem);
            });
        };
        shellIconExtractor.IconExtracted += iconExtOnIconExtracted;  // TODO: Remove this stuff, throw event instead?!?
        shellIconExtractor.Complete += ShellIconExtractor_Complete;
        shellIconExtractor.Complete += iconExtOnComplete;            // TODO: Remove this stuff, throw event instead?!?
        shellIconExtractor.Start();
    }

    private void ShellIconExtractor_Complete(object? sender, EventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            var itmCount = CurrentFolderItems?.Count ?? 0;
            Debug.Print($".IconExtOnComplete() = {itmCount} items - sender <{sender})>");
        });
    }


    private void RefreshButtonClick(object sender, RoutedEventArgs e)
    {
        // TODO: TryNavigate(CurrentFolderBrowserItem);
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
