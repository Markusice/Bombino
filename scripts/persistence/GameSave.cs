namespace Bombino.scripts.persistence;

using Godot;
using Godot.Collections;

internal class GameSave
{
    private const string GameSavePath = "user://save.json";

    internal static Dictionary<string, Variant> Data { get; private set; }

    internal static void WriteSave(Dictionary<string, Variant> data)
    {
        GetFileAccessAndSaveDataToFile(data);
    }

    internal static void LoadSave()
    {
        GetFileAccessAndLoadDataFromFile();
    }

    internal static bool IsSaveExits()
    {
        return FileAccess.FileExists(GameSavePath);
    }

    private static void GetFileAccessAndSaveDataToFile(Dictionary<string, Variant> data)
    {
        using var file = GetFileAccess(FileAccess.ModeFlags.Write);

        if (IsThereFileOpenError(file))
            return;

        SaveStringifiedDataToFile(data, file);
    }

    private static FileAccess GetFileAccess(FileAccess.ModeFlags modeFlag)
    {
        return FileAccess.Open(GameSavePath, modeFlag);
    }

    private static bool IsThereFileOpenError(FileAccess file)
    {
        if (file != null)
            return false;

        GD.PrintErr(FileAccess.GetOpenError());
        return true;
    }

    private static void SaveStringifiedDataToFile(Dictionary<string, Variant> data, FileAccess file)
    {
        file.StoreString(Json.Stringify(data));
    }

    private static void GetFileAccessAndLoadDataFromFile()
    {
        using var file = GetFileAccess(FileAccess.ModeFlags.Read);

        if (IsThereFileOpenError(file))
            return;

        LoadStringifiedDataFromFile(file);
    }

    private static void LoadStringifiedDataFromFile(FileAccess file)
    {
        var data = GetDataFromFile(file);

        SetData(data);
    }

    private static Dictionary<string, Variant> GetDataFromFile(FileAccess file)
    {
        var data = Json.ParseString(file.GetAsText()).AsGodotDictionary<string, Variant>();

        return data;
    }

    private static void SetData(Dictionary<string, Variant> data)
    {
        Data = data;
    }
}
