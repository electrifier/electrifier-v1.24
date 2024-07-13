using System.Diagnostics;
using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

    public void SetItemsSource(List<ExplorerBrowserItem> itemSourceCollection)
    {
        try
        {
            Items.Clear();
            Items.AddRange(itemSourceCollection);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void ObservableItemsCollection_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => throw new NotImplementedException();

    private void GridView_OnItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not ExplorerBrowserItem ebItem)
        {
            return;
        }

        var shItem = ebItem.ShellItem;
        // TODO: ebItem.Owner.TryNavigate(shItem);
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
