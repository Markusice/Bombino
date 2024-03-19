namespace Bombino.scripts.ui;

using Godot;

internal partial class Players2Button : Button, IUiButton
{
    [Export]
    private PackedScene _mapsMenuScene;

    public void OnPressed() => GetTree().ChangeSceneToPacked(_mapsMenuScene);
}
