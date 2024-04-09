using Bombino.game.persistence.state_storage;
using Godot.Collections;

namespace Bombino.game.persistence.storage_layers.key_binds;

internal class SettingsKeyBinds
{
    public const string KeyBindsPath = "user://keybinds.cfg";

    private readonly IDataAccessLayer _dataAccessLayer;

    public static Array<PlayerData> PlayersData => GameManager.PlayersData;

    public SettingsKeyBinds(IDataAccessLayer dataAccessLayer)
    {
        _dataAccessLayer = dataAccessLayer;
    }

    public void SaveKeyBinds()
    {
        _dataAccessLayer.SaveData();
    }

    public bool LoadKeyBinds()
    {
        return _dataAccessLayer.LoadData();
    }
}