namespace Bombino.scripts.persistence.keybinds;

using Godot.Collections;

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
