using System.Net.NetworkInformation;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace electrifier.ViewModels;

public partial class FileManagerViewModel : ObservableRecipient
{
    public ObservableCollection<string> Folders { get; } = new ObservableCollection<string>();

    public FileManagerViewModel()
    {
        Task.Run(() => EnumerateFolders(@"C:\"));
    }

    private void EnumerateFolders(string rootPath)
    {
        try
        {
            foreach (var directory in Directory.EnumerateDirectories(rootPath))
            {
                Folders.Add(directory);
                EnumerateFolders(directory);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Handle access denied exceptions
        }
    }
}