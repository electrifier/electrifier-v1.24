using CommunityToolkit.Mvvm.ComponentModel;
using electrifier.Controls.Vanara.Services;

namespace electrifier.ViewModels;

public partial class FileManagerViewModel : ObservableRecipient
{
    public readonly ShellNamespaceService ShellNamespaceService = new ShellNamespaceService();

    /// <summary>FileManagerViewModel</summary>
    public FileManagerViewModel()
    {
        var tsk = ShellNamespaceService.InitializeStockIcons();
    }
}
