using Bombino.file_system_helpers.directory;
using Bombino.file_system_helpers.file;
using Godot;
using FileAccess = Godot.FileAccess;

namespace Bombino.game.persistence.storage_layers.game_state;

/// <summary>
/// Represents a game save file.
/// </summary>
internal class GameSaver : IGameSaver<Godot.Collections.Dictionary<string, Variant>>
{
    #region Fields

    private readonly IDirAccessManager _dirAccessManager = new DirAccessManager();
    private readonly IFileAccessManager _fileAccessManager = new FileAccessManager();

    private readonly string _fileName = $"save-{Time.GetDatetimeStringFromSystem(utc: true)}";
    private const string SaveDirectory = "saves";
    private readonly string _gameSavePath;

    #endregion

    public GameSaver()
    {
        _gameSavePath = $"user://{SaveDirectory}/{_fileName}.json";
    }

    #region InterfaceMethods

    /// <summary>
    /// Writes the game save data to the file.
    /// </summary>
    /// <param name="data">The game save data to write.</param>
    public bool SaveData(Godot.Collections.Dictionary<string, Variant> data)
    {
        var dirError = _dirAccessManager.MakeDirectory("user://", SaveDirectory);
        if (dirError is not (Error.Ok or Error.AlreadyExists))
        {
            return false;
        }

        var loadFile = _fileAccessManager.LoadFile(_gameSavePath, FileAccess.ModeFlags.Write);
        var fileError = loadFile.Item1;
        if (fileError != Error.Ok)
        {
            return false;
        }

        using var file = loadFile.Item2;

        _fileAccessManager.StoreJSONData(file, data);

        return true;
    }

    #endregion
}
