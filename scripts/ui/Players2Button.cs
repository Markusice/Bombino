namespace Bombino.scripts.ui;

using Godot;

internal partial class Players2Button : Button, IUiButton
{
    #region Exports

    [Export]
    private PackedScene _mapsMenuScene;

    #endregion

    public void OnPressed() => GetTree().ChangeSceneToPacked(_mapsMenuScene);
}
