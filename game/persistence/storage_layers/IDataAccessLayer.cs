using Bombino.game.persistence.storage_layers.key_binds;

namespace Bombino.game.persistence.storage_layers;

internal interface IDataAccessLayer
{
    bool SaveData(SettingsKeyBinds dataHolder);

    bool LoadData(SettingsKeyBinds dataHolder);
}