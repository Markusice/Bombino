namespace Bombino.scripts.ui;

using Godot;

/// <summary>
/// Represents a button used for playing the game.
/// </summary>
internal partial class PlayButton : Button, IUiButton
{
    /// <summary>
    /// Called when the play button is pressed.
    /// </summary>
    public void OnPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/ui/play_menu.tscn");
    }
}
