using System;
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
public abstract class AbstractBrowserItem<T>
{
    public T AbstractItem { get; }
    public bool IsFolder;
    public ShellItem ShellItem = global::Vanara.Windows.Shell.ShellItem.Open(Shell32.PIDL.Null);
    public Shell32.IShellItemArray GetChildArray() => null;
    public new string ToString() => $"AbstractBrowserItem<T {typeof(T)}>({AbstractItem})";
}

[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public abstract class Shell32BrowserItem : AbstractBrowserItem<ShellItem>
{
    protected Shell32BrowserItem(ShellItem shItem) { }
    //internal static Shell32BrowserItem CreateInstance(ShellItem shItem)
    //{
    //    return new Shell32BrowserItem(shItem);
    //}

    public new string ToString() => $"Shell32BrowserItem>({AbstractItem})";
}

public class FolderItem(ShellFolder shellFolder) : Shell32BrowserItem(shellFolder);




//public class FolderBrowserItem<T>() : AbstractBrowserItem<T>(folderItem)
//{
//    //public FolderBrowserItem<T>(T folderItem)
//    //{
//    //}
//}

//public class KnownFolderBrowserItem<T>(T folderItem) : AbstractBrowserItem<T>(folderItem)
//{
//    //KnownFolderBrowserItem(Shell32.KNOWNFOLDER_DEFINITION)
//    KnownFolderBrowserItem(Shell32.KNOWNFOLDERID);

//}

//public class KnownFolderBrowserItem(Shell32.KNOWNFOLDERID knownFolder)
//    : AbstractBrowserItem<ShellItem>(new ShellFolder(knownFolder))
//public class FolderBrowserItem(Shell32.Folder folder)
//    : AbstractBrowserItem<ShellItem>(new ShellItem(folder)), Shell32.Folder { }
