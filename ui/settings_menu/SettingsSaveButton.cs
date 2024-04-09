using Bombino.game.persistence.storage_layers.key_binds;
using Godot;

namespace Bombino.ui.settings_menu;

internal partial class SettingsSaveButton : Button
{
    [Export(PropertyHint.File, "*.tscn")] private string _startingScreenPath;

    private void OnPressed()
    {
        var settingsDataAccessLayer = new SettingsDataAccessLayer();
        var settingsKeyBinds = new SettingsKeyBinds(settingsDataAccessLayer);

        settingsKeyBinds.SaveKeyBinds();

        GetTree().ChangeSceneToFile(_startingScreenPath);
    }
}