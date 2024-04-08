namespace Bombino.scripts.persistence;

using Godot;
using Godot.Collections;

/// <summary>
/// Represents a game save file.
/// </summary>
internal class GameSave
{
    private const string GameSavePath = "user://save.json";

    public static Dictionary<string, Variant> Data { get; private set; }

    /// <summary>
    /// Writes the game save data to the file.
    /// </summary>
    /// <param name="data">The game save data to write.</param>
    public static void WriteSave(Dictionary<string, Variant> data)
    {
        GetFileAccessAndSaveDataToFile(data);
    }

    /// <summary>
    /// Loads the game save data from the file.
    /// </summary>
    public static void LoadSave()
    {
        GetFileAccessAndLoadDataFromFile();
    }

    /// <summary>
    /// Checks if a game save file exists.
    /// </summary>
    /// <returns><c>true</c> if the game save file exists; otherwise, <c>false</c>.</returns>
    public static bool IsSaveExits()
    {
        return FileAccess.FileExists(GameSavePath);
    }

    /// <summary>
    /// Gets file access and saves the data to a file.
    /// </summary>
    /// <param name="data">The data to be saved.</param>
    private static void GetFileAccessAndSaveDataToFile(Dictionary<string, Variant> data)
    {
        using var file = GetFileAccess(FileAccess.ModeFlags.Write);

        if (IsThereFileOpenError(file))
            return;

        SaveStringifiedDataToFile(data, file);
    }

    /// <summary>
    /// Provides access to a file.
    /// </summary>
    private static FileAccess GetFileAccess(FileAccess.ModeFlags modeFlag)
    {
        return FileAccess.Open(GameSavePath, modeFlag);
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

        GD.PrintErr(FileAccess.GetOpenError());
        return true;
    }

    /// <summary>
    /// Saves the stringified data to a file.
    /// </summary>
    /// <param name="data">The dictionary containing the data to be saved.</param>
    /// <param name="file">The file access object used to store the data.</param>
    private static void SaveStringifiedDataToFile(Dictionary<string, Variant> data, FileAccess file)
    {
        file.StoreString(Json.Stringify(data));
    }

    /// <summary>
    /// Gets file access and loads data from the file.
    /// </summary>
    private static void GetFileAccessAndLoadDataFromFile()
    {
        using var file = GetFileAccess(FileAccess.ModeFlags.Read);

        if (IsThereFileOpenError(file))
            return;

        LoadStringifiedDataFromFile(file);
    }

    /// <summary>
    /// Loads stringified data from a file and sets the data.
    /// </summary>
    /// <param name="file">The file access object.</param>
    private static void LoadStringifiedDataFromFile(FileAccess file)
    {
        var data = GetDataFromFile(file);

        SetData(data);
    }

    /// <summary>
    /// Retrieves data from a file and returns it as a dictionary.
    /// </summary>
    /// <param name="file">The file to retrieve data from.</param>
    /// <returns>A dictionary containing the retrieved data.</returns>
    private static Dictionary<string, Variant> GetDataFromFile(FileAccess file)
    {
        var data = Json.ParseString(file.GetAsText()).AsGodotDictionary<string, Variant>();

        return data;
    }

    /// <summary>
    /// Sets the data for the game save.
    /// </summary>
    /// <param name="data">The dictionary containing the data to be set.</param>
    private static void SetData(Dictionary<string, Variant> data)
    {
        Data = data;
    }
}
