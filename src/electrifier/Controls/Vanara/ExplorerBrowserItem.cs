using Vanara.Windows.Shell;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.ComponentModel;
using electrifier.Controls.Vanara.Services;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.PInvoke;

namespace electrifier.Controls.Vanara;

// TODO: TreeViewNode - Property, events
// TODO: GridViewItem - Property, events

/// <summary>ViewModel for both <see cref="Shell32GridView"/> and <see cref="Shell32TreeView"/> Items.</summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public class ExplorerBrowserItem(Shell32.PIDL shItemId, SoftwareBitmapSource? bitmapSource = null) : IDisposable, INotifyPropertyChanged
{
    public SoftwareBitmapSource BitmapSource { get; set; } = bitmapSource ?? ShellNamespaceService.DefaultDocumentAssocImageBitmapSource;
    /// <summary>Get the current set of child items. <seealso cref="ExplorerBrowserItem"/>s as <seealso cref="List{T}"/>.</summary>
    public List<ExplorerBrowserItem> Children = [];
    /// <summary>Get the DisplayName.</summary>
    public string DisplayName => ShellItem.Name ?? ":error: <DisplayName.get()>";
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
    public bool IsProgressing;
    public bool IsSelected { get; set; }
    public double Opacity { get; set; } = 1;
    public ShellItem ShellItem { get; set; } = new ShellItem(shItemId);
    private bool _disposedValue;
    private readonly int? _imageListIndex;

    public ExplorerBrowserItem(ShellItem childItem) : this(childItem.PIDL) { }
    public ExplorerBrowserItem(Shell32.KNOWNFOLDERID kfId) : this(new ShellFolder(kfId).PIDL)
    {
        BitmapSource = ShellNamespaceService.DefaultFolderImageBitmapSource;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public event PropertyChangedEventHandler? PropertyChanged;
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

    #region Dispose pattern // todo: not implemented yet
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~ExplorerBrowserItem()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion Dispose pattern

    #region GetDebuggerDisplay()
    private string GetDebuggerDisplay()
    {
        var sb = new StringBuilder();
        sb.Append($"<{nameof(ExplorerBrowserItem)}> `{DisplayName}`");

        if (IsFolder) { sb.Append(", [folder]"); }

        return sb.ToString();
    }
    #endregion GetDebuggerDisplay()
}
