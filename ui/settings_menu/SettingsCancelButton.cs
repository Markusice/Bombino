using Godot;

namespace Bombino.ui.settings_menu;

/// <summary>
/// A button that cancels the settings and returns to the starting screen.
/// </summary>
internal partial class SettingsCancelButton : Button
{
    [Export(PropertyHint.File, "*.tscn")]
    private string _startingScreenPath;

    /// <summary>
    /// Event handler for the button press event.
    /// </summary>
    private void OnPressed()
    {
        GetTree().ChangeSceneToFile(_startingScreenPath);
    }
}
