using FileAccess = Godot.FileAccess;

namespace Bombino.game.persistence.storage_layers.game_state;

internal interface IGameSaver<T>
{
    bool SaveData(T data);

    T GetDataFromFile(FileAccess file);
}