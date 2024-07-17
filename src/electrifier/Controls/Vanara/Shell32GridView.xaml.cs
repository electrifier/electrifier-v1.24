using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System;
using Vanara.Windows.Shell;
using System.Collections.Specialized;
using Microsoft.UI.Dispatching;
using Visibility = Microsoft.UI.Xaml.Visibility;

namespace electrifier.Controls.Vanara;

// ObservableRecipient ???
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public sealed partial class Shell32GridView : UserControl, INotifyPropertyChanged
{
    public Shell32GridView()
    {
        InitializeComponent();
        //DataContext = this;
    }

    public void SetItems(ExplorerBrowserItem ebItem)
    {
        Debug.Assert(ebItem != null);
        //if (Visibility == Visibility.Collapsed)
        //{
        //    // TODO: bool NeedsRefresh = true;
        //    return;
        //}

        // INFO: This works with breakpoint for delay I guess:
        var name = nameof(Shell32GridView) +".SetItems";
        var itemName = ebItem.DisplayName;
        var childCount = ebItem.Children?.Count;

        DispatcherQueue.TryEnqueue(() =>
        {
            var itmSource = ebItem.Children;

            NativeGridView.ItemsSource = itmSource;

            Debug.WriteLine($"{name}(`{itemName}`), {childCount} items");
        });


        //try
        //{
        //    var dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        //    Task.Run(() => 
        //    {
        //        var itmSrc = ebItem.Children;
        //        var testthis = NativeGridView.IsSynchronizedWithCurrentItem;
        //    });
        //}
        //catch (Exception e)
        //{
        //    Console.WriteLine(e);
        //    throw;
        //}
    }

    private void ObservableItemsCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Debug.WriteLine($".ObservableItemsCollection_CollectionChanged()");
    }

    private void NativeGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        Debug.WriteLine($".NativeGridView_ContainerContentChanging()");
    }

    private string GetDebuggerDisplay()
    {
        return nameof(Shell32GridView) + ToString();
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    #endregion
}
