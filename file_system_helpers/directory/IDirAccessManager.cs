using Godot;

namespace Bombino.file_system_helpers.directory;

internal interface IDirAccessManager
{
    /// <summary>
    /// Retrieves the file names of all files in the specified directory.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>
    /// A tuple containing an error code and an array of file names.
    /// The error code indicates whether the operation was successful or not.
    /// The array of file names contains the names of all files in the directory.
    /// </returns>
    (Error, string[]) GetFileNames(string path);

    /// <summary>
    /// Creates a directory at the specified path with the specified name.
    /// </summary>
    /// <returns>
    /// An error code which indicates whether the operation was successful or not.
    /// </returns>
    Error MakeDirectory(string path, string name);
}
