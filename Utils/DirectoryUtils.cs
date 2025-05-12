using Godot;
using System.IO;
using System;

namespace GodotUtils;

public static class DirectoryUtils
{
    /// <summary>
    /// Recursively traverses all directories and performs a action on each file path. 
    /// 
    /// <code>
    /// Traverse("res://", fullFilePath => GD.Print(fullFilePath))
    /// </code>
    /// </summary>
    public static bool Traverse(string directory, Func<string, bool> actionFullFilePath)
    {
        directory = NormalizePath(ProjectSettings.GlobalizePath(directory));
        using DirAccess dir = DirAccess.Open(directory);
        dir.ListDirBegin();

        string nextFileName;
        while ((nextFileName = dir.GetNext()) != string.Empty)
        {
            string fullFilePath = Path.Combine(directory, nextFileName);

            if (dir.CurrentIsDir())
            {
                if (!nextFileName.StartsWith('.'))
                {
                    if (Traverse(fullFilePath, actionFullFilePath))
                        return true;
                }
            }
            else
            {
                if (actionFullFilePath(fullFilePath))
                    return true;
            }
        }

        dir.ListDirEnd();
        return false;
    }


    /// <summary>
    /// Recursively searches for the file name and if found returns the full file path to
    /// that file.
    /// 
    /// <code>
    /// string fullPathToPlayer = FindFile("res://", "Player.tscn")
    /// </code>
    /// </summary>
    /// <returns>Returns the full path to the file or null if the file is not found</returns>
    public static string FindFile(string directory, string fileName)
    {
        string foundPath = null;

        Traverse(directory, fullFilePath =>
        {
            if (Path.GetFileName(fullFilePath) == fileName)
            {
                foundPath = fullFilePath;
                return true;
            }

            return false;
        });

        return foundPath;
    }

    /// <summary>
    /// Normalizes the specified path by replacing all forward slashes ('/') and backslashes ('\') with the system's directory separator character.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>A normalized path where all directory separators are replaced with the system's directory separator character.</returns>
    /// <remarks>
    /// This method is useful when dealing with paths that may come from different sources (e.g., URLs, different operating systems) and need to be standardized for the current environment.
    /// </remarks>
    public static string NormalizePath(string path)
    {
        return path
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);
    }

    /// <summary>
    /// Recursively deletes all empty folders in this folder
    /// </summary>
    public static void DeleteEmptyDirectories(string path)
    {
        path = NormalizePath(ProjectSettings.GlobalizePath(path));

        if (Directory.Exists(path))
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteEmptyDirectories(directory);
                DeleteEmptyDirectory(directory);
            }
        }
    }

    /// <summary>
    /// Checks if the folder is empty and deletes it if it is
    /// </summary>
    private static void DeleteEmptyDirectory(string path)
    {
        path = NormalizePath(ProjectSettings.GlobalizePath(path));

        if (IsDirectoryEmpty(path))
        {
            Directory.Delete(path, recursive: false);
        }
    }

    /// <summary>
    /// Checks if the directory is empty
    /// </summary>
    /// <returns>Returns true if the directory is empty</returns>
    private static bool IsDirectoryEmpty(string path)
    {
        path = NormalizePath(ProjectSettings.GlobalizePath(path));

        return Directory.GetDirectories(path).Length == 0 && Directory.GetFiles(path).Length == 0;
    }
}
