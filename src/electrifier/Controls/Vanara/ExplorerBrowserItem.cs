using Vanara.Windows.Shell;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;

namespace electrifier.Controls.Vanara;

// TODO: TreeViewNode - Property, events
// TODO: GridViewItem - Property, events

/// <summary>
/// A ViewModel for both <see cref="Shell32GridView"/> and <see cref="Shell32TreeView"/> Items.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public class ExplorerBrowserItem : INotifyPropertyChanged
{
    public record ExplorerBrowserItemViewState
    {
        public ExplorerBrowserItemViewState(bool transparentHiddenFiles)
        {
            TransparentHiddenFiles = transparentHiddenFiles;
        }

        private bool TransparentHiddenFiles
        {
            get;
            set;
        }
    }
    public SoftwareBitmapSource? BitmapSource { get; set; }

    /// <summary>Get the current set of <seealso cref="ExplorerBrowserItem"/>s as <seealso cref="List{T}"/>.</summary>
    public List<ExplorerBrowserItem>? Children;
    /// <summary>Get the DisplayName.</summary>
    public string DisplayName
    {
        get;
    }
    /// <summary>
    /// The specified folders have subfolders. The SFGAO_HASSUBFOLDER attribute is only advisory and might be returned by Shell folder implementations even if they do not contain subfolders. Note, however, that the converse—failing to return SFGAO_HASSUBFOLDER—definitively states that the folder objects do not have subfolders.
    /// Returning SFGAO_HASSUBFOLDER is recommended whenever a significant amount of time is required to determine whether any subfolders exist. For example, the Shell always returns SFGAO_HASSUBFOLDER when a folder is located on a network drive.
    /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/shell/sfgao"/>
    /// </summary>
    public bool HasUnrealizedChildren
    {
        get
        {
            try
            {
                if (ShellItem.Attributes.HasFlag(ShellItemAttribute.HasSubfolder))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                Debug.Print("HasUnrealizedChildren failed!");
            }
            return false;
        }
    }
    public bool IsExpanded { get; set; }
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

    public ExplorerBrowserItem(ShellItem? shItem, bool isSeparator = false)
    {
        ShellItem = new ShellItem(shItem.PIDL);
        DisplayName = ShellItem.Name ?? ":error: <DisplayName.get()>";
        IsExpanded = false;
        // todo: If IsSelected, add overlay of opened folder icon to TreeView optionally
        IsSelected = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public static ExplorerBrowserItem ExplorerBrowserSeparator()
    {
        return new ExplorerBrowserItem(null, true);
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
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
}
