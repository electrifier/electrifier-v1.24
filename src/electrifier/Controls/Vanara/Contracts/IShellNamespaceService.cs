using System.Runtime.InteropServices;
using electrifier.Controls.Vanara.Services;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.PInvoke;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara.Contracts;

public interface IShellNamespaceService
{
    /// <summary><see cref="HRESULT"/> code of <see cref="COMException"/><i>('0x80070490');</i>
    /// <remarks>Fired when <b>`Element not found`</b> while enumerating the Shell32 Namespace.</remarks>
    /// <remarks>As far as I know, this also gets fired when <b>No Disk in Drive</b> error occurs.</remarks></summary>
    public static readonly HRESULT HResultElementNotFound = new(0x80070490);
    /// <summary><see cref="ShellFolder"/> of virtual `<b>Home</b>` directory.
    /// <remarks>This equals Shell 32 URI: <code>shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}</code></remarks></summary>
    public static ShellFolder HomeShellFolder => new("shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}");


}