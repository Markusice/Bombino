using Bombino.game;
using Bombino.ui.scripts;
using Godot;

namespace Bombino.ui.play_menu;

/// <summary>
/// Represents a button for selecting 3 players in the UI.
/// </summary>
internal partial class Players3Button : Button, IUiButton
{
    [Export(PropertyHint.File, "*.tscn")] private string _mapsMenuPath;

    /// <summary>
    /// Sets the number of players to 3 and changes the scene to the maps menu.
    /// </summary>
    public void OnPressed()
    {
        GameManager.NumberOfPlayers = 3;

        GetTree().ChangeSceneToFile(_mapsMenuPath);
    }
}