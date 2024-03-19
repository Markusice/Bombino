namespace Bombino.scripts.ui;

using Godot;

internal partial class Players3Button : Button, IUiButton
{
    [Export]
    private PackedScene _mapsMenuScene;

    public void OnPressed()
    {
        GameManager.NumberOfPlayers = 3;

        GetTree().ChangeSceneToPacked(_mapsMenuScene);
    }
}