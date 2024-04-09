using Bombino.ui.scripts;
using Godot;

namespace Bombino.ui.maps_menu;

/// <summary>
/// Represents a button used for selecting a map in the UI.
/// </summary>
internal partial class MapButton : Button, IUiButton
{
    [Export(PropertyHint.File, "*.tscn")] private string _roundsMenuScenePath;

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
    private void SetSelectedMapAndChangeScene()
    {
    }
}