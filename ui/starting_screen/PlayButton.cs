using Bombino.ui.scripts;
using Godot;

namespace Bombino.ui.starting_screen;

/// <summary>
/// Represents a button used for playing the game.
/// </summary>
internal partial class PlayButton : Button, IUiButton
{
    [Export(PropertyHint.File, "*.tscn")] private string _playMenuPath;

    /// <summary>
    /// Called when the play button is pressed.
    /// </summary>
    public void OnPressed()
    {
        GetTree().ChangeSceneToFile(_playMenuPath);
    }
}