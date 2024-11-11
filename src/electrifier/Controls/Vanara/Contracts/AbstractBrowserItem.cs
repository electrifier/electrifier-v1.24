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
using static Vanara.PInvoke.ComCtl32;

namespace electrifier.Controls.Vanara.Contracts;

[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public abstract class AbstractBrowserItem<T>(bool isFolder, AbstractBrowserItemCollection<T> childItems)
{
    public readonly bool IsFolder = isFolder;
    public readonly AbstractBrowserItemCollection<T> ChildItems = childItems;
    public new string ToString() => $"AbstractBrowserItem<{typeof(T)}>('no_folder')";
}

[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public abstract class AbstractBrowserItemCollection<T> : IEnumerable<AbstractBrowserItem<T>>, IList<AbstractBrowserItem<T>>
{
    private readonly IList<AbstractBrowserItem<T>> _collection = [];

    AbstractBrowserItem<T> IList<AbstractBrowserItem<T>>.this[int index] { get => _collection[index]; set => _collection[index] = value; }
    int ICollection<AbstractBrowserItem<T>>.Count => _collection.Count;
    bool ICollection<AbstractBrowserItem<T>>.IsReadOnly => false;
    void ICollection<AbstractBrowserItem<T>>.Add(AbstractBrowserItem<T> item) => _collection.Add(item);
    void ICollection<AbstractBrowserItem<T>>.Clear() => _collection.Clear();
    bool ICollection<AbstractBrowserItem<T>>.Contains(AbstractBrowserItem<T> item) => _collection.Contains(item);
    void ICollection<AbstractBrowserItem<T>>.CopyTo(AbstractBrowserItem<T>[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);
    IEnumerator<AbstractBrowserItem<T>> IEnumerable<AbstractBrowserItem<T>>.GetEnumerator() => _collection.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();
    int IList<AbstractBrowserItem<T>>.IndexOf(AbstractBrowserItem<T> item) => _collection.IndexOf(item);
    void IList<AbstractBrowserItem<T>>.Insert(int index, AbstractBrowserItem<T> item) => _collection.Insert(index, item);
    bool ICollection<AbstractBrowserItem<T>>.Remove(AbstractBrowserItem<T> item) => _collection.Remove(item);
    void IList<AbstractBrowserItem<T>>.RemoveAt(int index) => _collection.RemoveAt(index);
}
