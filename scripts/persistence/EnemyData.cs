namespace Bombino.scripts.persistence;

using Godot;

internal partial class EnemyData : Resource
{
    [Export]
    public Vector3 Position { get; set; }

    public EnemyData(Vector3 position)
    {
        Position = position;
    }

    public EnemyData()
        : this(Vector3.Zero)
    {
    }
}
