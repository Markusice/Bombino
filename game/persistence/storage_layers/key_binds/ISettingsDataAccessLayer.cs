namespace Bombino.game.persistence.storage_layers.key_binds;

internal interface ISettingsDataAccessLayer<in T>
{
    bool SaveData(T data);

    bool LoadData(T data);
}