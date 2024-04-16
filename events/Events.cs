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
    /// <param name="playerColor">to identify player's player_name and bomb_status container</param>
    /// </summary>
    [Signal]
    public delegate void PlayerDiedEventHandler(string playerColor);

    /// <summary>
    /// Event that is triggered when the number of available bombs for a player is incremented.
    /// <param name="playerColor">to identify player's player_name and bomb_status container</param>
    /// <paramref name="numberOfAvailableBombs"/>
    /// </summary>
    [Signal]
    public delegate void PlayerBombNumberIncrementedEventHandler(string playerColor, int numberOfAvailableBombs);

    /// <summary>
    /// Event that is triggered when the number of available bombs for a player is decreased.
    /// <param name="playerColor">to identify player's player_name and bomb_status container</param>
    /// </summary>
    [Signal]
    public delegate void PlayerBombNumberDecreasedEventHandler(string playerColor, int numberOfAvailableBombs);

    #endregion

    public static Events Instance { get; private set; }

    public override void _EnterTree() => Instance = this;
}