namespace Bombino.scripts.ui;

using Godot;

/// <summary>
/// Represents a button used for playing the game.
/// </summary>
internal partial class PlayButton : Button, IUiButton
{
    #region Exports

    [Export]
    private PackedScene _playMenuScene;

    #endregion

    /// <summary>
    /// Called when the play button is pressed.
    /// </summary>
    public void OnPressed() => GetTree().ChangeSceneToPacked(_playMenuScene);
}
