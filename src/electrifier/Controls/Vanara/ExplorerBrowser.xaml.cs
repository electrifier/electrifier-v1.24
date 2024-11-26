/* todo: Use Visual States for Errors, Empty folders, Empty Drives */

using electrifier.Controls.Vanara.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using electrifier.Controls.Vanara.Helpers;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using static Vanara.PInvoke.Shell32;

namespace electrifier.Controls.Vanara;

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
                        ? await GetStockIconBitmapSource(Shell32.SHSTOCKICONID.SIID_FOLDER)
                        : await GetStockIconBitmapSource(Shell32.SHSTOCKICONID.SIID_DOCNOASSOC)
                };

                target.ChildItems.Add(ebItem);
                ShellListView.Items.Add(ebItem);
            }
        }
        catch (COMException comEx)
        {
            Debug.Fail(
                $"[Error] Navigate(<{target}>) failed. COMException: <HResult: {comEx.HResult}>: `{comEx.Message}`");
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

    public async Task<SoftwareBitmapSource> GetStockIconBitmapSource(Shell32.SHSTOCKICONID shStockIconId)
    {
        try
        {
            if (_stockIconDictionary.TryGetValue(shStockIconId, out var source))
            {
                return source;
            }

            var siFlags = Shell32.SHGSI.SHGSI_LARGEICON | Shell32.SHGSI.SHGSI_ICON;
            var icninfo = Shell32.SHSTOCKICONINFO.Default;
            SHGetStockIconInfo(shStockIconId, siFlags, ref icninfo)
                .ThrowIfFailed($"SHGetStockIconInfo({shStockIconId})");

            var hIcon = icninfo.hIcon;
            var icnHandle = hIcon.ToIcon();
            var bmpSource = ShellNamespaceService.GetWinUi3BitmapSourceFromIcon(icnHandle);
            await bmpSource;
            var softBitmap = bmpSource.Result;

            if (softBitmap != null)
            {
                _ = _stockIconDictionary.TryAdd(shStockIconId, softBitmap);
                return softBitmap;
            }

            throw new ArgumentOutOfRangeException($"Can't get StockIcon for SHSTOCKICONID: {shStockIconId.ToString()}");
        }
        catch (Exception)
        {
            throw; // TODO handle exception
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
            Debug.Print(
                $".NativeTreeView_SelectionChanged(`{selectedFolder?.DisplayName}`, treeNode: {currentTreeNode?.ToString()}");

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

