using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara.Contracts;
public abstract class AbstractBrowserItem<T>(T shellItem)
{
    public T ShellItem { get; } = shellItem;
    public Shell32.IShellItemArray? GetChildArray(Shell32.SVGIO opt) => null;
}

public class KnownFolderBrowserItem(Shell32.KNOWNFOLDERID knownFolder)
    : AbstractBrowserItem<ShellItem>(new ShellFolder(knownFolder))
{
}
