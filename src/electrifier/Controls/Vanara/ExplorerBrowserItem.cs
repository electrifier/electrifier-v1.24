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