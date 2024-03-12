using Godot;

public partial class PlayButton : Button, IUIButton
{
    [Export]
    private PackedScene _playMenuScene;

    public void OnPressed()
    {
        GetTree().ChangeSceneToPacked(_playMenuScene);
    }
}