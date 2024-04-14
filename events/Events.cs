using Godot;

namespace Bombino.events;

internal sealed partial class Events : Node
{
    #region Signals

    [Signal]
    public delegate void PlayerDiedEventHandler(string playerColor);

    #endregion

    public static Events Instance { get; private set; }

    public override void _EnterTree() => Instance = this;
}