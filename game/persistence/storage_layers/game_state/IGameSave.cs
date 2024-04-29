using FileAccess = Godot.FileAccess;

namespace Bombino.game.persistence.storage_layers.game_state;

internal interface IGameSave<T>
{
    (bool, FileAccess) LoadFile(string path, FileAccess.ModeFlags modeFlags);

    bool SaveData(T data);

    T GetDataFromFile(FileAccess file);
}