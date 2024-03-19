namespace Bombino.scripts.ui;

using Godot;

internal partial class Players3Button : Button, IUiButton
{
    #region Exports

    [Export]
    private PackedScene _mapsMenuScene;

    #endregion

    public void OnPressed()
    {
        GameManager.NumberOfPlayers = 3;

        GetTree().ChangeSceneToPacked(_mapsMenuScene);
    }
}
