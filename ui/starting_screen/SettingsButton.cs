using Bombino.ui.scripts;
using Godot;

namespace Bombino.ui.starting_screen;

/// <summary>
/// Represents a button that opens the settings menu when pressed.
/// </summary>
internal partial class SettingsButton : Button, IUiButton
{
    [Export(PropertyHint.File, "*.tscn")] private string _settingsMenuPath;

    /// <summary>
    /// Called when the settings button is pressed.
    /// </summary>
    public void OnPressed()
    {
        GetTree().ChangeSceneToFile(_settingsMenuPath);
    }
}