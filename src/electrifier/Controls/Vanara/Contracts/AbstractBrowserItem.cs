using CommunityToolkit.Mvvm.ComponentModel;
using electrifier.Controls.Vanara.Services;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using static Vanara.PInvoke.ComCtl32;
using static Vanara.PInvoke.Shell32;

namespace electrifier.Controls.Vanara.Contracts;

[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public abstract class AbstractBrowserItem<T>(bool isFolder, List<AbstractBrowserItem<T>>? childItems)
{
    public readonly bool IsFolder = isFolder;       // WARN: TODO: Check this. If unknown, then find it out!  ... edit: or use virtual function for this!
    public readonly List<AbstractBrowserItem<T>> ChildItems = childItems ?? [];
    public SoftwareBitmapSource SoftwareBitmapSource = isFolder
        ? IShellNamespaceService.FolderBitmapSource
        : IShellNamespaceService.DocumentBitmapSource;
    //internal void async IconUpdate(int Index, SoftwareBitmapSource bmpSrc);
    //internal void async ChildItemsIconUpdate();
    public new string ToString() => $"AbstractBrowserItem(<{typeof(T)}>(isFolder {isFolder}, childItems {childItems})";
}

[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public abstract class AbstractBrowserItemCollection<T> : IEnumerable<AbstractBrowserItem<T>>, IList<AbstractBrowserItem<T>>
{
    //protected readonly ShellItem? _parentOwnerItem;
    protected readonly IList<AbstractBrowserItem<T>> Collection = [];

    AbstractBrowserItem<T> IList<AbstractBrowserItem<T>>.this[int index] { get => Collection[index]; set => Collection[index] = value; }
    int ICollection<AbstractBrowserItem<T>>.Count => Collection.Count;
    bool ICollection<AbstractBrowserItem<T>>.IsReadOnly => false;
    void ICollection<AbstractBrowserItem<T>>.Add(AbstractBrowserItem<T> item) => Collection.Add(item);
    void ICollection<AbstractBrowserItem<T>>.Clear() => Collection.Clear();
    bool ICollection<AbstractBrowserItem<T>>.Contains(AbstractBrowserItem<T> item) => Collection.Contains(item);
    void ICollection<AbstractBrowserItem<T>>.CopyTo(AbstractBrowserItem<T>[] array, int arrayIndex) => Collection.CopyTo(array, arrayIndex);
    IEnumerator<AbstractBrowserItem<T>> IEnumerable<AbstractBrowserItem<T>>.GetEnumerator() => Collection.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Collection.GetEnumerator();
    int IList<AbstractBrowserItem<T>>.IndexOf(AbstractBrowserItem<T> item) => Collection.IndexOf(item);
    void IList<AbstractBrowserItem<T>>.Insert(int index, AbstractBrowserItem<T> item) => Collection.Insert(index, item);
    bool ICollection<AbstractBrowserItem<T>>.Remove(AbstractBrowserItem<T> item) => Collection.Remove(item);
    void IList<AbstractBrowserItem<T>>.RemoveAt(int index) => Collection.RemoveAt(index);

    public new string ToString() => $"AbstractBrowserItemCollection(<{typeof(T)}>(number of child items: {Collection.Count})";
}
