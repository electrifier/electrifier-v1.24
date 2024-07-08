using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI;
using electrifier.Controls.Vanara;
using Vanara.Windows.Shell;

namespace electrifier.ViewModels;

public partial class TextEditorViewModel : ObservableRecipient
{
    public ShellItem CurrentFolder;

    public string CursorPosition => $"Column: {CursorX} Line: {CursorY}";
    public int CursorX { get; set; } = 666;
    public int CursorY { get; set; } = 0;

    public TextEditorViewModel() => CurrentFolder = ShellFolder.Desktop;
}
