using Godot;

public partial class SettingsButton : Button, IUIButton
{
    [Export]
    private PackedScene _settingsMenuScene;

    public void OnPressed()
    {
        GetTree().ChangeSceneToPacked(_settingsMenuScene);
    }
}