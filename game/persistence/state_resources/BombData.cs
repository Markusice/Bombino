using Godot;

namespace Bombino.game.persistence.state_resources;

internal partial class BombData : Resource
{
    /// <summary>
    /// The position of the bomb.
    /// </summary>
    [Export]
    public Vector3 Position { get; set; }

    /// <summary>
    /// The range of the bomb.
    /// </summary>
    [Export]
    public float Range { get; set; }

    /// <summary>
    /// The time left for the bomb to explode.
    /// </summary>
    [Export]
    public float TimeLeft { get; set; }

    /// <summary>
    /// The player that placed the bomb.
    /// </summary>
    [Export]
    public PlayerData PlayerData { get; set; }

    public BombData(Vector3 position, float timeLeft, float range)
    {
        Position = position;
        Range = range;
        TimeLeft = timeLeft;
    }

    public BombData()
        : this(Vector3.Zero, 0, 0) { }
}
