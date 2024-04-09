using Godot;

namespace Bombino.game.persistence.state_storage;

internal partial class EnemyData : Resource
{
    [Export] public Vector3 Position { get; set; }

    public EnemyData(Vector3 position)
    {
        Position = position;
    }

    public EnemyData()
        : this(Vector3.Zero)
    {
    }
}