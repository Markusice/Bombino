namespace Bombino.scripts;

using ui;
using Godot.Collections;

/// <summary>
/// Represents the keys for the players' actions.
/// </summary>
internal static class PlayersActionKeys
{
    internal static readonly Array<string> Player2 = ActionKeysContainer.ActionItems[5..10];
    internal static readonly Array<string> Player1 = ActionKeysContainer.ActionItems[..5];
    internal static readonly Array<string> Player3 = ActionKeysContainer.ActionItems[10..];
}