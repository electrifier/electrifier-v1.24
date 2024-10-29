using System.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vanara.PInvoke;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using System.Windows.Input;
using CommunityToolkit.WinUI.Collections;
using electrifier.Controls.Vanara.Services;
using Vanara.Windows.Shell;
using Visibility = Microsoft.UI.Xaml.Visibility;

namespace electrifier.Controls.Vanara;

// https://github.com/dahall/Vanara/blob/master/Windows.Forms/Controls/ExplorerBrowser.cs

/* todo: Use Visual States for Errors, Empty folders, Empty Drives */
public sealed partial class ExplorerBrowser : INotifyPropertyChanged
{
    /// <summary>
    /// HResult code for <code><see cref="System.Runtime.InteropServices.COMException"/> 0x80070490</code>
    /// TODO: Add this to Vanara... https://github.com/dahall/Vanara/issues/490
    /// <remarks>Fired when `Element not found`</remarks>
    /// </summary>
    public HRESULT HResultElementNotFound = 0x80070490;

    private readonly ShellFolder HomeShellFolder = new("shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}");


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
    /// <summary>Current Folder content Items.</summary>
    public ObservableCollection<ExplorerBrowserItem> CurrentFolderItems
    {
        get => (ObservableCollection<ExplorerBrowserItem>)GetValue(CurrentFolderItemsProperty);
        set => SetValue(CurrentFolderItemsProperty, value);
    }
    public static readonly DependencyProperty CurrentFolderItemsProperty = DependencyProperty.Register(
        nameof(CurrentFolderItems),
        typeof(ObservableCollection<ExplorerBrowserItem>),
        typeof(ExplorerBrowser),
        new PropertyMetadata(null,
            new PropertyChangedCallback(OnCurrentFolderItemsChanged)));
    private static void OnCurrentFolderItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        //ImageWithLabelControl iwlc = d as ImageWithLabelControl; //null checks omitted
        var s = e.NewValue; //null checks omitted
        Debug.Print($".OnCurrentFolderItemsChanged(): {s}");
    }

    private AdvancedCollectionView _advancedCollectionView;

    public int ItemCount
    {
        get => (int)GetValue(ItemCountProperty);
        set => SetValue(ItemCountProperty, value);
    }
    public static readonly DependencyProperty ItemCountProperty = DependencyProperty.Register(
        nameof(ItemCount),
        typeof(int),
        typeof(ExplorerBrowser),
        new PropertyMetadata(0));
    public int FileCount
    {
        get => (int)GetValue(FileCountProperty);
        set => SetValue(FileCountProperty, value);
    }
    public static readonly DependencyProperty FileCountProperty = DependencyProperty.Register(
        nameof(FileCount),
        typeof(int),
        typeof(ExplorerBrowser),
        new PropertyMetadata(0));
    public int FolderCount
    {
        get => (int)GetValue(FolderCountProperty);
        set => SetValue(FolderCountProperty, value);
    }
    public static readonly DependencyProperty FolderCountProperty = DependencyProperty.Register(
        nameof(FolderCount),
        typeof(int),
        typeof(ExplorerBrowser),
        new PropertyMetadata(0));
    public string NavigationFailure
    {
        get => (string)GetValue(NavigationFailureProperty);
        set => SetValue(NavigationFailureProperty, value);
    }
    public static readonly DependencyProperty NavigationFailureProperty = DependencyProperty.Register(
        nameof(NavigationFailure),
        typeof(string),
        typeof(ExplorerBrowser),
        new PropertyMetadata(string.Empty));
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (value == _isLoading)
            {
                return;
            }

            _isLoading = value;
            OnPropertyChanged();
        }
    }
    public Visibility GridViewVisibility
    {
        get => _gridViewVisibility;
        set
        {
            if (value == _gridViewVisibility)
            {
                return;
            }

            _gridViewVisibility = value;
            OnPropertyChanged();
        }
    }
    public Visibility TreeViewVisibility
    {
        get => (Visibility)GetValue(TreeViewVisibilityProperty);
        set => SetValue(TreeViewVisibilityProperty, value);
    }
    public static readonly DependencyProperty TreeViewVisibilityProperty = DependencyProperty.Register(
        nameof(TreeViewVisibility),
        typeof(Visibility),
        typeof(ExplorerBrowser),
        new PropertyMetadata(default(object)));
    public Visibility TopCommandBarVisibility
    {
        get => _topCommandBarVisibility;
        set
        {
            if (value == _topCommandBarVisibility)
            {
                return;
            }

            _topCommandBarVisibility = value;
            OnPropertyChanged();
        }
    }
    public Visibility BottomAppBarVisibility
    {
        get => _bottomAppBarVisibility;
        set
        {
            if (value == _bottomAppBarVisibility)
            {
                return;
            }

            _bottomAppBarVisibility = value;
            OnPropertyChanged();
        }
    }
    public Visibility BottomCommandBarVisibility
    {
        get => _bottomCommandBarVisibility;
        set
        {
            if (value == _bottomCommandBarVisibility)
            {
                return;
            }

            _bottomCommandBarVisibility = value;
            OnPropertyChanged();
        }
    }
    public ICommand RefreshViewCommand
    {
        get;
    }
    private SoftwareBitmapSource? _defaultFolderImageBitmapSource;
    private SoftwareBitmapSource? _defaultDocumentAssocImageBitmapSource;
    /// <summary>Raises the <see cref="NavigationFailed"/> event.</summary>
    internal void OnNavigationFailed(ExtNavigationFailedEventArgs? nfevent)
    {
        if (nfevent?.FailedLocation is null)
        {
            return;
        }

        NavigationFailed?.Invoke(this, nfevent);
    }
    /// <summary>Fires when either a Navigating listener cancels the navigation, or if the operating system determines that navigation is not possible.</summary>
    public event EventHandler<ExtNavigationFailedEventArgs>? NavigationFailed;
    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;

        CurrentFolderItems = [];
        CurrentFolderBrowserItem = new ExplorerBrowserItem(HomeShellFolder);

        _advancedCollectionView = new(CurrentFolderItems, true);

        NavigationFailed += ExplorerBrowser_NavigationFailed;

        ShellTreeView.NativeTreeView.SelectionChanged += ShellTreeView_SelectionChanged;
        ShellGridView.NativeGridView.SelectionChanged += NativeGridView_SelectionChanged;

        RefreshViewCommand = new RelayCommand(() => OnRefreshViewCommand(this, new RoutedEventArgs()));

        this.Loading += async (sender, args) =>
        {
            await InitializeViewModel();
        };

        this.Loaded += async (sender, args) =>
        {
            if (CurrentFolderBrowserItem != null)
            {
                await _stockIconTask;
                Navigate(CurrentFolderBrowserItem);
            }
        };
    }

    private Task _stockIconTask;

    private async Task InitializeViewModel()
    {
        _stockIconTask = InitializeStockIcons();

        var rootItems = new List<ExplorerBrowserItem>
        {
            new ExplorerBrowserItem(HomeShellFolder),

            // todo: add home folder
            // todo: add Gallery
            Shell32FolderService.KnownFolderItem(Shell32.KNOWNFOLDERID.FOLDERID_OneDrive),
            // todo: add separator
            Shell32FolderService.KnownFolderItem(Shell32.KNOWNFOLDERID.FOLDERID_Desktop),
            Shell32FolderService.KnownFolderItem(Shell32.KNOWNFOLDERID.FOLDERID_Downloads),
            Shell32FolderService.KnownFolderItem(Shell32.KNOWNFOLDERID.FOLDERID_Documents),
            Shell32FolderService.KnownFolderItem(Shell32.KNOWNFOLDERID.FOLDERID_Pictures),
            Shell32FolderService.KnownFolderItem(Shell32.KNOWNFOLDERID.FOLDERID_Music),
            Shell32FolderService.KnownFolderItem(Shell32.KNOWNFOLDERID.FOLDERID_Videos),
            Shell32FolderService.KnownFolderItem(Shell32.KNOWNFOLDERID.FOLDERID_ComputerFolder),
            Shell32FolderService.KnownFolderItem(Shell32.KNOWNFOLDERID.FOLDERID_NetworkFolder),
            // todo: add separator new(ExplorerBrowserItemSeparator());
            Shell32FolderService.KnownFolderItem(Shell32.KNOWNFOLDERID.FOLDERID_ThisPCDesktop), // todo: WARN: Check why this leads to `SyncCenter`?
        };

        ShellTreeView.ItemsSource = rootItems;

        // todo: CurrentFolderBrowserItem = initialTarget; => OnLoaded()
    }
    /// <summary>
    /// <see href="https://github.com/dahall/Vanara/blob/ac0a1ac301dd4fdea9706688dedf96d596a4908a/Windows.Shell.Common/StockIcon.cs"/>
    /// </summary>
    /// <returns></returns>
    public async Task InitializeStockIcons()
    {
        try
        {
            using var siFolder = new StockIcon(Shell32.SHSTOCKICONID.SIID_FOLDER);
            {
                var idx = siFolder.SystemImageIndex;
                var icnHandle = siFolder.IconHandle.ToIcon();
                var bmpSource = GetWinUi3BitmapSourceFromIcon(icnHandle);
                _defaultFolderImageBitmapSource = await bmpSource;
            }

            using var siDocument = new StockIcon(Shell32.SHSTOCKICONID.SIID_DOCNOASSOC);
            {
                var idx = siDocument.SystemImageIndex;
                var icnHandle = siDocument.IconHandle.ToIcon();
                var bmpSource = GetWinUi3BitmapSourceFromIcon(icnHandle);
                _defaultDocumentAssocImageBitmapSource = await bmpSource;
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    private void ExplorerBrowser_NavigationFailed(object? sender, ExtNavigationFailedEventArgs e)
    {
        var location = e.FailedLocation;

        NavigationFailure = $"Navigation failed: '{location}' cannot be navigated to. <Show More Info> <Report a Bug>";
        NavigationFailedInfoBar.IsOpen = true;
        NavigationFailedInfoBar.Message = NavigationFailure;
        var childElement = new TextBox();
        NavigationFailedInfoBar.Content = childElement;
        IsLoading = false;
        e.IsHandled = true;
    }

    public void ExtractChildItems(ExplorerBrowserItem? targetFolder)
    {
        Debug.Print($".ExtractChildItems(<{targetFolder?.DisplayName}>) extracting...");

        if (targetFolder is null)
        {
            throw new ArgumentNullException(nameof(targetFolder));
        }

        try
        {
            /* var ext = new ShellIconExtractor(new ShellFolder(targetFolder.ShellItem));
               ext.Complete += ShellIconExtractorComplete;
               ext.IconExtracted += ShellIconExtractorIconExtracted;
               ext.Start(); */
            Debug.Assert(targetFolder.ShellItem.PIDL != Shell32.PIDL.Null);
            var shItemId = targetFolder.ShellItem.PIDL;
            using var shFolder = new ShellFolder(shItemId);

            if ((shFolder.Attributes & ShellItemAttribute.Removable) != 0)
            {
                // TODO: Check for Disc in Drive, fail only if device not present
                // TODO: Add `Eject-Buttons` to TreeView (right side, instead of TODO: Pin header) and GridView
                Debug.WriteLine($"GetChildItems: IsRemovable = true");
                var eventArgs = new NavigationFailedEventArgs();
                return;
            }

            //var ext = new ShellIconExtractor(new ShellFolder(targetFolder.ShellItem));
            //ext.Complete += ShellIconExtractorComplete;
            //ext.IconExtracted += ShellIconExtractorIconExtracted;
            //ext.Start();

            var children = shFolder.EnumerateChildren(FolderItemFilter.Folders | FolderItemFilter.NonFolders);
            var shellItems = children as ShellItem[] ?? children.ToArray();
            //itemCount = shellItems.Length;
            targetFolder.Children = []; // TODO: new ReadOnlyDictionary<ExplorerBrowserItem, int>();

            if (shellItems.Length > 0)
            {
                foreach (var shItem in shellItems)
                {
                    var ebItem = new ExplorerBrowserItem(shItem);
                    if (ebItem.IsFolder)
                    {
                        ebItem.BitmapSource = _defaultFolderImageBitmapSource;
                        targetFolder.Children?.Insert(0, ebItem);
                    }
                    else
                    {
                        ebItem.BitmapSource = _defaultDocumentAssocImageBitmapSource;
                        targetFolder.Children?.Add(ebItem);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        Debug.Print($".ExtractChildItems(<{targetFolder?.DisplayName}>) extracted: {ItemCount} items: {FileCount} files, {FolderCount} folders");
    }


    private bool _isLoading;
    private Visibility _gridViewVisibility;
    private Visibility _topCommandBarVisibility;
    private Visibility _bottomAppBarVisibility;
    private Visibility _bottomCommandBarVisibility;
    private void ShellTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        try
        {
            var ebItem = (ExplorerBrowserItem?)args.AddedItems.FirstOrDefault()!;

            if (ebItem is not ExplorerBrowserItem)
            {
                Debug.Print($".ShellTreeView_SelectionChanged({args})");
                return;
            }

            Debug.Print($".ShellTreeView_SelectionChanged({ebItem.DisplayName});");

            // todo: If ebItem.PIDL.Compare(CurrentFolderBrowserItem.ShellItem.PIDL) => Just Refresh();
            // todo: Use CONSTANTS from ExplorerBrowser if possible
            Navigate(ebItem, selectTreeViewNode: false);
            // todo: add extension methods:
            // Navigate().ThrowIfFailed;
            // Navigate().InitialFolder();
        }
        catch (Exception e)
        {
            // todo: fire navigation failed event. Handle `IsHandled` before throwing
            throw new ArgumentOutOfRangeException($"AddedItem is not type {nameof(ExplorerBrowserItem)}")
            {
                HelpLink = null,    // todo: link to github bug report
                HResult = 0,    // todo: hresult treeview seelection failed
                Source = $"{typeof(ExplorerBrowser)}",
            };
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

                Navigate(ebItem, selectTreeViewNode: true);

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
    public void Navigate(ExplorerBrowserItem ebItem, bool selectTreeViewNode = true)
    {
        try
        {
            Debug.Print($".Navigate(`{ebItem.DisplayName}`)");
            CurrentFolderBrowserItem = ebItem;
            if (selectTreeViewNode)
            {
                ebItem.IsSelected = true;
            }
            CurrentFolderItems.Clear();
            IsLoading = true;
            ExtractChildItems(ebItem);

            if (!(ebItem.Children?.Count > 0))
            {
                return;
            }

            foreach (var childItem in ebItem.Children)
            {
                CurrentFolderItems.Add(childItem);
            }
        }
        catch (COMException comEx)
        {
            var navFailedEventArgs = new ExtNavigationFailedEventArgs();
            navFailedEventArgs.Hresult = comEx.HResult;
            navFailedEventArgs.FailedLocation = ebItem.ShellItem;

            if (comEx.HResult == HResultElementNotFound)
            {
                Debug.WriteLine($"[Error] {comEx.HResult}: {navFailedEventArgs}");
                //NavigationFailure = msg;
                //HasNavigationFailure = true;
                navFailedEventArgs.IsHandled = false;

                OnNavigationFailed(navFailedEventArgs);

                if (navFailedEventArgs.IsHandled)
                {
                    return;
                }
            }

            Debug.Fail($"[Error] Navigate(<{ebItem}>) failed. COMException: {comEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Debug.Fail($"[Error] Navigate(<{ebItem}>) failed. Exception: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Taken from <see href="https://stackoverflow.com/questions/76640972/convert-system-drawing-icon-to-microsoft-ui-xaml-imagesource"/>
    /// See also <see href="https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.imaging.bitmapimage?view=windows-app-sdk-1.6"/>, which can deal with .ico natively.
    /// </summary>
    /// <param name="bitmapIcon"></param>
    /// <returns></returns>
    public static async Task<SoftwareBitmapSource?> GetWinUi3BitmapSourceFromIcon(Icon bitmapIcon)
    {
        ArgumentNullException.ThrowIfNull(bitmapIcon);

        return await GetWinUi3BitmapSourceFromGdiBitmap(bitmapIcon.ToBitmap());
    }

    /// <summary>
    /// Taken from <see href="https://stackoverflow.com/questions/76640972/convert-system-drawing-icon-to-microsoft-ui-xaml-imagesource"/>.
    /// See also <see href="https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.image.source?view=windows-app-sdk-1.5#microsoft-ui-xaml-controls-image-source"/>.
    /// See also <see href="https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.imaging.bitmapimage?view=windows-app-sdk-1.6"/>, which can deal with .ico natively.
    /// </summary>
    /// <param name="gdiBitmap"></param>
    /// <returns></returns>
    public static async Task<SoftwareBitmapSource?> GetWinUi3BitmapSourceFromGdiBitmap(Bitmap gdiBitmap)
    {
        ArgumentNullException.ThrowIfNull(gdiBitmap);

        // get pixels as an array of bytes
        var data = gdiBitmap.LockBits(new System.Drawing.Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, gdiBitmap.PixelFormat);
        var bytes = new byte[data.Stride * data.Height];
        Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
        gdiBitmap.UnlockBits(data);

        // get WinRT SoftwareBitmap
        var softwareBitmap = new Windows.Graphics.Imaging.SoftwareBitmap(
            Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8,
            gdiBitmap.Width,
            gdiBitmap.Height,
            Windows.Graphics.Imaging.BitmapAlphaMode.Premultiplied);
        softwareBitmap.CopyFromBuffer(bytes.AsBuffer());

        // build WinUI3 SoftwareBitmapSource
        var source = new SoftwareBitmapSource();
        await source.SetBitmapAsync(softwareBitmap);
        return source;
    }

    public void OnRefreshViewCommand(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine($".OnRefreshViewCommand(sender <{sender}>, RoutedEventArgs <{e.ToString()}>)");
        /* // TODO: TryNavigate(CurrentFolderBrowserItem); */
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

/// <summary>Extended Event argument for the <see cref="NavigationFailedEventArgs"/> event</summary>
public class ExtNavigationFailedEventArgs : NavigationFailedEventArgs
{
    public bool IsHandled
    {
        get; set;
    }
    public HRESULT? Hresult
    {
        get;
        set;
    }
}

/// <summary>Event argument for The Navigated event</summary>
public class ExtNavigatedEventArgs : NavigatedEventArgs
{
    public int ItemCount { get; set; } = 0;
    public int FolderCount { get; set; } = 0;
    public int FileCount { get; set; } = 0;

    /// <summary>Initializes a new instance of the <see cref="T:Vanara.Windows.Shell.NavigatedEventArgs" /> class.</summary>
    /// <param name="folder">The folder.</param>
    public ExtNavigatedEventArgs(ShellFolder folder) : base(folder)
    {
        //NewLocation = folder;   // TODO: ?!?
    }
}

