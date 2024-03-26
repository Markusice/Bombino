namespace Bombino.scripts.ui;

using Godot;

/// <summary>
/// Represents a button for selecting 3 players in the UI.
/// </summary>
internal partial class Players3Button : Button, IUiButton
{
    #region Exports

    [Export]
    private PackedScene _mapsMenuScene;

    #endregion

    /// <summary>
    /// Sets the number of players to 3 and changes the scene to the maps menu.
    /// </summary>
    public void OnPressed()
    {
        GameManager.NumberOfPlayers = 3;

        GetTree().ChangeSceneToPacked(_mapsMenuScene);
    }
}
