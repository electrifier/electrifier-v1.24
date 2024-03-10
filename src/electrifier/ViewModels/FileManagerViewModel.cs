using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace electrifier.ViewModels;

public partial class FileManagerViewModel : ObservableRecipient
{
    public ObservableCollection<string> Folders { get; } = new ObservableCollection<string>();

    /// <summary>
    /// 
    /// </summary>
    public FileManagerViewModel()
    {
    }

    //    private readonly Task enumerateFoldersTask = Task.Run(() => EnumerateFolders(@"C:\", true));
    //
    //    /// <summary>
    //    /// functionality to enumerate folders
    //    /// <throws cref="DirectoryNotFoundException"></throws>
    //    /// <throws cref="UnauthorizedAccessException"></throws>
    //    /// <throws cref="PathTooLongException"></throws>
    //    /// </summary>
    //    /// 
    //    public static string EnumerateFolders(string rootPath, bool enumerateChildFoldersIfAllowed = false)
    //    {
    //        try
    //        {
    //            foreach (var dirName in Directory.EnumerateDirectories(rootPath))
    //            {
    //                return dirName;
    //                //Folders.Add(dirName);
    //
    //                //if (enumerateChildFoldersIfAllowed)
    //                //{
    //                //    EnumerateFolders(dirName, false);
    //                //}
    //            }
    //        }
    //        catch (DirectoryNotFoundException ex)
    //        {
    //            throw new DirectoryNotFoundException(rootPath, ex);
    //        }
    //        catch (UnauthorizedAccessException ex)
    //        {
    //            throw new UnauthorizedAccessException(rootPath, ex);
    //        }
    //        catch (PathTooLongException ex)
    //        {
    //            throw new PathTooLongException(rootPath, ex);
    //        }
    //
    //        return "{ unknown error in $EnumerateFolders }";
    //    }
}
