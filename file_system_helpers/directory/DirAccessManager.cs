using Godot;

namespace Bombino.file_system_helpers.directory;

internal class DirAccessManager : IDirAccessManager
{
    #region InterfaceMethods

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
