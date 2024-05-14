using Bombino.game.persistence.storage_layers.key_binds;
using Godot;

namespace Bombino.ui.settings_menu;

/// <summary>
/// A button that saves the settings and returns to the starting screen.
/// </summary>
internal partial class SettingsSaveButton : Button
{
    [Export(PropertyHint.File, "*.tscn")]
    private string _startingScreenPath;

    /// <summary>
    /// Event handler for the button press event.
    /// </summary>
    private void OnPressed()
    {
        var settingsDataAccessLayer = new SettingsDataAccessLayer();
        var settingsKeyBinds = new SettingsKeyBinds(settingsDataAccessLayer);

        settingsKeyBinds.SaveKeyBinds();

        GetTree().ChangeSceneToFile(_startingScreenPath);
    }
}
