namespace Bombino.game.persistence.storage_layers.key_binds;

/// <summary>
/// Represents a data access layer for settings.
/// </summary>
internal interface ISettingsDataAccessLayer<in T>
{
    bool SaveData(T data);

    bool LoadData(T data);
}