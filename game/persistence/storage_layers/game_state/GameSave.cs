using Godot;
using FileAccess = Godot.FileAccess;

namespace Bombino.game.persistence.storage_layers.game_state;

/// <summary>
/// Represents a game save file.
/// </summary>
internal class GameSave : IGameSave<Godot.Collections.Dictionary<string, Variant>>
{
    #region Fields

    private const string GameSavePath = "user://save.json";

    #endregion

    #region InterfaceMethods

    /// <summary>
    /// Loads the game save data from the file.
    /// </summary>
    public (bool, FileAccess) LoadFile(string path, FileAccess.ModeFlags modeFlags)
    {
        using var file = GetFileAccess(path, modeFlags);

        return (IsThereFileOpenError(file), file);
    }

    /// <summary>
    /// Writes the game save data to the file.
    /// </summary>
    /// <param name="data">The game save data to write.</param>
    public bool SaveData(Godot.Collections.Dictionary<string, Variant> data)
    {
        var (error, file) = LoadFile(GameSavePath, FileAccess.ModeFlags.Write);

        if (error) return false;

        SaveDataToFile(file, data);

        return true;
    }

    /// <summary>
    /// Retrieves data from a file and returns it as a dictionary.
    /// </summary>
    /// <returns>A dictionary containing the retrieved data.</returns>
    public Godot.Collections.Dictionary<string, Variant> GetData(FileAccess file)
    {
        var data = Json.ParseString(file.GetAsText()).AsGodotDictionary<string, Variant>();

        return data;
    }

    #endregion

    /// <summary>
    /// Provides access to a file.
    /// </summary>
    private static FileAccess GetFileAccess(string path, FileAccess.ModeFlags modeFlag)
    {
        return FileAccess.Open(path, modeFlag);
    }

    /// <summary>
    /// Checks if there is a file open error.
    /// </summary>
    /// <param name="file">The file to check.</param>
    /// <returns>True if there is a file open error, false otherwise.</returns>
    private static bool IsThereFileOpenError(FileAccess file)
    {
        if (file != null)
            return false;

        GD.PushError(FileAccess.GetOpenError());

        return true;
    }

    /// <summary>
    /// Saves the stringified data to a file.
    /// </summary>
    /// <param name="data">The dictionary containing the data to be saved.</param>
    /// <param name="file">The file access object used to store the data.</param>
    private static void SaveDataToFile(FileAccess file, Godot.Collections.Dictionary<string, Variant> data)
    {
        file.StoreString(Json.Stringify(data));
    }
}