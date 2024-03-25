namespace Bombino.scripts.persistence;

using Godot;
using Godot.Collections;

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
    [Export]
    public PlayerColor Color { get; set; }

    /// <summary>
    /// Gets or sets the range of the player's bombs.
    /// </summary>
    [Export]
    public int BombRange { get; set; } = 2;

    /// <summary>
    /// Gets or sets the number of available bombs for the player.
    /// </summary>
    [Export]
    public int NumberOfAvailableBombs { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum number of available bombs for the player.
    /// </summary>
    [Export]
    public int MaxNumberOfAvailableBombs { get; set; } = 1;

    /// <summary>
    /// Gets or sets the action keys for the player.
    /// </summary>
    [Export]
    public Array<string> ActionKeys { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerData"/> class with default values.
    /// </summary>
    public PlayerData()
        : this(0, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerData"/> class with the specified color and action keys.
    /// </summary>
    /// <param name="color">The color of the player.</param>
    /// <param name="actionKeys">The action keys for the player.</param>
    public PlayerData(PlayerColor color, Array<string> actionKeys)
    {
        Color = color;
        ActionKeys = actionKeys;
    }
}
