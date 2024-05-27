using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.Windows.Shell;

namespace electrifier.Controls.Vanara;

public class /*record*/ Shell32GridViewItem
{
    public string DisplayName
    {
        get;
    }

    public string HintText
    {
        get; private set;
    }

    // TODO: Use: async bitmap for XAML binding: public readonly ImageSource ImageSource;

    public bool IsFolder
    {
        get; private set;
    }

    public ShellItem ShellItem
    {
        get;
    }

    public Shell32GridViewItem(ShellItem shItem, string? overrideDisplayName = default)
    {
        ShellItem = shItem ?? throw new ArgumentNullException(nameof(shItem));
        DisplayName = overrideDisplayName ?? (ShellItem.Name ?? throw new Exception("shItem Display Name"));
        IsFolder = shItem is ShellFolder;
        HintText = (IsFolder ? "Folder: " : "File: ") + shItem.ParsingName;

        var bitmap = new BitmapImage
        {
            // TODO: make async: bitmap.SetSourceAsync("ms-appx:///...");
            UriSource = new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico")
        };
    }

    //public Shell32GridViewItem(ShellItem shItem, string displayName, string hintText, bool isFolder, ShellItem shellItem)
    //    : this(shItem, displayName)
    //{
    //    HintText = hintText ?? throw new ArgumentNullException(nameof(hintText));
    //    IsFolder = isFolder;
    //    ShellItem = shellItem ?? throw new ArgumentNullException(nameof(shellItem));
    //}

    internal static Shell32GridViewItem? Parent(ShellItem shItem)
    {
        if (shItem is null)
        {
            throw new ArgumentNullException(nameof(shItem));
        }

        if (shItem.Parent is null)
        {
            return null;
        }

        return new Shell32GridViewItem(shItem.Parent, overrideDisplayName: "..")
        {
            IsFolder = true,
            HintText = shItem.Parent.ParsingName ?? throw new Exception("shItem Display Name")
        };
    }
}
