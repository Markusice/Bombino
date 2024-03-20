namespace Bombino.scripts.ui;

using Godot;

internal partial class MapButton : Button, IUiButton
{
    #region Exports

    private string _roundsMenuScenePath = "res://scenes/ui/rounds_menu.tscn";

    #endregion

    public void OnPressed()
    {
        SetSelectedMapAndChangeScene();

        GetTree().ChangeSceneToFile(_roundsMenuScenePath);
    }

    private void SetSelectedMapAndChangeScene() { }
}
