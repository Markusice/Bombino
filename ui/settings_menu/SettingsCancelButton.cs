using Godot;

namespace Bombino.ui.settings_menu;

internal partial class SettingsCancelButton : Button
{
    [Export(PropertyHint.File, "*.tscn")]
    private string _startingScreenPath;

    private void OnPressed()
    {
        GetTree().ChangeSceneToFile(_startingScreenPath);
    }
}
