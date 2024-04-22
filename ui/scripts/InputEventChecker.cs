using Godot;

namespace Bombino.ui.scripts;

/// <summary>
/// Provides methods for checking input events.
/// </summary>
internal abstract class InputEventChecker
{
    /// <summary>
    /// Checks if the Escape key is pressed in the given input event.
    /// </summary>
    /// <param name="event">The input event to check.</param>
    /// <returns>True if the Escape key is pressed, false otherwise.</returns>
    public static bool IsEscapeKeyPressed(InputEvent @event)
    {
        return @event is InputEventKey { Pressed: true, Keycode: Key.Escape };
    }

    /// <summary>
    /// Checks if the Enter key is pressed in the given input event.
    /// </summary>
    /// <param name="event">The input event to check.</param>
    /// <returns>True if the Enter key is pressed, false otherwise.</returns>
    public static bool IsEnterKeyPressed(InputEvent @event)
    {
        return @event is InputEventKey { Pressed: true, Keycode: Key.Enter };
    }
}