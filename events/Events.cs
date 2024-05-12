using Godot;

namespace Bombino.events;

/// <summary>
/// Event bus for distant nodes to communicate using signals.
/// This is intended for cases where connecting the nodes directly creates more coupling
/// or increases code complexity substantially.
/// </summary>
internal sealed partial class Events : Node
{
    #region Signals

    /// <summary>
    /// Emitted when a player dies.
    /// </summary>
    /// <param name="playerColor">to identify player's player_name and bomb_status container</param>
    [Signal]
    public delegate void PlayerDiedEventHandler(string playerColor);

    /// <summary>
    /// Emitted when the number of available bombs for a player is incremented.
    /// </summary>
    /// <param name="playerColor">to identify player's player_name and bomb_status container</param>
    /// <param name="numberOfAvailableBombs">the number of available bombs</param>
    [Signal]
    public delegate void PlayerBombNumberIncrementedEventHandler(string playerColor, int numberOfAvailableBombs);

    /// <summary>
    /// Emitted when the number of available bombs for a player is decreased.
    /// </summary>
    /// <param name="playerColor">to identify player's player_name and bomb_status container</param>
    /// <param name="numberOfAvailableBombs">the number of available bombs</param>
    [Signal]
    public delegate void PlayerBombNumberDecreasedEventHandler(string playerColor, int numberOfAvailableBombs);

    #endregion

    /// <summary>
    /// Singleton instance of the event bus.
    /// </summary>
    public static Events Instance { get; private set; }

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _EnterTree() => Instance = this;
}