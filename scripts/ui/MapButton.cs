namespace Bombino.scripts.ui;

using Godot;

/// <summary>
/// Represents a button used for selecting a map in the UI.
/// </summary>
internal partial class MapButton : Button, IUiButton
{
    #region Exports

    private string _roundsMenuScenePath = "res://scenes/ui/rounds_menu.tscn";

    #endregion

    /// <summary>
    /// Called when the map button is pressed.
    /// Sets the selected map and changes the scene to the rounds menu.
    /// </summary>
    public void OnPressed()
    {
        SetSelectedMapAndChangeScene();

        GetTree().ChangeSceneToFile(_roundsMenuScenePath);
    }

    /// <summary>
    /// Sets the selected map and changes the scene.
    /// </summary>
    private void SetSelectedMapAndChangeScene() { }
}
