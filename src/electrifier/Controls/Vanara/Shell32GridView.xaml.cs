using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public sealed partial class Shell32GridView : UserControl, INotifyPropertyChanged   
{
    public List<ExplorerBrowserItem> Items = [];
    public ObservableCollection<ExplorerBrowserItem> ObservableItemsCollection;

    public Shell32GridView()
    {
        InitializeComponent();
        DataContext = this;

        ObservableItemsCollection = new ObservableCollection<ExplorerBrowserItem>(Items);
        ObservableItemsCollection.CollectionChanged += ObservableItemsCollection_CollectionChanged;
    }

    public void SetItems(ExplorerBrowserItem ebItem)
    {
        try
        {
            SetField(ref Items, ebItem.Children);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void ObservableItemsCollection_CollectionChanged(object? sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        throw new NotImplementedException("ObservableItemsCollection_CollectionChanged");
    }

    //private void GridView_OnItemClick(object sender, ItemClickEventArgs e)
    //{
    //    if (e.ClickedItem is not ExplorerBrowserItem ebItem)
    //    {
    //        Debug.Print($"GridView_OnItemClick: OnItemClick: No valid ExplorerBrowserItem");
    //        return;
    //    }
    //    var shItem = ebItem.ShellItem;
    //    // TODO: Raise event on DblClick to call 'ebItem.Owner.TryNavigate(shItem);'
    //}

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
