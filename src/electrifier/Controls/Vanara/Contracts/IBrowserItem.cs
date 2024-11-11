using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara.Contracts;
public interface IBrowserItem
{
    public virtual Shell32.IShellItemArray? GetChildArray(Shell32.SVGIO opt) => null;

}
