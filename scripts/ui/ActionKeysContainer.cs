namespace Bombino.scripts.ui;

using Godot;
using Godot.Collections;
using persistence;

/// <summary>
/// Represents a container for action keys in the user interface.
/// </summary>
internal partial class ActionKeysContainer : GridContainer
{
    public static Array<string> ActionItems { get; } =
        new()
        {
            "move_forward",
            "move_back",
            "move_left",
            "move_right",
            "place_bomb",
            "move_forward_p2",
            "move_back_p2",
            "move_left_p2",
            "move_right_p2",
            "place_bomb_p2",
            "move_forward_p3",
            "move_back_p3",
            "move_left_p3",
            "move_right_p3",
            "place_bomb_p3",
        };

    public override void _Ready()
    {
        ActionKeysMapper.CreateActionKeys(this);
    }
}
