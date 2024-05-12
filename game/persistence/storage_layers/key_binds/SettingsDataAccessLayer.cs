using Bombino.player;
using Godot;

namespace Bombino.game.persistence.storage_layers.key_binds;

/// <summary>
/// Represents a data access layer for settings.
/// </summary>
internal class SettingsDataAccessLayer : ISettingsDataAccessLayer<Dictionary<string, PlayerColor>>
{
    #region Fields

    private const string KeyBindsPath = "user://keybinds.cfg";

    private ConfigFile Config { get; } = new();

    #endregion

    #region InterfaceMethods

    /// <summary>
    /// Saves the key binds data to the config file.
    /// </summary>
    /// <param name="dataForConfigFile">The key binds data to save.</param>
    /// <returns>True if the data was successfully saved; otherwise, false.</returns>
    public bool SaveData(Dictionary<string, PlayerColor> dataForConfigFile)
    {
        SavePlayersKeyBinds(dataForConfigFile);

        var error = Config.Save(KeyBindsPath);
        if (error != Error.Ok)
        {
            GD.PushError(
                $"An error occurred when trying to load the config file ({KeyBindsPath}): {error}"
            );

            return false;
        }

        return true;
    }

    /// <summary>
    /// Loads the key binds data from the config file.
    /// </summary>
    /// <param name="dataForConfigFile">The key binds data to load.</param>
    /// <returns>True if the data was successfully loaded; otherwise, false.</returns>
    public bool LoadData(Dictionary<string, PlayerColor> dataForConfigFile)
    {
        var error = Config.Load(KeyBindsPath);
        if (error == Error.FileNotFound)
            return SaveData(dataForConfigFile);

        if (error != Error.Ok)
        {
            GD.PushError(
                $"An error occurred when trying to load the config file ({KeyBindsPath}): {error}"
            );

            return false;
        }

        LoadPlayersKeyBinds(dataForConfigFile);

        return true;
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsDataAccessLayer"/> class.
    /// </summary>
    /// <param name="data">The key binds data to load.</param>
    private void SavePlayersKeyBinds(Dictionary<string, PlayerColor> data)
    {
        foreach (var (inputAction, playerColor) in data)
        {
            Config.SetValue(
                playerColor.ToString(),
                inputAction,
                GetKeyBindForInputAction(inputAction)
            );
        }
    }

    /// <summary>
    /// Gets the key bind for the input action.
    /// </summary>
    /// <param name="inputAction">The input action to get the key bind for.</param>
    private static string GetKeyBindForInputAction(string inputAction)
    {
        return InputMap.ActionGetEvents(inputAction)[0].AsText();
    }

    /// <summary>
    /// Loads the players' key binds from the config file.
    /// </summary>
    /// <param name="dataForConfigFile">The key binds data to load.</param>
    private void LoadPlayersKeyBinds(Dictionary<string, PlayerColor> dataForConfigFile)
    {
        foreach (var (inputAction, playerColor) in dataForConfigFile)
        {
            var playerKeyBind = Config.GetValue(playerColor.ToString(), inputAction).AsString();

            InputMap.ActionEraseEvents(inputAction);

            InputMap.ActionAddEvent(
                inputAction,
                new InputEventKey { Keycode = OS.FindKeycodeFromString(playerKeyBind) }
            );
        }
    }
}
