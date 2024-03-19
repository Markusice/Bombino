namespace Bombino.scripts.ui;

using Godot;

internal partial class MapButton : Button, IUiButton
{
    [Export]
    private PackedScene _roundsMenuScene;

    public void OnPressed()
    {
        SetSelectedMapAndChangeScene();

        GetTree().ChangeSceneToPacked(_roundsMenuScene);
    }

    private void SetSelectedMapAndChangeScene()
    {
    }
}