using Godot;
using FileAccess = Godot.FileAccess;

namespace Bombino.file_system_helpers.file;

/// <summary>
/// Manages access to files.
/// </summary>
internal class FileAccessManager : IFileAccessManager
{
    #region InterfaceMethods

    /// <summary>
    /// Loads a file from the specified path with the given mode flags.
    /// </summary>
    /// <param name="path">The path of the file to load.</param>
    /// <param name="modeFlags">The mode flags specifying the access mode for the file.</param>
    /// <returns>A tuple containing a boolean indicating whether the file was successfully loaded and a FileAccess object representing the access mode for the file.</returns>
    public (Error, FileAccess) LoadFile(string path, FileAccess.ModeFlags modeFlags)
    {
        var file = OpenFile(path, modeFlags);
        var error = IsThereOpenError(file);
        if (error)
        {
            HandleError(path);

            return (Error.Failed, file);
        }

        return (Error.Ok, file);
    }

    /// <summary>
    /// Retrieves the JSON data from the specified file.
    /// </summary>
    /// <param name="file">The file to retrieve the JSON data from.</param>
    /// <returns>The JSON data as a string.</returns>
    public Godot.Collections.Dictionary<string, Variant> GetJsonData(FileAccess file)
    {
        var jsonData = Json.ParseString(GetFileContent(file)).AsGodotDictionary<string, Variant>();

        return jsonData;
    }

    /// <summary>
    /// Stores JSON data in the specified file.
    /// </summary>
    /// <param name="file">The file access object.</param>
    /// <param name="data">The JSON data to store.</param>
    public void StoreJSONData(FileAccess file, Godot.Collections.Dictionary<string, Variant> data)
    {
        var JSONData = Json.Stringify(data);

        file.StoreString(JSONData);
    }

    #endregion

    /// <summary>
    /// Opens a file with the specified path and mode flag.
    /// </summary>
    /// <param name="path">The path of the file to open.</param>
    /// <param name="modeFlag">The mode flag specifying the file access mode.</param>
    /// <returns>A <see cref="FileAccess"/> object representing the opened file.</returns>
    private static FileAccess OpenFile(string path, FileAccess.ModeFlags modeFlag)
    {
        return FileAccess.Open(path, modeFlag);
    }

    /// <summary>
    /// Checks if there is a file open error.
    /// </summary>
    /// <param name="file">The file access object.</param>
    /// <returns>True if there is a file open error, false otherwise.</returns>
    private static bool IsThereOpenError(FileAccess file)
    {
        return file == null;
    }

    /// <summary>
    /// Handles errors that occur during file access.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    private static void HandleError(string path)
    {
        GD.PushError(
            $"An error occurred when trying to access the path ({path}): {FileAccess.GetOpenError()}"
        );
    }

    /// <summary>
    /// Retrieves the content of a file.
    /// </summary>
    /// <param name="file">The file to retrieve the content from.</param>
    /// <returns>The content of the file as a string.</returns>
    private static string GetFileContent(FileAccess file)
    {
        var content = file.GetAsText();

        return content;
    }
}
