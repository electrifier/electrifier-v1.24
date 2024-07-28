using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

/// <summary>
/// A ViewModel for both <see cref="Shell32GridView"/> and <see cref="Shell32TreeView"/> Items.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public class ExplorerBrowserItem /* : INotifyPropertyChanged */
{
    public List<ExplorerBrowserItem>? Children;
    public string DisplayName
    {
        get;
    }
    public bool HasUnrealizedChildren
    {
        get;
        private set;
    }
    public ImageEx? ImageIconSource
    {
        get;
        internal set;
    }
    public bool IsExpanded
    {
        get; set;
    }
    public bool IsFolder => ShellItem.IsFolder;
    public bool IsLink => ShellItem.IsLink;
    public bool IsSelected
    {
        get; set;
    }
    public ShellItem ShellItem
    {
        get;
    }


    // TODO: TreeViewNode - Property
    // TODO: GridViewItem - Property
    public ExplorerBrowserItem(ShellItem shItem)
    {
        ShellItem = new(shItem.PIDL);
        DisplayName = ShellItem.Name ?? ":error: <DisplayName.get()>";

        if (ShellItem.IsFolder)
        {
            // TODO: Check, since this call has failed in case of TeeView/GridView navigation:
            HasUnrealizedChildren = (ShellItem.Attributes.HasFlag(ShellItemAttribute.HasSubfolder));
        }

        IsExpanded = false;
        // TODO: If IsSelected, add overlay of opened folder icon to TreeView
        IsSelected = false;

        //Debug.Print($"ExplorerBrowserItem <{GetDebuggerDisplay()}> created.");
    }

    #region GetDebuggerDisplay()
    private string GetDebuggerDisplay()
    {
        var sb = new StringBuilder();
        sb.Append($"<{nameof(ExplorerBrowserItem)}> `{DisplayName}`");

        if (IsFolder) { sb.Append(", [folder]"); }

        return sb.ToString();
    }
    #endregion

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
