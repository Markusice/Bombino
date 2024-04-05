namespace Bombino.scripts.persistence.keybinds;

using Godot.Collections;

internal class SettingsKeybinds
{
    public const string KeybindsPath = "user://keybinds.cfg";

    private readonly IDataAccessLayer _dataAccessLayer;

    public static Array<PlayerData> PlayersData => GameManager.PlayersData;

    public SettingsKeybinds(IDataAccessLayer dataAccessLayer)
    {
        _dataAccessLayer = dataAccessLayer;
    }

    public void SaveKeybinds()
    {
        _dataAccessLayer.SaveData();
    }
    
    public bool LoadKeybinds()
    {
        return _dataAccessLayer.LoadData();
    }
}
