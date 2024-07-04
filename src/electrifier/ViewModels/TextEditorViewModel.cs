using CommunityToolkit.Mvvm.ComponentModel;

namespace electrifier.ViewModels;

public partial class TextEditorViewModel : ObservableRecipient
{
    public string StatusCursorPosition
    {
        get;
        internal set;
    }

    public TextEditorViewModel()
    {
        StatusCursorPosition = SetCursorPosition(0, 0);
    }

    internal string SetCursorPosition(int x, int y)
    {
        return StatusCursorPosition = $"x: {x} y: {y}";
    }
}
