using Bombino.game;
using Bombino.ui.scripts;
using Godot;

namespace Bombino.ui.play_menu;

/// <summary>
/// Represents a button that allows the user to select the number of players as 2.
/// </summary>
internal partial class Players2Button : Button, IUiButton
{
    [Export(PropertyHint.File, "*.tscn")] private string _mapsMenuPath;

    /// <summary>
    /// Event handler for the button press event.
    /// </summary>
    public void OnPressed()
    {
        GameManager.NumberOfPlayers = 2;

        GetTree().ChangeSceneToFile(_mapsMenuPath);
    }
}