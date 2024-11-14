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
    private bool _isLoading;
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
    public event EventHandler<NavigatedEventArgs> Navigated;
    public event EventHandler<NavigationFailedEventArgs> NavigationFailed;

    public static ShellNamespaceService ShellNamespaceService => App.GetService<ShellNamespaceService>();

    /// <summary>ExplorerBrowser Implementation for WinUI3.</summary>
    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;

        ShellTreeView.NativeTreeView.SelectionChanged += NativeTreeView_SelectionChanged;

        using var shHome = ShellNamespaceService.HomeShellFolder;
        Navigate(new BrowserItem(shHome.PIDL, true));
    }

    public async void Navigate(BrowserItem target)
    {
        Debug.WriteLineIf(!target.IsFolder, $"Navigate({target.DisplayName}) => `{{target.DisplayName}}` is not a folder!");

        try
        {
            IsLoading = true;
            using var shFolder = new ShellFolder(target.ShellItem);

            ShellListView.Items.Clear();
            foreach (var child in shFolder)
            {
                if (child.IsFolder)
                    target.ChildItems.Add(new BrowserItem(child.PIDL, true));
                ShellListView.Items.Add(new BrowserItem(child.PIDL, child.IsFolder));
            }

        }
        catch (COMException comEx)
        {
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

    public async void Navigate(ShellItem target, TreeViewNode? treeViewNode = null)
    {
        // TODO: Search for best matching RootItem in the tree hierarchy
    }

    private void NativeTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        Debug.Print($".NativeTreeView_SelectionChanged(): Updating `ShellListView` items...");

        ShellListView.Items.Clear();

        var addedItems = args.AddedItems;
        if (addedItems.Count > 0)
        {
            var folder = addedItems[0] as BrowserItem;

            foreach (var childItem in folder.ChildItems)
            {
                ShellListView.Items.Add(BrowserItem.FromPIDL(childItem.PIDL));
            }

            Navigate(folder);
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


public class BrowserItem(Shell32.PIDL pidl, bool isFolder, List<AbstractBrowserItem<ShellItem>>? childItems = default)
    : AbstractBrowserItem<ShellItem>(isFolder, childItems)
{
    public readonly Shell32.PIDL PIDL = new(pidl);
    public string DisplayName => ShellItem.GetDisplayName(ShellItemDisplayString.NormalDisplay) ?? ShellItem.ToString();
    public ShellItem ShellItem = new(pidl);
    public SoftwareBitmapSource SoftwareBitmapSource = isFolder ? ShellNamespaceService.FolderBitmapSource : ShellNamespaceService.DocumentBitmapSource;
    public new ObservableCollection<BrowserItem> ChildItems = [];
    public static BrowserItem FromPIDL(Shell32.PIDL pidl) => new(pidl, false);
    public static BrowserItem FromShellFolder(ShellFolder shellFolder) => new(shellFolder.PIDL, true);
    public static BrowserItem FromKnownFolderId(Shell32.KNOWNFOLDERID knownItemId) => new(new ShellFolder(knownItemId).PIDL, true);


    //public Task<int> Enumerate()
    //{
    //    ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_AddNewPrograms));
    //    ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_AddNewPrograms));
    //    return Task.CompletedTask as Task<int>;
    //}
}


public partial class BrowserItemCollection : List<ShellItem>, IList
{
    protected IList ListImplementation => new List<BrowserItem>();

    public void CopyTo(Array array, int index) => ListImplementation.CopyTo(array, index);
    public new int Count => ListImplementation.Count;
    public bool IsSynchronized => ListImplementation.IsSynchronized;
    public object SyncRoot => ListImplementation.SyncRoot;
    public int Add(object? value) => ListImplementation.Add(value);
    public new void Clear() => ListImplementation.Clear();
    public bool Contains(object? value) => ListImplementation.Contains(value);
    public int IndexOf(object? value) => ListImplementation.IndexOf(value);
    public void Insert(int index, object? value) => ListImplementation.Insert(index, value);
    public void Remove(object? value) => ListImplementation.Remove(value);
    public new void RemoveAt(int index) => ListImplementation.RemoveAt(index);
    public bool IsFixedSize => ListImplementation.IsFixedSize;
    public bool IsReadOnly => ListImplementation.IsReadOnly;
    public new object? this[int index]
    {
        get => ListImplementation[index];
        set => ListImplementation[index] = value;
    }
}
