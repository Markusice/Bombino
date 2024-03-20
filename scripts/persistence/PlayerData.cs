namespace Bombino.scripts.persistence;

using Godot;
using Godot.Collections;

internal partial class PlayerData : Resource
{
    [Export]
    public Vector3 Position { get; set; }

    [Export]
    public PlayerColor Color { get; set; }

    [Export]
    public int BombRange { get; set; } = 2;

    [Export]
    public int NumberOfAvailableBombs { get; set; } = 1;

    [Export]
    public int MaxNumberOfAvailableBombs { get; set; } = 1;

    [Export]
    public Array<string> ActionKeys { get; set; }

    public PlayerData()
        : this(0, null) { }

    public PlayerData(PlayerColor color, Array<string> actionKeys)
    {
        Color = color;
        ActionKeys = actionKeys;
    }
}
