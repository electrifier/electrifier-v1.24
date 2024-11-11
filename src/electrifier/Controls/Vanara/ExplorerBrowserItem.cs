using System.Collections;
using electrifier.Controls.Vanara.Services;
using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

// TODO: TreeViewNode - Property, events
// TODO: GridViewItem - Property, events

/// <summary>ViewModel for both <see cref="Shell32GridView"/> and <see cref="Shell32TreeView"/> Items.</summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public class ExplorerBrowserItem : IDisposable, INotifyPropertyChanged
{
    /// <summary>The Shell Icon as <seealso cref="SoftwareBitmapSource"/>.</summary>
    public SoftwareBitmapSource BitmapSource { get; set; }
    /// <summary>The current set of child <seealso cref="ExplorerBrowserItem"/>s as <seealso cref="List{T}"/>.</summary>
    public List<ExplorerBrowserItem> Children = [];
    /// <summary>Get the DisplayName.</summary>
    public string DisplayName => ShellItem.Name ?? ":error: <DisplayName.get()>";
    /// <summary>
    /// The specified folders have subfolders. The SFGAO_HASSUBFOLDER attribute is only advisory and might be returned by Shell folder implementations even if they do not contain subfolders. Note, however, that the converse—failing to return SFGAO_HASSUBFOLDER—definitively states that the folder objects do not have subfolders.
    /// Returning SFGAO_HASSUBFOLDER is recommended whenever a significant amount of time is required to determine whether any subfolders exist. For example, the Shell always returns SFGAO_HASSUBFOLDER when a folder is located on a network drive.
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/shell/sfgao"/>
    /// </summary>
    public bool HasUnrealizedChildren => (ShellItem.Attributes.HasFlag(ShellItemAttribute.HasSubfolder));
    public bool IsExpanded { get; set; }
    public bool IsFolder => ShellItem.IsFolder;
    public bool IsHidden { get; set; } = false;
    public bool IsLink => ShellItem.IsLink;
    public bool IsProgressing { get; set; } = false;
    public bool IsSelected { get; set; }
    public double Opacity { get; set; } = 1;
    public ShellItem ShellItem { get; }
    private bool _disposedValue;

    /// <summary>ViewModel for both <see cref="Shell32GridView"/> and <see cref="Shell32TreeView"/> Items.</summary>
    public ExplorerBrowserItem(Shell32.PIDL shItemId, SoftwareBitmapSource? bitmapSource = null)
    {
        BitmapSource = bitmapSource;
        ShellItem = new ShellItem(shItemId);

        // TODO: Check for Library
        BitmapSource = IsFolder
            ? ShellNamespaceService.DefaultFolderImageBitmapSource
            : ShellNamespaceService.DefaultDocumentAssocImageBitmapSource;
    }
    public ExplorerBrowserItem(ShellItem shItem, SoftwareBitmapSource? bitmapSource = null) : this(shItem.PIDL, bitmapSource) { }
    public ExplorerBrowserItem(Shell32.KNOWNFOLDERID kfId, SoftwareBitmapSource? bitmapSource = null) : this(new ShellFolder(kfId).PIDL, bitmapSource) { }


    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #region Dispose pattern // todo: not implemented yet
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~ExplorerBrowserItem()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion Dispose pattern

    #region GetDebuggerDisplay()
    private string GetDebuggerDisplay()
    {
        var sb = new StringBuilder();
        sb.Append($"<{nameof(ExplorerBrowserItem)}> `{DisplayName}`");

        if (IsFolder) { sb.Append(", [folder]"); }

        return sb.ToString();
    }
    #endregion GetDebuggerDisplay()
}

public static class ExplorerBrowserItemExtensions
{
    public static ExplorerBrowserItem enumChilds(this ExplorerBrowserItem itm)
    {
        if (itm.Children.Count > 0)
        {
            itm.Children.Clear();
        }

        return itm;
    }
}

/// <summary>
/// Wrapper for 
///     Vanara.Windows.Shell.ShellItemArray
/// Implementing
///     System.Collections.IList
/// </summary>
public class ReadOnlyShell32ItemArray : ShellItemArray, IList
{
    object? IList.this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public bool IsFixedSize => true;
    public bool IsReadOnly => true;
    public bool IsSynchronized => throw new NotImplementedException();
    public object SyncRoot => throw new NotImplementedException();
    public int Add(object? value) => throw new NotImplementedException();
    public void Clear() => throw new NotImplementedException();
    public bool Contains(object? value)
    {
        return false;
    }
    public void CopyTo(Array array, int index)
    {
    }
    public int IndexOf(object? value)
    {
        return -1;
    }
    public void Insert(int index, object? value) => throw new NotImplementedException();
    public void Remove(object? value) => throw new NotImplementedException();
    public void RemoveAt(int index) => throw new NotImplementedException();

    // TODO event CollectionChanged, add Creator/Sender (TreeViewItem, ListViewItem) - attribute
    // TODO event iconchanged
    // todo from(epBrowserItem), from(ReadableShellItemArray), for(explorerbrowser) etc..
}


/// <summary>
/// Wrapper for 
///     Vanara.Windows.Shell.ShellItemArray
/// Implementing
///     System.Collections.IList
///
/// todo: Add version index
/// todo: /* IPropertyChanged */
/// </summary>
public class Shell32ItemArray : IList 
{
    private ArraySegment<ShellItem> _folderSegment = [];
    private ArraySegment<ShellItem> _nonFolderSegment = [];
    //private object[ShellItem] _contents = new ArraySegment<ShellItem>();
    // private object[] _contents = new object[8];
    //private IList<Shell32.IEnumShellItems> list; => IEnumShellItems

    public Shell32ItemArray() { }

    void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
    int ICollection.Count => _folderSegment.Count + _nonFolderSegment.Count;
    bool ICollection.IsSynchronized => throw new NotImplementedException();
    object ICollection.SyncRoot => throw new NotImplementedException();
    object? IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => false;
    int IList.Add(object? value) => throw new NotImplementedException();
    void IList.Clear() => throw new NotImplementedException();
    bool IList.Contains(object? value) => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    int IList.IndexOf(object? value) => throw new NotImplementedException();
    void IList.Insert(int index, object? value) => throw new NotImplementedException();
    void IList.Remove(object? value) => throw new NotImplementedException();
    void IList.RemoveAt(int index) => throw new NotImplementedException();
}