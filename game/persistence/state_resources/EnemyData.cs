using Godot;

namespace Bombino.game.persistence.state_resources;

/// <summary>
/// Represents the data for an enemy.
/// </summary>
internal partial class EnemyData : Resource
{
    /// <summary>
    /// Gets or sets the position of the enemy.
    /// </summary>
    /// <returns>The position of the enemy.</returns>
    [Export]
    public Vector3 Position { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the enemy is dead.
    /// </summary>
    /// <returns><c>true</c> if the enemy is dead; otherwise, <c>false</c>.</returns>
    public bool IsDead { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the enemy can kill the player. Used
    /// for preventing the enemy from killing the player when a round initializes.
    /// </summary>
    /// <returns><c>true</c> if the enemy can kill the player; otherwise, <c>false</c>.</returns>
    public bool CanKillPlayer { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnemyData"/> class with the specified position.
    /// </summary>
    /// <param name="position">The position of the enemy.</param>
    /// <returns>A new instance of the <see cref="EnemyData"/> class.</returns>
    public EnemyData(Vector3 position)
    {
        Position = position;
        IsDead = false;
        CanKillPlayer = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnemyData"/> class with default values.
    /// </summary>
    /// <returns>A new instance of the <see cref="EnemyData"/> class.</returns>
    public EnemyData()
        : this(Vector3.Zero) { }
}
