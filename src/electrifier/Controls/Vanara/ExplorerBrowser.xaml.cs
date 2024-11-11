/* todo: Use Visual States for Errors, Empty folders, Empty Drives */

using System.Collections;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Collections;
using electrifier.Controls.Vanara.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.InteropServices;
using System.Windows.Input;
using electrifier.Controls.Vanara.Contracts;
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
    private readonly ShellNamespaceService _namespaceService = new();
    private Visibility _topCommandBarVisibility;

    public ObservableCollection<BrowserItem> ListViewItems
    {
        get; private set;
    }
    public ObservableCollection<BrowserItem> TreeViewItems
    {
        get; private set;
    }
    ///// <summary>Current Folder content ListViewItems, as used by <see cref="ShellListView"/>.</summary>
    //public ExplorerBrowserItemCollection CurrentFolderItems
    //{
    //    get => (ExplorerBrowserItemCollection)GetValue(CurrentFolderItemsProperty);
    //    set => SetValue(CurrentFolderItemsProperty, value);
    //}
    //public static readonly DependencyProperty CurrentFolderItemsProperty = DependencyProperty.Register(
    //    nameof(CurrentFolderItems),
    //    typeof(ObservableCollection<ExplorerBrowserItem>),
    //    typeof(ExplorerBrowser),
    //    new PropertyMetadata(null,
    //        new PropertyChangedCallback(OnCurrentFolderItemsChanged)));
    //private static void OnCurrentFolderItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //{
    //    //ImageWithLabelControl iwlc = d as ImageWithLabelControl; //null checks omitted
    //    var s = e.NewValue; //null checks omitted
    //    Debug.Print($".OnCurrentFolderItemsChanged(): {s}");
    //}
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
    public event EventHandler<NavigationFailedEventArgs>? NavigationFailed;
    /// <summary>ExplorerBrowser Implementation for WinUI3.</summary>
    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;

        TreeViewItems = new ObservableCollection<BrowserItem>();
        ListViewItems = new ObservableCollection<BrowserItem>();

        //TreeViewItems.Add(new BrowserItem(ShellFolder.Desktop.PIDL, true));
        //ListViewItems.Add(new BrowserItem(ShellFolder.Desktop.PIDL, true));


        //TreeViewItems = new BrowserItemCollection();

        //_shell32AdvancedCollectionView = new AdvancedCollectionView(ListViewItems, true);


        //TreeViewItems.Add(new BrowserItem(ShellFolder.Desktop.PIDL, true));
        //

        //_shell32AdvancedCollectionView.Source = ListViewItems;
        //_shell32AdvancedCollectionView 
        //_advancedCollectionView = new(CurrentFolderItems, true);
        //NavigationFailed += ExplorerBrowser_NavigationFailed;
        //ShellTreeView.NativeTreeView.SelectionChanged += ShellTreeView_SelectionChanged;
        //ShellGridView.NativeGridView.SelectionChanged += NativeGridView_SelectionChanged;


    }

    ///* todo: see DataRow.Version */
    //public async Task UpdateGridView()
    //{
    //    //if (shDataTableTask is not null)
    //    //{
    //    //    await shDataTableTask;
    //    //    var rows = shDataTableTask.Result.Rows;
    //    //    foreach (DataRow row in rows)
    //    //    {
    //    //        var pidl = ShellDataTable.GetPIDL(row);

    //    //        CurrentFolderItems.Add(new ExplorerBrowserItem(pidl));
    //    //    }
    //    //}
    //}

    /* FolderItemFilter.FlatList => für Home-Folder */
    public async void Navigate(BrowserItem target)
    {
        try
        {
            Debug.Print($".Navigate(`{target.DisplayName}`)");

            if (target.IsFolder)
            {
                using var shFolder = new ShellFolder(target.ShellItem);

                // todo: warn: put to finally block
                //CurrentFolderBrowserItem = targetBrowserItem;
                //CurrentFolderItems.Clear(); // TODO: enumerate
                IsLoading = true;
            }
        }
        catch (COMException comEx)
        {
            var navFailedEventArgs = new NavigationFailedEventArgs
            {
                //                Hresult = comEx.HResult, // todo: put to lasterror, so com hresult handling
                //FailedLocation = target.DisplayName;
            };

            if (comEx.HResult == ShellNamespaceService.HResultElementNotFound)
            {
                Debug.WriteLine($"[Error] {comEx.HResult}: {navFailedEventArgs}");
                //NavigationFailure = msg;
                //HasNavigationFailure = true;
                //navFailedEventArgs.IsHandled = false;


                //if (navFailedEventArgs.IsHandled)
                //{
                //    return;
                //}
            }

            Debug.Fail($"[Error] Navigate(<{target}>) failed. COMException: <Result: {comEx.HResult}>: `{comEx.Message}`");
            throw;
        }
        catch (Exception ex)
        {
            Debug.Fail($"[Error] Navigate(<{target}>) failed, reason unknown: {ex.Message}");
            throw;
        }
        finally
        {
            IsLoading = false;
            //_ = UpdateGridView();
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


public class BrowserItem(Shell32.PIDL pidl, bool isFolder)
    : AbstractBrowserItem<ShellItem>(isFolder, childItems: new BrowserItemCollection())
{
    public readonly Shell32.PIDL PIDL = pidl;
    public string DisplayName => ShellItem.GetDisplayName(ShellItemDisplayString.NormalDisplay) ?? ShellItem.ToString();
    public ShellItem ShellItem = new ShellItem(pidl);
    public BitmapSource BitmapSource;

    public static BrowserItem FromPIDL(Shell32.PIDL pidl) => new(pidl, false);
    public static BrowserItem FromShellFolder(ShellFolder shellFolder) => new(shellFolder.PIDL, true);
    public static BrowserItem FromKnownItemId(Shell32.KNOWNFOLDERID knownItemId) => new(new ShellFolder(knownItemId).PIDL, true);
}
public partial class BrowserItemCollection : AbstractBrowserItemCollection<ShellItem>, IList
{
    protected IList ListImplementation => new List<BrowserItem>();
    public void CopyTo(Array array, int index) => ListImplementation.CopyTo(array, index);

    public int Count => ListImplementation.Count;

    public bool IsSynchronized => ListImplementation.IsSynchronized;

    public object SyncRoot => ListImplementation.SyncRoot;

    public int Add(object? value)
    {

        return ListImplementation.Add(value);
    }

    public void Clear() => ListImplementation.Clear();

    public bool Contains(object? value) => ListImplementation.Contains(value);

    public int IndexOf(object? value) => ListImplementation.IndexOf(value);

    public void Insert(int index, object? value) => ListImplementation.Insert(index, value);

    public void Remove(object? value) => ListImplementation.Remove(value);

    public void RemoveAt(int index) => ListImplementation.RemoveAt(index);

    public bool IsFixedSize => ListImplementation.IsFixedSize;

    public bool IsReadOnly => ListImplementation.IsReadOnly;

    public object? this[int index]
    {
        get => ListImplementation[index];
        set => ListImplementation[index] = value;
    }
}
