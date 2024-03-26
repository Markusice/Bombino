namespace Bombino.scripts.ui;

using Godot;

/// <summary>
/// Represents a button that allows the user to select the number of players as 2.
/// </summary>
internal partial class Players2Button : Button, IUiButton
{
    #region Exports

    [Export]
    private PackedScene _mapsMenuScene;

    #endregion

    /// <summary>
    /// Event handler for the button press event.
    /// </summary>
    public void OnPressed() => GetTree().ChangeSceneToPacked(_mapsMenuScene);
}
