using CommunityToolkit.Mvvm.ComponentModel;
using electrifier.Controls.Vanara.Services;

namespace electrifier.ViewModels;

public partial class FileManagerViewModel : ObservableRecipient
{
    public readonly ShellNamespaceService ShellNamespaceService = new();

    /// <summary>FileManagerViewModel</summary>
    public FileManagerViewModel()
    {
    }
}
