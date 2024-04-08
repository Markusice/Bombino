namespace Bombino.scripts.ui.settings_menu;

using Godot;
using persistence.keybinds;

internal partial class SettingsSaveButton : Button
{
    private void OnPressed()
    {
        var settingsDataAccessLayer = new SettingsDataAccessLayer();
        var settingsKeyBinds = new SettingsKeyBinds(settingsDataAccessLayer);

        settingsKeyBinds.SaveKeyBinds();

        GetTree().ChangeSceneToFile("res://scenes/ui/starting_screen.tscn");
    }
}
