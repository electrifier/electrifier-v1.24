/* todo: Use Visual States for Errors, Empty folders, Empty Drives */
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using System.Windows.Input;
using electrifier.Controls.Vanara.Services;
namespace electrifier.Controls.Vanara;
using Visibility = Microsoft.UI.Xaml.Visibility;
// https://github.com/dahall/Vanara/blob/master/Windows.Forms/Controls/ExplorerBrowser.cs
/// <summary>Replacement for <see cref="Vanara.Windows.Forms.Controls.Explorer.cs">Windows.Forms/Controls/ExplorerBrowser.cs</see></summary>
public sealed partial class ExplorerBrowser : INotifyPropertyChanged
{
    /// <summary>
    /// HResult code for <code><see cref="System.Runtime.InteropServices.COMException"/>: 0x80070490</code>
    /// <remarks>Fired when `Element not found`</remarks>
    /// </summary>
    public readonly HRESULT HResultElementNotFound = new HRESULT(0x80070490);
    public readonly ShellFolder HomeShellFolder = new("shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}");
    private Visibility _bottomAppBarVisibility;
    private Visibility _bottomCommandBarVisibility;
    private Visibility _gridViewVisibility;
    private bool _isLoading;
    private Task _stockIconTask;
    private Visibility _topCommandBarVisibility;
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
            Debug.WriteLine($".OnCurrentFolderBrowserItemChanged(<'{ebItem.DisplayName}'>) DependencyObject <'{d}'>");
        }
        else
        {
            Debug.WriteLine($"[E].OnCurrentFolderBrowserItemChanged(): `{s}` -> ERROR:UNKNOWN TYPE! Should be <ExplorerBrowserItem>");
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
    private ShellNamespaceService _nsService = new ShellNamespaceService();

    /// <summary>
    /// ExplorerBrowser Implementation for WinUI3
    /// </summary>
    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;

        CurrentFolderItems = [];
        CurrentFolderBrowserItem = new ExplorerBrowserItem(ShellFolder.Desktop.PIDL);

        _advancedCollectionView = new(CurrentFolderItems, true);

        NavigationFailed += ExplorerBrowser_NavigationFailed;

        ShellTreeView.NativeTreeView.SelectionChanged += ShellTreeView_SelectionChanged;
        ShellGridView.NativeGridView.SelectionChanged += NativeGridView_SelectionChanged;

        RefreshViewCommand = new RelayCommand(() => OnRefreshViewCommand(this, new RoutedEventArgs()));

        Loading += async (sender, args) =>
        {
            _ = InitializeViewModel();
        };

        Loaded += async (sender, args) =>
        {
            if (CurrentFolderBrowserItem == null)
            {
                return;
            }

            if (_stockIconTask != null)
            {
                await _stockIconTask;
            }

            Navigate(CurrentFolderBrowserItem);
        };
    }

    private async Task InitializeViewModel()
    {
        var rootItems = new List<ExplorerBrowserItem>
        {
            // todo: new ExplorerBrowserItem(HomeShellFolder.PIDL),
            // todo: add Gallery? is this Adobe?
            // todo: add separators
            new(new ShellFolder(Shell32.KNOWNFOLDERID.FOLDERID_OneDrive).PIDL),
            new(new ShellFolder(Shell32.KNOWNFOLDERID.FOLDERID_Downloads).PIDL),
            new(new ShellFolder(Shell32.KNOWNFOLDERID.FOLDERID_Documents).PIDL),
            new(new ShellFolder(Shell32.KNOWNFOLDERID.FOLDERID_Desktop).PIDL),
            new(new ShellFolder(Shell32.KNOWNFOLDERID.FOLDERID_Pictures).PIDL),
            new(new ShellFolder(Shell32.KNOWNFOLDERID.FOLDERID_Music).PIDL),
            new(new ShellFolder(Shell32.KNOWNFOLDERID.FOLDERID_Videos).PIDL),
            new(new ShellFolder(Shell32.KNOWNFOLDERID.FOLDERID_ComputerFolder).PIDL),
            new(new ShellFolder(Shell32.KNOWNFOLDERID.FOLDERID_NetworkFolder).PIDL),
            // todo: WARN: Check why this leads to `SyncCenter`?
            new(new ShellFolder(Shell32.KNOWNFOLDERID.FOLDERID_ThisPCDesktop).PIDL),
        };
        ShellTreeView.ItemsSource = rootItems;
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

    /// <summary>
    /// ExtractChildItems
    /// TODO: Add Stack, or ShellDataTable
    /// TODO: Pre-Enumerate slow folders while building the tree
    /// </summary>
    public static List<ExplorerBrowserItem> ExtractChildItems(ExplorerBrowserItem parentBrowserItem)
    {
        var shItem = parentBrowserItem.ShellItem;
        var result = new List<ExplorerBrowserItem>();

        if ((shItem.Attributes & ShellItemAttribute.Removable) != 0)
        {
            // TODO: Check for Disc in Drive, fail only if device not present
            // TODO: Add `Eject-Buttons` to TreeView (right side, instead of TODO: Pin header) and GridView
            Debug.WriteLine($"GetChildItems: IsRemovable = true");
            return result;
            //var eventArgs = new NavigationFailedEventArgs();
            //return Task.FromCanceled<>();
            //cancelToken.ThrowIfCancellationRequested(); 
        }

        if (!shItem.IsFolder)
        {
            return result;
        }

        try
        {
            using var shFolder = new ShellFolder(shItem);
            var children = shFolder.EnumerateChildren(FolderItemFilter.Folders | FolderItemFilter.NonFolders);
            var shellItems = children as ShellItem[] ?? children.ToArray();
            var cnt = shellItems.Length;

            if (cnt > 0)
            {
                foreach (var item in shellItems)
                {
                    var ebItem = new ExplorerBrowserItem(item.PIDL);
                    if (ebItem.IsFolder)
                    {
                        result.Insert(0, ebItem);
                    }
                    else
                    {
                        result.Add(ebItem);
                    }
                }
            }
        }
        catch (COMException comEx)
        {
            Debug.Fail(comEx.Message);
        }
        catch (Exception e)
        {
            Debug.Fail(e.Message);
        }

        return result;
    }

    private void ShellTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        try
        {
            var ebItem = (ExplorerBrowserItem?)args.AddedItems.FirstOrDefault()!;

            if (ebItem is null)
            {
                Debug.Print($".ShellTreeView_SelectionChanged({args})");
                return;
            }

            Debug.Print($".ShellTreeView_SelectionChanged({ebItem.DisplayName});");

            // todo: If ebItem.PIDL.Compare(CurrentFolderBrowserItem.ShellItem.PIDL) => Just Refresh();
            // todo: Use CONSTANTS from ExplorerBrowser if possible
            Navigate(ebItem);
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

        if (newTarget is ExplorerBrowserItem ebItem)
        {
            Debug.Print($".NativeGridView_SelectionChanged(`{ebItem.DisplayName}`)");

            Navigate(ebItem); //, selectTreeViewNode: true);

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
    public void Navigate(ExplorerBrowserItem targetBrowserItem)
    {
        try
        {
            Debug.Print($".Navigate(`{targetBrowserItem.DisplayName}`)");
            CurrentFolderBrowserItem = targetBrowserItem;
            CurrentFolderItems.Clear();
            IsLoading = true;

            var childShellItems = ExtractChildItems(targetBrowserItem);

            if (childShellItems.Count <= 0)
            {
                return;
            }

            foreach (var childItem in childShellItems)
            {
                CurrentFolderItems.Add(childItem);
            }
        }
        catch (COMException comEx)
        {
            var navFailedEventArgs = new ExtNavigationFailedEventArgs
            {
                Hresult = comEx.HResult,
                FailedLocation = targetBrowserItem.ShellItem
            };

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

            Debug.Fail($"[Error] Navigate(<{targetBrowserItem}>) failed. COMException: {comEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Debug.Fail($"[Error] Navigate(<{targetBrowserItem}>) failed. Exception: {ex.Message}");
            throw;
        }
        finally
        {
            IsLoading = false;

        }
    }

    public void OnRefreshViewCommand(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine($".OnRefreshViewCommand(sender <{sender}>, RoutedEventArgs <{e}>)");
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
/// <remarks>Initializes a new instance of the <see cref="T:Vanara.Windows.Shell.NavigatedEventArgs" /> class.</remarks>
/// <param name="folder">The folder.</param>
public class ExtNavigatedEventArgs(ShellFolder folder) : NavigatedEventArgs(folder)
{
    public int ItemCount { get; set; } = 0;
    public int FolderCount { get; set; } = 0;
    public int FileCount { get; set; } = 0;
}

