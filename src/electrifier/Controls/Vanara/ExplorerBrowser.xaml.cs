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
using Vanara.Collections;
using static CommunityToolkit.WinUI.Animations.Expressions.ExpressionValues;

namespace electrifier.Controls.Vanara;

// https://github.com/dahall/Vanara/blob/master/Windows.Forms/Controls/ExplorerBrowser.cs
/// <summary>Replacement for <see cref="Vanara.Windows.Forms.Controls.Explorer.cs">Windows.Forms/Controls/ExplorerBrowser.cs</see></summary>
public sealed partial class ExplorerBrowser : INotifyPropertyChanged
{
    public int ItemCount;

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
        Navigate(new BrowserItem(shHome.PIDL, true)); // TODO: Navigate to TreeViewItem!
    }

    public void Navigate(ShellItem? shellItem,
        ExplorerBrowserNavigationItemCategory category = ExplorerBrowserNavigationItemCategory.Default)
    {
        Debug.Assert(shellItem != null);

        Debug.WriteLineIf(!shellItem.IsFolder, $"Navigate({shellItem.ToString()}) => is not a folder!");
        // TODO: If no folder, or drive empty, etc... show empty listview with error message

        BrowserItem targetItem = new(shellItem.PIDL, null, null);
        _currentNavigationTask = Navigate(targetItem);
    }

    private Task<HRESULT>? _currentNavigationTask;

    public async Task<HRESULT> Navigate(BrowserItem target)
    {
        var shTargetItem = target.ShellItem;

        //Debug.WriteLineIf(!target.IsFolder, $"Navigate({target.DisplayName}) => is not a folder!");
        // TODO: If no folder, or drive empty, etc... show empty listview with error message

        try
        {
            if (_currentNavigationTask is { IsCompleted: false })
            {
                // cancel current task
                //CurrentNavigationTask
            }

            IsLoading = true;

            if (target.ChildItems.Count <= 0)
            {
                using var shFolder = new ShellFolder(target.ShellItem);

            target.ChildItems.Clear();
            ShellListView.Items.Clear();
                foreach (var child in shFolder)
                {
                    var shStockIconId = child.IsFolder
                        ? Shell32.SHSTOCKICONID.SIID_FOLDER
                        : Shell32.SHSTOCKICONID.SIID_DOCASSOC;

                    var ebItem = new BrowserItem(child.PIDL, child.IsFolder)
                    {
                        // TODO: init ShellNamespaceService
                        //SoftwareBitmap = await ShellNamespaceService.GetStockIconBitmapSource(shStockIconId)
                    };

                    target.ChildItems.Add(ebItem);
                    ShellListView.Items.Add(ebItem);
                }
            }

            // TODO: Load folder-open icon and overlays



//            foreach (var child in shFolder)
//            {
//                target.ChildItems.Add(ebItem);
//                ShellListView.Items.Add(ebItem);
//            }
        }
        catch (COMException comEx)
        {
            Debug.Fail(
                $"[Error] Navigate(<{target}>) failed. COMException: <HResult: {comEx.HResult}>: `{comEx.Message}`");

            return new HRESULT(comEx.HResult);
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

        return HRESULT.S_OK;
    }

    //    /// <summary>
    //    /// Clears the Explorer Browser of existing content, fills it with content from the specified container, and adds a new point to the
    //    /// Travel Log.
    //    /// </summary>
    //    /// <param name="shellItem">The shell container to navigate to.</param>
    //    /// <param name="category">The category of the <paramref name="shellItem"/>.</param>
    //    public void Navigate(ShellItem? shellItem, ExplorerBrowserNavigationItemCategory category = ExplorerBrowserNavigationItemCategory.Default)
    //    {
    //        if (explorerBrowserControl is null)
    //        {
    //            antecreationNavigationTarget = (shellItem, category);
    //        }
    //        else if (shellItem is not null)
    //        {
    //            var hr = explorerBrowserControl.BrowseToObject(shellItem.IShellItem, (SBSP)category);
    //            if (hr == HRESULT_RESOURCE_IN_USE || hr == HRESULT_CANCELLED)
    //                OnNavigationFailed(new NavigationFailedEventArgs { FailedLocation = shellItem });
    //            else if (hr.Failed)
    //                throw new ArgumentException("Unable to browse to this shell item.", nameof(shellItem), hr.GetException());
    //        }
    //    }
    //
    //    /// <summary>
    //    /// Navigates to the last item in the navigation history list. This does not change the set of locations in the navigation log.
    //    /// </summary>
    //    public bool NavigateBack()
    //    {
    //        if (History.CanNavigateBackward) { Navigate(null, ExplorerBrowserNavigationItemCategory.NavigateBack); return true; }
    //        return false;
    //    }
    //
    //    /// <summary>
    //    /// Navigates to the next item in the navigation history list. This does not change the set of locations in the navigation log.
    //    /// </summary>
    //    /// <returns>True if the navigation succeeded, false if it failed for any reason.</returns>
    //    public bool NavigateForward()
    //    {
    //        if (History.CanNavigateForward) { Navigate(null, ExplorerBrowserNavigationItemCategory.NavigateForward); return true; }
    //        return false;
    //    }
    //
    //    /// <summary>
    //    /// Navigate within the navigation log in a specific direciton. This does not change the set of locations in the navigation log.
    //    /// </summary>
    //    /// <param name="direction">The direction to navigate within the navigation logs collection.</param>
    //    /// <returns>True if the navigation succeeded, false if it failed for any reason.</returns>
    //    public bool NavigateFromHistory(NavigationLogDirection direction) => History.NavigateLog(direction);
    //
    //    /// <summary>Navigate within the navigation log. This does not change the set of locations in the navigation log.</summary>
    //    /// <param name="historyIndex">An index into the navigation logs Locations collection.</param>
    //    /// <returns>True if the navigation succeeded, false if it failed for any reason.</returns>
    //    public bool NavigateToHistoryIndex(int historyIndex) => History.NavigateLog(historyIndex);
    //
    //    /// <summary>Selects all items in the current view.</summary>
    //    public void SelectAll()
    //    {
    //        var fv2 = GetFolderView2();
    //        if (fv2 is null) return;
    //        for (var i = 0; i < fv2.ItemCount(SVGIO.SVGIO_ALLVIEW); i++)
    //            fv2.SelectItem(i, SVSIF.SVSI_SELECT);
    //    }
    //
    //    /// <summary>Unselects all items in the current view.</summary>
    //    public void UnselectAll()
    //    {
    //        var fv2 = GetFolderView2();
    //        fv2?.SelectItem(-1, SVSIF.SVSI_DESELECTOTHERS);
    //    }


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

                // TODO: Refresh
            }
            else
            {
                // TODO: ShellTreeView.NativeTreeView.SelectedItem = newTreeNode(find TreeNode

                if (selectedFolder?.PIDL is null)
                {
                    Debug.Print(".NativeTreeView_SelectionChanged(): selectedFolder is null!");
                    return;
                }

                // => TODO: currentTreeNode as TreeViewNode ;
                Navigate(selectedFolder.ShellItem, ExplorerBrowserNavigationItemCategory.Absolute);
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

#region DON'T TOUCH Imports from Vanara.Windows.Forms.ExplorerBrowser.
/// <summary>
/// Public imports from Vanara.
/// </summary>
public partial class ExplorerBrowser
{
    /// <summary>
    /// Indicates the content options of the explorer browser. Typically use one, or a bitwise combination of these flags to specify how
    /// content should appear in the explorer browser control
    /// </summary>
    [Flags]
    public enum ExplorerBrowserContentSectionOptions : uint
    {
        /// <summary>No options.</summary>
        None = FOLDERFLAGS.FWF_NONE,

        /// <summary>The view should be left-aligned.</summary>
        AlignLeft = FOLDERFLAGS.FWF_ALIGNLEFT,

        /// <summary>Automatically arrange the elements in the view.</summary>
        AutoArrange = FOLDERFLAGS.FWF_AUTOARRANGE,

        /// <summary>Turns on check mode for the view</summary>
        CheckSelect = FOLDERFLAGS.FWF_CHECKSELECT,

        /// <summary>When the view is set to "Tile" the layout of a single item should be extended to the width of the view.</summary>
        ExtendedTiles = FOLDERFLAGS.FWF_EXTENDEDTILES,

        /// <summary>When an item is selected, the item and all its sub-items are highlighted.</summary>
        FullRowSelect = FOLDERFLAGS.FWF_FULLROWSELECT,

        /// <summary>The view should not display file names</summary>
        HideFileNames = FOLDERFLAGS.FWF_HIDEFILENAMES,

        /// <summary>The view should not save view state in the browser.</summary>
        NoBrowserViewState = FOLDERFLAGS.FWF_NOBROWSERVIEWSTATE,

        /// <summary>Do not display a column header in the view in any view mode.</summary>
        NoColumnHeader = FOLDERFLAGS.FWF_NOCOLUMNHEADER,

        /// <summary>Only show the column header in details view mode.</summary>
        NoHeaderInAllViews = FOLDERFLAGS.FWF_NOHEADERINALLVIEWS,

        /// <summary>The view should not display icons.</summary>
        NoIcons = FOLDERFLAGS.FWF_NOICONS,

        /// <summary>Do not show subfolders.</summary>
        NoSubfolders = FOLDERFLAGS.FWF_NOSUBFOLDERS,

        /// <summary>Navigate with a single click</summary>
        SingleClickActivate = FOLDERFLAGS.FWF_SINGLECLICKACTIVATE,

        /// <summary>Do not allow more than a single item to be selected.</summary>
        SingleSelection = FOLDERFLAGS.FWF_SINGLESEL,

        /// <summary>
        /// Make the folder behave like the desktop. This value applies only to the desktop and is not used for typical Shell folders.
        /// </summary>
        Desktop = FOLDERFLAGS.FWF_DESKTOP,

        /// <summary>Draw transparently. This is used only for the desktop.</summary>
        Transparent = FOLDERFLAGS.FWF_TRANSPARENT,

        /// <summary>Do not add scroll bars. This is used only for the desktop.</summary>
        NoScrollBars = FOLDERFLAGS.FWF_NOSCROLL,

        /// <summary>The view should not be shown as a web view.</summary>
        NoWebView = FOLDERFLAGS.FWF_NOWEBVIEW,

        /// <summary>
        /// Windows Vista and later. Do not re-enumerate the view (or drop the current contents of the view) when the view is refreshed.
        /// </summary>
        NoEnumOnRefresh = FOLDERFLAGS.FWF_NOENUMREFRESH,

        /// <summary>Windows Vista and later. Do not allow grouping in the view</summary>
        NoGrouping = FOLDERFLAGS.FWF_NOGROUPING,

        /// <summary>Windows Vista and later. Do not display filters in the view.</summary>
        NoFilters = FOLDERFLAGS.FWF_NOFILTERS,

        /// <summary>Windows Vista and later. Items can be selected using check-boxes.</summary>
        AutoCheckSelect = FOLDERFLAGS.FWF_AUTOCHECKSELECT,

        /// <summary>Windows Vista and later. The view should list the number of items displayed in each group. To be used with IFolderView2::SetGroupSubsetCount.</summary>
        SubsetGroup = FOLDERFLAGS.FWF_SUBSETGROUPS,

        /// <summary>Windows Vista and later. Use the search folder for stacking and searching.</summary>
        UseSearchFolder = FOLDERFLAGS.FWF_USESEARCHFOLDER,

        /// <summary>
        /// Windows Vista and later. Ensure right-to-left reading layout in a right-to-left system. Without this flag, the view displays
        /// strings from left-to-right both on systems set to left-to-right and right-to-left reading layout, which ensures that file names
        /// display correctly.
        /// </summary>
        AllowRtlReading = FOLDERFLAGS.FWF_ALLOWRTLREADING,
    }

    /// <summary>These flags are used with <see cref="ExplorerBrowser.LoadCustomItems"/>.</summary>
    [Flags]
    public enum ExplorerBrowserLoadFlags
    {
        /// <summary>No flags.</summary>
        None = EXPLORER_BROWSER_FILL_FLAGS.EBF_NONE,

        /// <summary>
        /// Causes <see cref="ExplorerBrowser.LoadCustomItems"/> to first populate the results folder with the contents of the parent
        /// folders of the items in the data object, and then select only the items that are in the data object.
        /// </summary>
        SelectFromDataObject = EXPLORER_BROWSER_FILL_FLAGS.EBF_SELECTFROMDATAOBJECT,

        /// <summary>
        /// Do not allow dropping on the folder. In other words, do not register a drop target for the view. Applications can then register
        /// their own drop targets.
        /// </summary>
        NoDropTarget = EXPLORER_BROWSER_FILL_FLAGS.EBF_NODROPTARGET,
    }

    /// <summary>
    /// Specifies the options that control subsequent navigation. Typically use one, or a bitwise combination of these flags to specify how
    /// the explorer browser navigates.
    /// </summary>
    [Flags]
    public enum ExplorerBrowserNavigateOptions
    {
        /// <summary>No options.</summary>
        None = EXPLORER_BROWSER_OPTIONS.EBO_NONE,

        /// <summary>Always navigate, even if you are attempting to navigate to the current folder.</summary>
        AlwaysNavigate = EXPLORER_BROWSER_OPTIONS.EBO_ALWAYSNAVIGATE,

        /// <summary>Do not navigate further than the initial navigation.</summary>
        NavigateOnce = EXPLORER_BROWSER_OPTIONS.EBO_NAVIGATEONCE,

        /// <summary>
        /// Use the following standard panes: Commands Module pane, Navigation pane, Details pane, and Preview pane. An implementer of
        /// IExplorerPaneVisibility can modify the components of the Commands Module that are shown. For more information see,
        /// IExplorerPaneVisibility::GetPaneState. If EBO_SHOWFRAMES is not set, Explorer browser uses a single view object.
        /// </summary>
        ShowFrames = EXPLORER_BROWSER_OPTIONS.EBO_SHOWFRAMES,

        /// <summary>Do not update the travel log.</summary>
        NoTravelLog = EXPLORER_BROWSER_OPTIONS.EBO_NOTRAVELLOG,

        /// <summary>Do not use a wrapper window. This flag is used with legacy clients that need the browser parented directly on themselves.</summary>
        NoWrapperWindow = EXPLORER_BROWSER_OPTIONS.EBO_NOWRAPPERWINDOW,

        /// <summary>Show WebView for SharePoint sites.</summary>
        HtmlSharePointView = EXPLORER_BROWSER_OPTIONS.EBO_HTMLSHAREPOINTVIEW,

        /// <summary>Introduced in Windows Vista. Do not draw a border around the browser window.</summary>
        NoBorder = EXPLORER_BROWSER_OPTIONS.EBO_NOBORDER,

        /// <summary>Introduced in Windows Vista. Do not persist the view state.</summary>
        NoPersistViewState = EXPLORER_BROWSER_OPTIONS.EBO_NOPERSISTVIEWSTATE,
    }

    /// <summary>Flags specifying the folder to be browsed.</summary>
    [Flags]
    public enum ExplorerBrowserNavigationItemCategory : uint
    {
        /// <summary>An absolute PIDL, relative to the desktop.</summary>
        Absolute = SBSP.SBSP_ABSOLUTE,

        /// <summary>Windows Vista and later. Navigate without the default behavior of setting focus into the new view.</summary>
        ActivateNoFocus = SBSP.SBSP_ACTIVATE_NOFOCUS,

        /// <summary>Enable auto-navigation.</summary>
        AllowAutoNavigate = SBSP.SBSP_ALLOW_AUTONAVIGATE,

        /// <summary>
        /// Microsoft Internet Explorer 6 Service Pack 2 (SP2) and later. The navigation was possibly initiated by a web page with scripting
        /// code already present on the local system.
        /// </summary>
        CallerUntrusted = SBSP.SBSP_CALLERUNTRUSTED,

        /// <summary>
        /// Windows 7 and later. Do not add a new entry to the travel log. When the user enters a search term in the search box and
        /// subsequently refines the query, the browser navigates forward but does not add an additional travel log entry.
        /// </summary>
        CreateNoHistory = SBSP.SBSP_CREATENOHISTORY,

        /// <summary>
        /// Use default behavior, which respects the view option (the user setting to create new windows or to browse in place). In most
        /// cases, calling applications should use this flag.
        /// </summary>
        Default = SBSP.SBSP_DEFBROWSER,

        /// <summary>Use the current window.</summary>
        UseCurrentWindow = SBSP.SBSP_DEFMODE,

        /// <summary>
        /// Specifies a folder tree for the new browse window. If the current browser does not match the SBSP.SBSP_EXPLOREMODE of the browse
        /// object call, a new window is opened.
        /// </summary>
        ExploreMode = SBSP.SBSP_EXPLOREMODE,

        /// <summary>
        /// Windows Internet Explorer 7 and later. If allowed by current registry settings, give the browser a destination to navigate to.
        /// </summary>
        FeedNavigation = SBSP.SBSP_FEEDNAVIGATION,

        /// <summary>Windows Vista and later. Navigate without clearing the search entry field.</summary>
        KeepSearchText = SBSP.SBSP_KEEPWORDWHEELTEXT,

        /// <summary>Navigate back, ignore the PIDL.</summary>
        NavigateBack = SBSP.SBSP_NAVIGATEBACK,

        /// <summary>Navigate forward, ignore the PIDL.</summary>
        NavigateForward = SBSP.SBSP_NAVIGATEFORWARD,

        /// <summary>Creates another window for the specified folder.</summary>
        NewWindow = SBSP.SBSP_NEWBROWSER,

        /// <summary>Suppress selection in the history pane.</summary>
        NoHistorySelect = SBSP.SBSP_NOAUTOSELECT,

        /// <summary>Do not transfer the browsing history to the new window.</summary>
        NoTransferHistory = SBSP.SBSP_NOTRANSFERHIST,

        /// <summary>
        /// Specifies no folder tree for the new browse window. If the current browser does not match the SBSP.SBSP_OPENMODE of the browse
        /// object call, a new window is opened.
        /// </summary>
        NoFolderTree = SBSP.SBSP_OPENMODE,

        /// <summary>Browse the parent folder, ignore the PIDL.</summary>
        ParentFolder = SBSP.SBSP_PARENT,

        /// <summary>Windows 7 and later. Do not make the navigation complete sound for each keystroke in the search box.</summary>
        PlayNoSound = SBSP.SBSP_PLAYNOSOUND,

        /// <summary>Enables redirection to another URL.</summary>
        Redirect = SBSP.SBSP_REDIRECT,

        /// <summary>A relative PIDL, relative to the current folder.</summary>
        Relative = SBSP.SBSP_RELATIVE,

        /// <summary>Browse to another folder with the same Windows Explorer window.</summary>
        SameWindow = SBSP.SBSP_SAMEBROWSER,

        /// <summary>Microsoft Internet Explorer 6 Service Pack 2 (SP2) and later. The navigate should allow ActiveX prompts.</summary>
        TrustedForActiveX = SBSP.SBSP_TRUSTEDFORACTIVEX,

        /// <summary>
        /// Microsoft Internet Explorer 6 Service Pack 2 (SP2) and later. The new window is the result of a user initiated action. Trust the
        /// new window if it immediately attempts to download content.
        /// </summary>
        TrustFirstDownload = SBSP.SBSP_TRUSTFIRSTDOWNLOAD,

        /// <summary>
        /// Microsoft Internet Explorer 6 Service Pack 2 (SP2) and later. The window is navigating to an untrusted, non-HTML file. If the
        /// user attempts to download the file, do not allow the download.
        /// </summary>
        UntrustedForDownload = SBSP.SBSP_UNTRUSTEDFORDOWNLOAD,

        /// <summary>Write no history of this navigation in the history Shell folder.</summary>
        WriteNoHistory = SBSP.SBSP_WRITENOHISTORY
    }

    /// <summary>Indicates the viewing mode of the explorer browser</summary>
    public enum ExplorerBrowserViewMode
    {
        /// <summary>Choose the best view mode for the folder</summary>
        Auto = FOLDERVIEWMODE.FVM_AUTO,

        /// <summary>(New for Windows7)</summary>
        Content = FOLDERVIEWMODE.FVM_CONTENT,

        /// <summary>Object names and other selected information, such as the size or date last updated, are shown.</summary>
        Details = FOLDERVIEWMODE.FVM_DETAILS,

        /// <summary>The view should display medium-size icons.</summary>
        Icon = FOLDERVIEWMODE.FVM_ICON,

        /// <summary>Object names are displayed in a list view.</summary>
        List = FOLDERVIEWMODE.FVM_LIST,

        /// <summary>The view should display small icons.</summary>
        SmallIcon = FOLDERVIEWMODE.FVM_SMALLICON,

        /// <summary>The view should display thumbnail icons.</summary>
        Thumbnail = FOLDERVIEWMODE.FVM_THUMBNAIL,

        /// <summary>The view should display icons in a filmstrip format.</summary>
        ThumbStrip = FOLDERVIEWMODE.FVM_THUMBSTRIP,

        /// <summary>The view should display large icons.</summary>
        Tile = FOLDERVIEWMODE.FVM_TILE
    }

    /// <summary>Indicates the visibility state of an ExplorerBrowser pane.</summary>
    public enum PaneVisibilityState
    {
        /// <summary>Allow the explorer browser to determine if this pane is displayed.</summary>
        Default = EXPLORERPANESTATE.EPS_DONTCARE,

        /// <summary>Hide the pane</summary>
        Hide = EXPLORERPANESTATE.EPS_DEFAULT_OFF | EXPLORERPANESTATE.EPS_FORCE,

        /// <summary>Show the pane</summary>
        Show = EXPLORERPANESTATE.EPS_DEFAULT_ON | EXPLORERPANESTATE.EPS_FORCE
    }
}

#endregion DON'T TOUCH Imports from Vanara.Windows.Forms.ExplorerBrowser