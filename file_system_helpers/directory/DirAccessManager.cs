using Godot;

namespace Bombino.file_system_helpers.directory;

/// <summary>
/// Manages access to directories.
/// </summary>
internal class DirAccessManager : IDirAccessManager
{
    #region InterfaceMethods

    /// <summary>
    /// Retrieves the file names of all files in the specified directory.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <returns>
    /// A tuple containing an error code and an array of file names.
    /// The error code indicates whether the operation was successful or not.
    /// The array of file names contains the names of all files in the directory.
    /// </returns>
    public (Error, string[]) GetFileNames(string path)
    {
        using var dir = DirAccess.Open(path);
        if (IsThereOpenError(dir))
        {
            HandleError(path);

            return (Error.Failed, null);
        }

        return (Error.Ok, dir.GetFiles());
    }

    /// <summary>
    /// Creates a directory at the specified path with the specified name.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    /// <param name="name">The name of the directory to create.</param>
    /// <returns>
    /// An error code which indicates whether the operation was successful or not.
    /// </returns>
    public Error MakeDirectory(string path, string name)
    {
        using var dir = DirAccess.Open(path);
        if (IsThereOpenError(dir))
        {
            HandleError(path);

            return Error.Failed;
        }

        var error = dir.MakeDir(name);
        if (error == Error.AlreadyExists)
        {
            return error;
        }

        if (error != Error.Ok)
        {
            GD.PushError(
                $"An error occurred when trying to create the directory ({name}): {error}"
            );
        }

        return error;
    }

    /// <summary>
    /// Removes the file at the specified absolute path.
    /// </summary>
    /// <param name="path">The absolute path of the file to remove.</param>
    /// <returns>
    /// An error code which indicates whether the operation was successful or not.
    /// </returns>
    public Error RemoveFileAbsolute(string path)
    {
        var error = DirAccess.RemoveAbsolute(path);
        if (error != Error.Ok)
        {
            GD.PushError($"An error occurred when trying to remove the file ({path}): {error}");
        }

        return error;
    }

    #endregion

    /// <summary>
    /// Checks if there is a directory open error.
    /// </summary>
    /// <param name="dir">The directory access object.</param>
    /// <returns>True if there is a file open error, false otherwise.</returns>
    private static bool IsThereOpenError(DirAccess dir)
    {
        return dir == null;
    }

    /// <summary>
    /// Handles errors that occur during directory access.
    /// </summary>
    /// <param name="path">The path of the directory.</param>
    private static void HandleError(string path)
    {
        GD.PushError(
            $"An error occurred when trying to access the path ({path}): {DirAccess.GetOpenError()}"
        );
    }
}
