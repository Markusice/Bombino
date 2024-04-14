using Bombino.player;
using Godot;

namespace Bombino.events;

internal sealed partial class Events : Node
{
    #region Signals

    [Signal]
    public delegate void PlayerDiedEventHandler(string playerColor);

    [Signal]
    public delegate void PlayerBombNumberIncrementedEventHandler(string playerColor, int numberOfAvailableBombs);

    [Signal]
    public delegate void PlayerBombNumberDecreasedEventHandler(string playerColor, int numberOfAvailableBombs);

    #endregion

    public static Events Instance { get; private set; }

    public override void _EnterTree() => Instance = this;
}