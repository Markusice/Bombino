namespace Bombino.scripts.ui;

using Godot;

/// <summary>
/// Represents a custom button used for remapping actions.
/// </summary>
internal partial class RemapButton : Button
{
    private string Action { get; set; }

    public override void _Ready()
    {
        ToggleMode = true;
        ThemeTypeVariation = "RemapButton";

        SetProcessUnhandledInput(false);
    }

    /// <summary>
    /// Sets the action key for the button.
    /// </summary>
    /// <param name="actionItem">The action key to set.</param>
    public void SetActionKey(string actionItem)
    {
        Action = actionItem;

        SetText();
    }

    /// <summary>
    /// Sets the text of the button based on the first event associated with the specified action in the input map.
    /// </summary>
    private void SetText()
    {
        Text = InputMap.ActionGetEvents(Action)[0].AsText();
    }

    /// <summary>
    /// Called when the button is toggled.
    /// </summary>
    /// <param name="toggledOn">Whether the button is toggled on or off.</param>
    public override void _Toggled(bool toggledOn)
    {
        SetProcessUnhandledInput(toggledOn);

        if (ButtonPressed)
        {
            Text = "... Press any key ...";
            ReleaseFocus();
        }
        else
        {
            SetText();
            GrabFocus();
        }
    }

    /// <summary>
    /// Handles unhandled input events for the button.
    /// </summary>
    /// <param name="event">The input event to handle.</param>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsPressed())
            return;

        if (@event is not InputEventKey)
            return;

        InputMap.ActionEraseEvents(Action);
        InputMap.ActionAddEvent(Action, @event);

        ButtonPressed = false;
    }
}
