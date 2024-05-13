using Godot;
using FileAccess = Godot.FileAccess;

namespace Bombino.file_system_helpers.file;

/// <summary>
/// Interface for managing file access.
/// </summary>
public interface IFileAccessManager
{
    /// <summary>
    /// Loads a file from the specified path with the given mode flags.
    /// </summary>
    /// <param name="path">The path of the file to load.</param>
    /// <param name="modeFlags">The mode flags specifying the access mode for the file.</param>
    /// <returns>A tuple containing a boolean indicating whether the file was successfully loaded and a FileAccess object representing the access mode for the file.</returns>
    public (Error, FileAccess) LoadFile(string path, FileAccess.ModeFlags modeFlags);

    /// <summary>
    /// Retrieves the JSON data from the specified file.
    /// </summary>
    /// <param name="file">The file to retrieve the JSON data from.</param>
    /// <returns>The JSON data as a string.</returns>
    public Godot.Collections.Dictionary<string, Variant> GetJsonData(FileAccess file);

    /// <summary>
    /// Stores JSON data in the specified file.
    /// </summary>
    /// <param name="file">The file access object.</param>
    /// <param name="data">The JSON data to store.</param>
    public void StoreJSONData(FileAccess file, Godot.Collections.Dictionary<string, Variant> data);
}
