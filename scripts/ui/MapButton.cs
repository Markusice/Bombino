namespace Bombino.scripts.ui;

using Godot;

internal partial class MapButton : Button, IUiButton
{
    #region Exports

    [Export]
    private PackedScene _roundsMenuScene;

    #endregion

    public void OnPressed()
    {
        SetSelectedMapAndChangeScene();

        GetTree().ChangeSceneToPacked(_roundsMenuScene);
    }

    private void SetSelectedMapAndChangeScene() { }
}
