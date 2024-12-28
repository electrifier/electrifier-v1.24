namespace electrifier.ViewModels;
using System.Diagnostics;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Vanara.Windows.Shell;
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(), nq}}")]
public partial class TextEditorViewModel : ObservableRecipient
{
    public ShellItem CurrentFolder = ShellFolder.Desktop;
    private int CursorX
    {
        get;
        set;
    } = 0;
    private int CursorY
    {
        get;
        set;
    } = 0;
    public string CursorPosition => $"Ln: {CursorY}   Ch: {CursorX}";
    private string GetDebuggerDisplay()
    {
        var dbgDisplay = new StringBuilder();
        _ = dbgDisplay.Append(nameof(TextEditorViewModel));
        return dbgDisplay.ToString();
    }
}
