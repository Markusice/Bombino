namespace Bombino.scripts.ui;

using Godot;

/// <summary>
/// Represents a button that opens the settings menu when pressed.
/// </summary>
internal partial class SettingsButton : Button, IUiButton
{
    /// <summary>
    /// Called when the settings button is pressed.
    /// </summary>
    public void OnPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/ui/settings_menu.tscn");
    }
}
