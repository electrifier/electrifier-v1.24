using CommunityToolkit.Mvvm.ComponentModel;
using Vanara.Windows.Shell;

namespace electrifier.ViewModels;

public partial class TextEditorViewModel : ObservableRecipient
{
    public ShellItem CurrentFolder = ShellFolder.Desktop;

    public string CursorPosition => $"Column: {CursorX} Line: {CursorY}";
    public int CursorX { get; set; } = 0;
    public int CursorY { get; set; } = 0;
}
