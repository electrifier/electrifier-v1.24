using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using electrifier.Controls.Vanara.Contracts;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara.Helpers;

internal class BrowserItemFactory;

[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public abstract class AbstractBrowserItem<T>(bool isFolder, List<AbstractBrowserItem<T>>? childItems)
{
    public readonly bool
        IsFolder = isFolder; // WARN: TODO: Check this. If unknown, then find it out!  ... edit: or use virtual function for this!

    public readonly List<AbstractBrowserItem<T>> ChildItems = childItems ?? [];

    // ShellNamespace.RequestStockIcon()
    // StockIcon = new(int ArrayIndex, BitmapSource)
    //internal void async IconUpdate(int Index, SoftwareBitmapSource bmpSrc);
    //internal void async StockIconUpdate(STOCKICONID id, SoftwareBitmapSource bmpSrc);
    //internal void async ChildItemsIconUpdate();
    public new string ToString() => $"AbstractBrowserItem(<{typeof(T)}>(isFolder {isFolder}, childItems {childItems})";
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

    public static BrowserItem FromKnownFolderId(Shell32.KNOWNFOLDERID knownItemId) =>
        new(new ShellFolder(knownItemId).PIDL, true);

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