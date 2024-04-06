namespace Bombino.scripts.ui.settings_menu;

using Godot;

internal partial class SettingsCancelButton : Button
{
    private void OnPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/ui/starting_screen.tscn");
    }
}
