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
    private readonly ShellNamespaceService _namespaceService = new();
    public List<BrowserItem> ListViewItems = [];
    public List<BrowserItem> TreeViewItems = [];
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

    /// <summary>ExplorerBrowser Implementation for WinUI3.</summary>
    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;

        TreeViewItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Videos));
        TreeViewItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Videos));
        TreeViewItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Videos));
        TreeViewItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Videos));
        AdvancedCollectionView acv = new(TreeViewItems, true);
        this.ShellTreeView.NativeTreeView.ItemsSource = acv;

        Navigate(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Desktop));
    }

    public async void Navigate(BrowserItem target)
    {
        Debug.Print($".Navigate(`{target.DisplayName}`)");

        try
        {
            if (target.IsFolder)
            {
                using var shFolder = new ShellFolder(target.ShellItem);

                // todo: warn: put to finally block
                //CurrentFolderBrowserItem = targetBrowserItem;
                //CurrentFolderItems.Clear(); // TODO: enumerate
                IsLoading = true;
                target.ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Videos));
                target.ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Videos));
                target.ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Videos));
                target.ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_Videos));

                AdvancedCollectionView acvTree = new(target.ChildItems, true);
                this.ShellTreeView.NativeTreeView.ItemsSource = acvTree;

                AdvancedCollectionView acvList = new(target.ChildItems, true);
                //this.ShellListView.NativeListView.ItemsSource = acvList;
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
    public readonly Shell32.PIDL PIDL = new Shell32.PIDL(pidl);
    public string DisplayName => ShellItem.GetDisplayName(ShellItemDisplayString.NormalDisplay) ?? ShellItem.ToString();
    public ShellItem ShellItem = new ShellItem(pidl);
    public SoftwareBitmapSource SoftwareBitmapSource = ShellNamespaceService.DefaultDocumentAssocImageBitmapSource;
    public new List<BrowserItem> ChildItems = [];
    public static BrowserItem FromPIDL(Shell32.PIDL pidl) => new(pidl, false);
    public static BrowserItem FromShellFolder(ShellFolder shellFolder) => new(shellFolder.PIDL, true);
    public static BrowserItem FromKnownItemId(Shell32.KNOWNFOLDERID knownItemId) => new(new ShellFolder(knownItemId).PIDL, true);

    public Task<int> Enumerate()
    {
        ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_AddNewPrograms));
        ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_AddNewPrograms));
        ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_AddNewPrograms));
        ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_AddNewPrograms));
        ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_AddNewPrograms));
        ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_AddNewPrograms));
        ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_AddNewPrograms));
        ChildItems.Add(BrowserItem.FromKnownItemId(Shell32.KNOWNFOLDERID.FOLDERID_AddNewPrograms));
        return Task.CompletedTask as Task<int>;
    }
}


public partial class BrowserItemCollection : List<ShellItem>, IList
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
