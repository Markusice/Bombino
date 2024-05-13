namespace Bombino.game.persistence.storage_layers.game_state;

/// <summary>
/// Represents a game saver.
/// </summary>
internal interface IGameSaver<in T>
{
    bool SaveData(T data);
}
