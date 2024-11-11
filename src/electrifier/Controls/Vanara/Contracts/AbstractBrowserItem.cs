using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara.Contracts;

[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public abstract class AbstractBrowserItem<T>(T abstractItem)
{
    public T AbstractItem { get; } = abstractItem;

    public ShellItem ShellItem = global::Vanara.Windows.Shell.ShellItem.Open(Shell32.PIDL.Null);
    public Shell32.IShellItemArray GetChildArray(Shell32.SVGIO opt) => null;
    public new string ToString() => $"AbstractBrowserItem<T {typeof(T)}>(T)";
}

//public class KnownFolderBrowserItem(Shell32.KNOWNFOLDERID knownFolder)
//    : AbstractBrowserItem<ShellItem>(new ShellFolder(knownFolder))
//{
//}

//public class FolderBrowserItem(Shell32.Folder folder)
//    : AbstractBrowserItem<ShellItem>(new ShellItem(folder)), Shell32.Folder
//{

//}
