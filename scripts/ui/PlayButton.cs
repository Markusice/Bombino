namespace Bombino.scripts.ui;

using Godot;

internal partial class PlayButton : Button, IUiButton
{
    #region Exports

    [Export]
    private PackedScene _playMenuScene;

    #endregion

    public void OnPressed() => GetTree().ChangeSceneToPacked(_playMenuScene);
}
