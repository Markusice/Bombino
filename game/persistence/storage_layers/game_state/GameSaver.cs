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
    private readonly string _gameSavePath;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GameSaver"/> class.
    /// </summary>
    /// <remarks>
    /// The game save path is set to the user directory with the save directory path and the file name.
    /// </remarks>
    public GameSaver()
    {
        _gameSavePath = $"user://{SaveDirectory.Path}/{_fileName}.json";
    }

    #region InterfaceMethods

    /// <summary>
    /// Writes the game save data to the file.
    /// </summary>
    /// <param name="data">The game save data to write.</param>
    /// <returns>True if the data was successfully saved; otherwise, false.</returns>
    public bool SaveData(Godot.Collections.Dictionary<string, Variant> data)
    {
        var dirError = _dirAccessManager.MakeDirectory("user://", SaveDirectory.Path);
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

        _fileAccessManager.StoreJsonData(file, data);

        return true;
    }

    #endregion
}
