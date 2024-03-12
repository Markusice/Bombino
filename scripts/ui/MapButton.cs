using System;
using Godot;

public partial class MapButton : Button, IUIButton
{
    [Export]
    private PackedScene _roundsMenuScene;

    private string _mapName;

    public void OnPressed()
    {
        GameManager.SelectedMap = _mapName;

        GameManager.SelectedMap = Name.ToString() switch
        {
            { } name when name.StartsWith("1") => "Map1",
            { } name when name.StartsWith("2") => "Map2",
            _ => "Map3"
        };

        GetTree().ChangeSceneToPacked(_roundsMenuScene);
    }
}