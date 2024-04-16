using System;
using Bombino.player;
using Godot;

namespace Bombino.game.persistence.storage_layers.key_binds;

internal class SettingsDataAccessLayer : IDataAccessLayer
{
    private ConfigFile Config { get; } = new();

    public bool SaveData(SettingsKeyBinds dataHolder)
    {
        SavePlayersKeyBinds(dataHolder);

        var error = Config.Save(SettingsKeyBinds.KeyBindsPath);

        return error == Error.Ok;
    }

    public bool LoadData(SettingsKeyBinds dataHolder)
    {
        var error = Config.Load(SettingsKeyBinds.KeyBindsPath);

        if (error == Error.FileNotFound)
            return SaveData(dataHolder);

        if (error != Error.Ok)
            return false;

        LoadPlayersActionKeysAndSetKeyBinds(dataHolder);

        return true;
    }

    private void SavePlayersKeyBinds(SettingsKeyBinds dataHolder)
    {
        SavePlayerKeyBinds(dataHolder);
    }

    private void SavePlayerKeyBinds(SettingsKeyBinds dataHolder)
    {
        foreach (var inputActionForPlayerColor in dataHolder.InputActionsForPlayerColors)
        {
            Config.SetValue(inputActionForPlayerColor.Value.ToString(), inputActionForPlayerColor.Key,
                InputMap.ActionGetEvents(inputActionForPlayerColor.Key)[0].AsText());
        }
    }

    private void LoadPlayersActionKeysAndSetKeyBinds(SettingsKeyBinds dataHolder)
    {
        foreach (var inputActionForPlayerColor in dataHolder.InputActionsForPlayerColors)
        {
            var playerKeyBind = Config
                .GetValue(inputActionForPlayerColor.Value.ToString(), inputActionForPlayerColor.Key).AsString();

            InputMap.ActionEraseEvents(inputActionForPlayerColor.Key);

            InputMap.ActionAddEvent(inputActionForPlayerColor.Key,
                new InputEventKey { Keycode = OS.FindKeycodeFromString(playerKeyBind) });
        }
    }
}