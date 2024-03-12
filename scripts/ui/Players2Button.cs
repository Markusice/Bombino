using Godot;

public partial class Players2Button : Button, IUIButton
{
    [Export]
    private PackedScene _mapsMenuScene;

    public void OnPressed()
    {
        GetTree().ChangeSceneToPacked(_mapsMenuScene);
    }
}
