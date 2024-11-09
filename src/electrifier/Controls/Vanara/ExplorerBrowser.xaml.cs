/* todo: Use Visual States for Errors, Empty folders, Empty Drives */
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Collections;
using electrifier.Controls.Vanara.Services;
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
using Vanara.PInvoke;
using Vanara.Windows.Shell;
namespace electrifier.Controls.Vanara;
using Visibility = Microsoft.UI.Xaml.Visibility;
// https://github.com/dahall/Vanara/blob/master/Windows.Forms/Controls/ExplorerBrowser.cs
/// <summary>Replacement for <see cref="Vanara.Windows.Forms.Controls.Explorer.cs">Windows.Forms/Controls/ExplorerBrowser.cs</see></summary>
public sealed partial class ExplorerBrowser : INotifyPropertyChanged
{
    private Visibility _bottomAppBarVisibility;
    private Visibility _bottomCommandBarVisibility;
    private Visibility _gridViewVisibility;
    private bool _isLoading;
    private ShellNamespaceService _namespaceService = new();
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
    /// <summary>Current Folder content Items, as used by <see cref="Shell32GridView"/>.</summary>
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
    /// <summary>Fires when either a Navigating listener cancels the navigation, or if the operating system determines that navigation is not possible.</summary>
    public event EventHandler<ExtNavigationFailedEventArgs>? NavigationFailed;
    /// <summary>ExplorerBrowser Implementation for WinUI3.</summary>
    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;
        CurrentFolderItems = [];
        _advancedCollectionView = new(CurrentFolderItems, true);
        NavigationFailed += ExplorerBrowser_NavigationFailed;
        ShellTreeView.NativeTreeView.SelectionChanged += ShellTreeView_SelectionChanged;
        ShellGridView.NativeGridView.SelectionChanged += NativeGridView_SelectionChanged;

        // todo: ask for rootItems in callback event handler
        var rootItems = new List<ExplorerBrowserItem>
        {
            new (ShellNamespaceService.HomeShellFolder),
            // todo: add separators
            new(Shell32.KNOWNFOLDERID.FOLDERID_OneDrive),
            new(Shell32.KNOWNFOLDERID.FOLDERID_Downloads),
            new(Shell32.KNOWNFOLDERID.FOLDERID_Documents),
            new(Shell32.KNOWNFOLDERID.FOLDERID_Desktop),
            new(Shell32.KNOWNFOLDERID.FOLDERID_Pictures),
            new(Shell32.KNOWNFOLDERID.FOLDERID_Music),
            new(Shell32.KNOWNFOLDERID.FOLDERID_Videos),
            new(Shell32.KNOWNFOLDERID.FOLDERID_ComputerFolder),
            new(Shell32.KNOWNFOLDERID.FOLDERID_NetworkFolder),
            // todo: WARN: Check why this leads to `SyncCenter`?
            new(Shell32.KNOWNFOLDERID.FOLDERID_ThisPCDesktop),
            // todo: WARN: add Gallery? is this Item caused by Adobe?
        };
        Loading += async (sender, args) =>
        {
            ShellTreeView.ItemsSource = rootItems;
            if (rootItems.FirstOrDefault(new ExplorerBrowserItem(Shell32.KNOWNFOLDERID.FOLDERID_Desktop))
                is { } ebItem)
            {
                Navigate(ebItem);
                CurrentFolderBrowserItem = ebItem;  // TODO; Put to OnNavigated() event-handler
            }
        };
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

            var testEnum = ShellNamespaceService.RequestChildItemsAsync(targetBrowserItem);
            var childShellItems = ShellNamespaceService.ExtractChildItems(targetBrowserItem);

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

            if (comEx.HResult == ShellNamespaceService.HResultElementNotFound)
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
    /// <summary>Raises the <see cref="NavigationFailed"/> event.</summary>
    internal void OnNavigationFailed(ExtNavigationFailedEventArgs? nfevent)
    {
        if (nfevent?.FailedLocation is null)
        {
            return;
        }

        NavigationFailed?.Invoke(this, nfevent);
    }
    private void ExplorerBrowser_NavigationFailed(object? sender, ExtNavigationFailedEventArgs e)
    {
        NavigationFailure = $"Navigation failed: '{e.FailedLocation}' cannot be navigated to. <Show More Info> <Report a Bug>";
        IsLoading = false;
        throw new ArgumentOutOfRangeException(NavigationFailure);
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

