using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

// https://github.com/dahall/Vanara/blob/master/Windows.Forms/Controls/ExplorerBrowser.cs
// TODO: See also https://github.com/dahall/Vanara/blob/ac0a1ac301dd4fdea9706688dedf96d596a4908a/Windows.Shell.Common/StockIcon.cs
public sealed partial class ExplorerBrowser : INotifyPropertyChanged
{
    public ExplorerBrowserItem CurrentFolderBrowserItem;

    // TODO: Use shell32 stock icons
    internal static readonly BitmapImage DefaultFileImage =
        new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico"));

    internal static readonly BitmapImage DefaultFolderImage =
        new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico"));

    internal static readonly BitmapImage DefaultLibraryImage =
        new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Library.ico"));

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

    public ImageCache ImageCache
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
        _ = InitializeViewModel();


        //ShellTreeView.SelectionChanged += ShellTreeView_SelectionChanged;
    }

    private async Task InitializeViewModel()
    {
        ImageCache = new ImageCache();
        CurrentFolderBrowserItem = new ExplorerBrowserItem(ShellFolder.Desktop);
        ShellTreeView.InitializeRoot(CurrentFolderBrowserItem);
    }



    public bool TryNavigate(ExplorerBrowserItem targetFolder)
    {
        if (!targetFolder.ShellItem.IsFolder)
        {
            Debug.Fail($"TryNavigate: IsFolder of item {targetFolder.ShellItem} is false.");
            throw new InvalidOperationException($"TryNavigate: IsFolder of item {targetFolder.ShellItem} is false.");
        }

        try
        {
            Navigate2Target(targetFolder);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return true;
    }

    private void Navigate2Target(ExplorerBrowserItem targetFolder)
    {
        Debug.Assert(targetFolder is not null);

        var shellIconExtractor = new ShellIconExtractor(targetFolder.ShellItem.PIDL);
        shellIconExtractor.IconExtracted += IconExtOnIconExtracted;
        shellIconExtractor.Complete += IconExtOnComplete;
        shellIconExtractor.Start();

        // TODO: `CurrentFolderBrowserItem` shouldn't be created, but found in the tree!
        CurrentFolderBrowserItem = new ExplorerBrowserItem(targetFolder.ShellItem);
        IconExtractor = shellIconExtractor;

        void IconExtOnIconExtracted(object? sender, ShellIconExtractedEventArgs e)
        {
            var shItem = new ShellItem(e.ItemID);
            var ebItem = new ExplorerBrowserItem(shItem);
            CurrentFolderBrowserItem.Children.Add(ebItem);
        }

        void IconExtOnComplete(object? sender, EventArgs e)
        {
            var cnt = CurrentFolderBrowserItem.Children.Count;
            Debug.Print($".IconExtOnComplete(): {cnt} items");

            // TODO: using root item here, should be target folder?!?
            ShellTreeView.SetItemsSource(CurrentFolderBrowserItem);
            ShellGridView.SetItems(CurrentFolderBrowserItem);
        }
    }

    private void RefreshButtonClick(object sender, RoutedEventArgs e)
    {
        TryNavigate(CurrentFolderBrowserItem);
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
