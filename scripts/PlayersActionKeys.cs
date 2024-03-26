namespace Bombino.scripts;

using Godot.Collections;
using ui;

/// <summary>
/// Represents the keys for the players' actions.
/// </summary>
internal static class PlayersActionKeys
{
    public static readonly Array<string> Player2 = ActionKeysContainer.ActionItems[5..10];
    public static readonly Array<string> Player1 = ActionKeysContainer.ActionItems[..5];
    public static readonly Array<string> Player3 = ActionKeysContainer.ActionItems[10..];
}
