using Godot;

public partial class Players3Button : Button, IUIButton
{
    [Export]
    private PackedScene _mapsMenuScene;

    public void OnPressed()
    {
        GameManager.NumberOfPlayers = 3;
        
        GetTree().ChangeSceneToPacked(_mapsMenuScene);
    }
}
