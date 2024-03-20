namespace Bombino.scripts.ui;

using Godot;

internal class InputEventChecker
{
    public static bool IsEscapeKeyPressed(InputEvent @event)
    {
        return @event is InputEventKey { Pressed: true, Keycode: Key.Escape };
    }
}
