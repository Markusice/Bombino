namespace Bombino.scripts.persistence.keybinds;

using System;
using factories;
using Godot;
using Godot.Collections;

internal class SettingsDataAccessLayer : IDataAccessLayer
{
    public void SaveData()
    {
        var config = new ConfigFile();

        SavePlayersKeyBinds(config);

        config.Save(SettingsKeyBinds.KeyBindsPath);
    }

    public bool LoadData()
    {
        var config = new ConfigFile();
        var error = config.Load(SettingsKeyBinds.KeyBindsPath);

        if (error != Error.Ok)
            return false;

        LoadPlayersActionKeysAndSetKeyBinds(config);

        return true;
    }

    private static void SavePlayersKeyBinds(ConfigFile config)
    {
        foreach (var playerData in SettingsKeyBinds.PlayersData)
        {
            var playerActionKeys = playerData.ActionKeys;

            SavePlayerKeyBinds(config, playerActionKeys, playerData);
        }
    }

    private static void SavePlayerKeyBinds(ConfigFile config, Array<string> playerActionKeys, PlayerData playerData)
    {
        foreach (var actionKey in playerActionKeys)
            config.SetValue(playerData.Color.ToString(), actionKey,
                InputMap.ActionGetEvents(actionKey)[0].AsText());
    }

    private static void LoadPlayersActionKeysAndSetKeyBinds(ConfigFile config)
    {
        foreach (var playerColor in config.GetSections())
        {
            var playerColorEnum = Enum.Parse<PlayerColor>(playerColor);
            var playerActionKeys = PlayerActionKeysFactory.GetInstance(playerColorEnum).CreateActionKeys();

            LoadPlayerActionKeys(config, playerColor, playerActionKeys);
        }
    }

    private static void LoadPlayerActionKeys(ConfigFile config, string playerColor,
        Array<string> playerActionKeys)
    {
        foreach (var playerActionKey in playerActionKeys)
        {
            var playerKeyBind = config.GetValue(playerColor, playerActionKey).AsString();

            InputMap.ActionEraseEvents(playerActionKey);

            InputMap.ActionAddEvent(playerActionKey,
                new InputEventKey { Keycode = OS.FindKeycodeFromString(playerKeyBind) });
        }
    }
}
