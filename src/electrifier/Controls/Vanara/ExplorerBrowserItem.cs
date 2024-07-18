using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

/// <summary>
/// A ViewModel for both <see cref="Shell32GridView"/> and <see cref="Shell32GridView"/> Items.
/// </summary>

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public class ExplorerBrowserItem : INotifyPropertyChanged
{
    // primary properties
    public string DisplayName
    {
        get;
    }
    public ShellItem ShellItem
    {
        get;
    }
    public List<ExplorerBrowserItem>? Children;

    public bool IsFolder
    {
        get;
    }
    public bool HasUnrealizedChildren
    {
        get;
        private set;
    }
    public ImageSource? ImageIconSource
    {
        get;
        internal set;
    }

    private bool _isExpanded;
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded != value)
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsLink
    {
        get;
    }
    public bool IsSelected
    {
        get; set;
    }


    // TODO: TreeViewNode - Property
    // TODO: GridViewItem - Property
    // TODO: ExplorerBrowserItem.TreeNodeSelected = bool; => Initiate selection of this node
    public ExplorerBrowserItem(ShellItem shItem)
    {
        ShellItem = shItem ?? throw new ArgumentNullException(nameof(shItem));
        DisplayName = ShellItem.Name ?? throw new Exception("shItem Display Name");
        HasUnrealizedChildren = (shItem.Attributes.HasFlag(ShellItemAttribute.HasSubfolder));
        IsExpanded = false;
        IsFolder = shItem.IsFolder;
        IsLink = shItem.IsLink;
        IsSelected = false;

        //Debug.WriteLine($"{GetDebuggerDisplay()} created.");
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
