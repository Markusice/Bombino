using Bombino.player;
using Godot;

namespace Bombino.game.persistence.state_resources;

/// <summary>
/// Represents the data of a player in the game.
/// </summary>
internal partial class PlayerData : Resource
{
    /// <summary>
    /// Gets or sets the position of the player.
    /// </summary>
    [Export]
    public Vector3 Position { get; set; }

    /// <summary>
    /// Gets or sets the color of the player.
    /// </summary>
    /// <returns>The color of the player.</returns>
    [Export]
    public PlayerColor Color { get; set; }

    /// <summary>
    /// Gets or sets the range of the player's bombs.
    /// </summary>
    /// <returns> The range of the player's bombs.</returns> 
    [Export]
    public int BombRange { get; set; } = 2;

    /// <summary>
    /// Gets or sets the number of placed bombs for the player.
    /// </summary>
    /// <returns>The number of placed bombs for the player.</returns>
    [Export]
    public int NumberOfPlacedBombs { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum number of available bombs for the player.
    /// </summary>
    /// <returns>The maximum number of available bombs for the player.</returns>
    [Export]
    public int MaxNumberOfAvailableBombs { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating whether the player is dead.
    /// </summary>
    /// <returns>True if the player is dead; otherwise, false.</returns>
    [Export]
    public bool IsDead { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of wins for the player.
    /// </summary>
    /// <returns>The number of wins for the player.</returns>
    [Export]
    public ushort Wins { get; set; } = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerData"/> class with default values.
    /// </summary>
    public PlayerData()
        : this(Vector3.Zero, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerData"/> class with the specified color and action keys.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color">The color of the player.</param>
    public PlayerData(Vector3 position, PlayerColor color)
    {
        Position = position;
        Color = color;
    }

    /// <summary>
    /// Resets the player data to the default values for a new round.
    /// </summary>
    public static void ResetToNewRound(ref PlayerData playerData)
    {
        playerData.IsDead = false;
        playerData.MaxNumberOfAvailableBombs = 1;
        playerData.NumberOfPlacedBombs = 0;
        playerData.BombRange = 2;
    }
}