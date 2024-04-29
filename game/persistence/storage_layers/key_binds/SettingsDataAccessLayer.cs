using Bombino.player;
using Godot;

namespace Bombino.game.persistence.storage_layers.key_binds;

internal class SettingsDataAccessLayer : ISettingsDataAccessLayer<Dictionary<string, PlayerColor>>
{
    #region Fields

    private const string KeyBindsPath = "user://keybinds.cfg";

    private ConfigFile Config { get; } = new();

    #endregion

    #region InterfaceMethods

    public bool SaveData(Dictionary<string, PlayerColor> dataForConfigFile)
    {
        SavePlayersKeyBinds(dataForConfigFile);

        var error = Config.Save(KeyBindsPath);

        if (error == Error.Ok) return true;

        GD.PushError(error);

        return false;
    }

    public bool LoadData(Dictionary<string, PlayerColor> dataForConfigFile)
    {
        var error = Config.Load(KeyBindsPath);

        if (error == Error.FileNotFound)
            return SaveData(dataForConfigFile);

        if (error != Error.Ok)
            return false;

        LoadPlayersKeyBinds(dataForConfigFile);

        return true;
    }

    #endregion

    private void SavePlayersKeyBinds(Dictionary<string, PlayerColor> data)
    {
        foreach (var (inputAction, playerColor) in data)
        {
            Config.SetValue(playerColor.ToString(), inputAction,
                GetKeyBindForInputAction(inputAction));
        }
    }

    private static string GetKeyBindForInputAction(string inputAction)
    {
        return InputMap.ActionGetEvents(inputAction)[0].AsText();
    }

    private void LoadPlayersKeyBinds(Dictionary<string, PlayerColor> dataForConfigFile)
    {
        foreach (var (inputAction, playerColor) in dataForConfigFile)
        {
            var playerKeyBind = Config.GetValue(playerColor.ToString(), inputAction).AsString();

            InputMap.ActionEraseEvents(inputAction);

            InputMap.ActionAddEvent(inputAction,
                new InputEventKey { Keycode = OS.FindKeycodeFromString(playerKeyBind) });
        }
    }
}