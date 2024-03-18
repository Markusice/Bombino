namespace Bombino.scripts.ui;

using Godot;

internal partial class PlayButton : Button, IUiButton
{
    [Export]
    private PackedScene _playMenuScene;

    public void OnPressed() => GetTree().ChangeSceneToPacked(_playMenuScene);
}