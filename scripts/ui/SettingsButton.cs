namespace Bombino.scripts.ui;

using Godot;

internal partial class SettingsButton : Button, IUiButton
{
    #region Exports

    [Export]
    private PackedScene _settingsMenuScene;

    #endregion

    public void OnPressed() => GetTree().ChangeSceneToPacked(_settingsMenuScene);
}
