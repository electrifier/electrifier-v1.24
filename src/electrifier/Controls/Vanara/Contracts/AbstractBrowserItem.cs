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

/// <summary>
/// Abstract base class of an <see cref="ExplorerBrowser"/> Item.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <param name="abstractItem"></param>
/// <param name="id"></param>
/// <param name="isFolder"></param>
/// <param name="childItems"></param>
[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public abstract class AbstractBrowserItem<T, T2>(T abstractItem, T2 id, bool isFolder, AbstractBrowserItemCollection<T, T2> childItems)
{
    public T Item = abstractItem;
    public T2 Id = id;
    public bool IsFolder = isFolder;
    public AbstractBrowserItemCollection<T, T2> ChildItems = childItems;
    public new string ToString() => $"AbstractBrowserItem<T {typeof(T)}>({abstractItem})";
}
/// <summary>
/// Base class for BrowserItemCollection
/// <typeparam name="T">BrowserItem</typeparam>
/// <typeparam name="T2">BrowserItemIdentifier</typeparam>
/// </summary>
[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public abstract class AbstractBrowserItemCollection<T, T2> : ICollection<T>
{
    public abstract int Count
    {
        get;
    }
    public abstract bool IsReadOnly
    {
        get;
    }
    public abstract void Add(T item);
    public abstract void Clear();
    public abstract bool Contains(T item);
    public abstract void CopyTo(T[] array, int arrayIndex);
    public abstract IEnumerator<T> GetEnumerator();
    public abstract bool Remove(T item);
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    public new string ToString() => $"AbstractBrowserItemCollection<T {typeof(T)}>({this})";
}



public class ExplorerBrowserItem : AbstractBrowserItem<ShellItem, Shell32.PIDL>
{
    public ExplorerBrowserItem(ShellItem shItem,
        ExplorerBrowserItemCollection children) : base(shItem,
        shItem.PIDL,
        shItem.IsFolder,
        children)
    {
    }
    public ExplorerBrowserItem(ShellItem shItem) : base(shItem,
        shItem.PIDL,
        shItem.IsFolder,
        new ExplorerBrowserItemCollection())
    {
    }

    public static ExplorerBrowserItem Create(ShellItem shItem) => new ExplorerBrowserItem(shItem);
}
public partial class ExplorerBrowserItemCollection : AbstractBrowserItemCollection<ShellItem, Shell32.PIDL>, IList<ShellItem>, IList
{
    private IList<ShellItem> _listImplementation;

    public override bool Remove(ShellItem item) => throw new NotImplementedException();

    public override int Count
    {
        get;
    }

    public override bool IsReadOnly { get; }

    public bool IsFixedSize => throw new NotImplementedException();

    public bool IsSynchronized => throw new NotImplementedException();

    public object SyncRoot => throw new NotImplementedException();

    object? IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Add(ShellItem item) => _listImplementation.Add(item);

    public override void Clear() => throw new NotImplementedException();

    public override bool Contains(ShellItem item) => throw new NotImplementedException();

    public override void CopyTo(ShellItem[] array, int arrayIndex) => throw new NotImplementedException();

    public override IEnumerator<ShellItem> GetEnumerator() => throw new NotImplementedException();
    public int IndexOf(ShellItem item) => _listImplementation.IndexOf(item);

    public void Insert(int index, ShellItem item) => _listImplementation.Insert(index, item);

    public void RemoveAt(int index) => _listImplementation.RemoveAt(index);
    public int Add(object? value) => throw new NotImplementedException();
//    public override void Add(ShellItem item) => throw new NotImplementedException();
    public bool Contains(object? value) => throw new NotImplementedException();
    public int IndexOf(object? value) => throw new NotImplementedException();
    public void Insert(int index, object? value) => throw new NotImplementedException();
    public void Remove(object? value) => throw new NotImplementedException();
    public void CopyTo(Array array, int index) => throw new NotImplementedException();

    public ShellItem this[int index]
    {
        get => _listImplementation[index];
        set => _listImplementation[index] = value;
    }
}







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
