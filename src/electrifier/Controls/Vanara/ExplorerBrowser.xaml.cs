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
using electrifier.Controls.Vanara.Helpers;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

using static Vanara.PInvoke.Shell32;

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

    private readonly Dictionary<Shell32.SHSTOCKICONID, SoftwareBitmapSource> _stockIconDictionary = [];

    /// <summary>ExplorerBrowser Implementation for WinUI3.</summary>
    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;

        ShellTreeView.NativeTreeView.SelectionChanged += NativeTreeView_SelectionChanged;

        using var shHome = ShellNamespaceService.HomeShellFolder;
        //Navigate(new BrowserItem(shHome.PIDL, true)); // TODO: Navigate to TreeViewItem!
    }

    // TODO: @dahall: Maybe we should throw HRESULT-COM-Errors at least here? Your HRESULT.ThrowIfFailed() - Pattern?
    public async void Navigate(BrowserItem target)
    {
        Debug.WriteLineIf(!target.IsFolder, $"Navigate({target.DisplayName}) => is not a folder!");

        try
        {
            IsLoading = true;
            using var shFolder = new ShellFolder(target.ShellItem);

            target.ChildItems.Clear();
            ShellListView.Items.Clear();
            foreach (var child in shFolder)
            {
                var ebItem = new BrowserItem(child.PIDL, child.IsFolder)
                {
                    SoftwareBitmap = child.IsFolder
                    ? await BrowserItemFactory.GetStockIconBitmapSource(Shell32.SHSTOCKICONID.SIID_FOLDER)
                    : await BrowserItemFactory.GetStockIconBitmapSource(Shell32.SHSTOCKICONID.SIID_DOCNOASSOC)
                };

                target.ChildItems.Add(ebItem);
                ShellListView.Items.Add(ebItem);
            }
        }
        catch (COMException comEx)
        {
            Debug.Fail($"[Error] Navigate(<{target}>) failed. COMException: <HResult: {comEx.HResult}>: `{comEx.Message}`");
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
        }
    }

    public async void Navigate(ShellItem target, TreeViewNode? treeViewNode = null)
    {
        // TODO: Search for best matching RootItem in the tree hierarchy
    }

    private void NativeTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        var addedItems = args.AddedItems;
        if (addedItems.Count > 0)
        {
            Debug.Assert(addedItems.Count == 1);
            var selectedFolder = addedItems[0] as BrowserItem;
            var currentTreeNode = ShellTreeView.NativeTreeView.SelectedItem;
            Debug.Print($".NativeTreeView_SelectionChanged(`{selectedFolder?.DisplayName}`, treeNode: {currentTreeNode?.ToString()}");

            if (currentTreeNode is BrowserItem browserItem && browserItem.PIDL.Equals(selectedFolder?.PIDL))
            {
                Debug.Print(".NativeTreeView_SelectionChanged(): CurrentTreeNode already equals selected /added Item.");
            }
            else
            {
                // TODO: ShellTreeView.NativeTreeView.SelectedItem = newTreeNode(find TreeNode

                if (selectedFolder?.PIDL is null)
                {
                    Debug.Print(".NativeTreeView_SelectionChanged(): selectedFolder is null!");

                    return;
                }

                Navigate(selectedFolder);
                //Navigate(selectedFolder.ShellItem, currentTreeNode as TreeViewNode);
                // TODO: Raise NavigationFailed()
            }
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

[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public class BrowserItem(Shell32.PIDL pidl, bool isFolder, List<AbstractBrowserItem<ShellItem>>? childItems = default)
    : AbstractBrowserItem<ShellItem>(isFolder, childItems), INotifyPropertyChanged
{
    public readonly Shell32.PIDL PIDL = new(pidl);
    public string DisplayName => ShellItem.GetDisplayName(ShellItemDisplayString.NormalDisplay) ?? ShellItem.ToString();
    public ShellItem ShellItem = new(pidl);
    public new ObservableCollection<BrowserItem> ChildItems = [];
    public static BrowserItem FromPIDL(Shell32.PIDL pidl) => new(pidl, false);
    public static BrowserItem FromShellFolder(ShellFolder shellFolder) => new(shellFolder.PIDL, true);
    public static BrowserItem FromKnownFolderId(Shell32.KNOWNFOLDERID knownItemId) => new(new ShellFolder(knownItemId).PIDL, true);

    public SoftwareBitmapSource? SoftwareBitmap;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
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
