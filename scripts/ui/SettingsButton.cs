namespace Bombino.scripts.ui;

using Godot;

internal partial class SettingsButton : Button, IUiButton
{
    [Export]
    private PackedScene _settingsMenuScene;

    public void OnPressed() => GetTree().ChangeSceneToPacked(_settingsMenuScene);
}