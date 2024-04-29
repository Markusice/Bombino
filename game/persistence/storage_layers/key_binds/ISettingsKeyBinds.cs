namespace Bombino.game.persistence.storage_layers.key_binds;

internal interface ISettingsKeyBinds
{
    bool SaveKeyBinds();

    bool LoadKeyBinds();
}