using System.IO;
using UnityEngine;

/// <summary>
/// If we try to delete a directory, sometimes it gives error, because it cannot be deleted, because it is not empty. The DeleteDirectory function deletes the directory even if it is not empty.
/// </summary>

public class DirectoryHandler : MonoBehaviour
{
    public void DeleteDirectory(string directoryPath)
    {
        DeleteDirectoryContent(directoryPath); 
        Directory.Delete(directoryPath);
    }

    public void DeleteDirectoryContent(string directoryPath)
    {
        DirectoryInfo DirectoryInfo = new DirectoryInfo(directoryPath);
        FileInfo[] Files = DirectoryInfo.GetFiles();
        foreach (FileInfo File in Files)
        {
            File.Delete();
        }
        foreach (DirectoryInfo Subdirectory in DirectoryInfo.GetDirectories())
        {
            DeleteDirectoryContent(Subdirectory.FullName);
            Subdirectory.Delete();
        } 
    }
}