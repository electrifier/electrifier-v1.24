using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara.Contracts;

[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public struct AbstractBrowserItem<T>(T itemId, bool isFolder, AbstractBrowserItemCollection<T> childItems)
{
    public readonly T ItemId = itemId;
    public readonly bool IsFolder = false;

    //public AbstractBrowserItemCollection<T, T2> ChildItems = childItems;
    //public new string ToString() => $"AbstractBrowserItem<T {typeof(T)}>({abstractItem})";
}

[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public class AbstractBrowserItemCollection<T> : IEnumerable<AbstractBrowserItem<T>>, IList<AbstractBrowserItem<T>>
{
    private IList<AbstractBrowserItem<T>> collection;

    AbstractBrowserItem<T> IList<AbstractBrowserItem<T>>.this[int index] { get => collection[index]; set => collection[index] = value; }
    int ICollection<AbstractBrowserItem<T>>.Count => throw new NotImplementedException();
    bool ICollection<AbstractBrowserItem<T>>.IsReadOnly => throw new NotImplementedException();
    void ICollection<AbstractBrowserItem<T>>.Add(AbstractBrowserItem<T> item) => throw new NotImplementedException();
    void ICollection<AbstractBrowserItem<T>>.Clear() => throw new NotImplementedException();
    bool ICollection<AbstractBrowserItem<T>>.Contains(AbstractBrowserItem<T> item) => throw new NotImplementedException();
    void ICollection<AbstractBrowserItem<T>>.CopyTo(AbstractBrowserItem<T>[] array, int arrayIndex) => throw new NotImplementedException();
    IEnumerator<AbstractBrowserItem<T>> IEnumerable<AbstractBrowserItem<T>>.GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    int IList<AbstractBrowserItem<T>>.IndexOf(AbstractBrowserItem<T> item) => throw new NotImplementedException();
    void IList<AbstractBrowserItem<T>>.Insert(int index, AbstractBrowserItem<T> item) => throw new NotImplementedException();
    bool ICollection<AbstractBrowserItem<T>>.Remove(AbstractBrowserItem<T> item) => throw new NotImplementedException();
    void IList<AbstractBrowserItem<T>>.RemoveAt(int index) => throw new NotImplementedException();
}


public class ExplorerBrowserItemCollection : AbstractBrowserItemCollection<ShellItem>
{
}


//public class ExplorerBrowserItem : AbstractBrowserItem<ShellItem>
//{
//    //public ExplorerBrowserItem(ShellItem shItem,
//    //    ExplorerBrowserItemCollection children) : base(shItem, children)
//    //{
//    //}
//    //public ExplorerBrowserItem(ShellItem shItem) : base(shItem,
//    //    shItem.PIDL,
//    //    shItem.IsFolder,
//    //    new ExplorerBrowserItemCollection())
//    //{
//    //}

//    //public static ExplorerBrowserItem Create(ShellItem shItem) => new ExplorerBrowserItem(shItem);
//}
//public partial class ExplorerBrowserItemCollection : AbstractBrowserItemCollection<ShellItem, Shell32.PIDL>, IList<ShellItem>, IList
//{
//    private IList<ShellItem> _listImplementation;

//    public override bool Remove(ShellItem item) => throw new NotImplementedException();

//    public override int Count
//    {
//        get;
//    }

//    public override bool IsReadOnly { get; }

//    public bool IsFixedSize => throw new NotImplementedException();

//    public bool IsSynchronized => throw new NotImplementedException();

//    public object SyncRoot => throw new NotImplementedException();

//    object? IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//    public override void Add(ShellItem item) => _listImplementation.Add(item);

//    public override void Clear() => throw new NotImplementedException();

//    public override bool Contains(ShellItem item) => throw new NotImplementedException();

//    public override void CopyTo(ShellItem[] array, int arrayIndex) => throw new NotImplementedException();

//    public override IEnumerator<ShellItem> GetEnumerator() => throw new NotImplementedException();
//    public int IndexOf(ShellItem item) => _listImplementation.IndexOf(item);

//    public void Insert(int index, ShellItem item) => _listImplementation.Insert(index, item);

//    public void RemoveAt(int index) => _listImplementation.RemoveAt(index);
//    public int Add(object? value) => throw new NotImplementedException();
////    public override void Add(ShellItem item) => throw new NotImplementedException();
//    public bool Contains(object? value) => throw new NotImplementedException();
//    public int IndexOf(object? value) => throw new NotImplementedException();
//    public void Insert(int index, object? value) => throw new NotImplementedException();
//    public void Remove(object? value) => throw new NotImplementedException();
//    public void CopyTo(Array array, int index) => throw new NotImplementedException();

//    public ShellItem this[int index]
//    {
//        get => _listImplementation[index];
//        set => _listImplementation[index] = value;
//    }}







//public class KnownFolderBrowserItem<T>(T folderItem) : AbstractBrowserItem<T>(folderItem)
//{
//    //KnownFolderBrowserItem(Shell32.KNOWNFOLDER_DEFINITION)
//    KnownFolderBrowserItem(Shell32.KNOWNFOLDERID);

//}

//public class KnownFolderBrowserItem(Shell32.KNOWNFOLDERID knownFolder)
//    : AbstractBrowserItem<ShellItem>(new ShellFolder(knownFolder))
//public class FolderBrowserItem(Shell32.Folder folder)
//    : AbstractBrowserItem<ShellItem>(new ShellItem(folder)), Shell32.Folder { }
public class Spielplatz
{
    public Spielplatz()
    {
        //var desktop = new Abtract
    }
}
