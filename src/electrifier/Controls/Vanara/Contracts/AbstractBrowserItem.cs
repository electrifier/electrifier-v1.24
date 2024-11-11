using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara.Contracts;
public class AbstractBrowserItem
{
    internal Shell32.IShellItemArray? GetChildArray(Shell32.SVGIO opt) => null;

    public abstract class ABrowserItem(ShellItem shellItem) : ObservableObject
    {
        private readonly ShellItem _shellItem = shellItem;
    }
}
