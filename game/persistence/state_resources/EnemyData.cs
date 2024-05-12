using Godot;

namespace Bombino.game.persistence.state_storage;

/// <summary>
/// Represents the data for an enemy.
/// </summary>
internal partial class EnemyData : Resource
{
    /// <summary>
    /// Gets or sets the position of the enemy.
    /// </summary>
    /// <returns>The position of the enemy.</returns>
    [Export] public Vector3 Position { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnemyData"/> class with the specified position.
    /// </summary>
    /// <param name="position">The position of the enemy.</param>
    /// <returns>A new instance of the <see cref="EnemyData"/> class.</returns>
    public EnemyData(Vector3 position)
    {
        Position = position;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnemyData"/> class with default values.
    /// </summary>
    /// <returns>A new instance of the <see cref="EnemyData"/> class.</returns>
    public EnemyData()
        : this(Vector3.Zero)
    {
    }
}