namespace Bombino.scripts.ui;

using Godot;

internal partial class RemapButton : Button
{
    private string Action { get; set; }

    public override void _Ready()
    {
        ToggleMode = true;
        ThemeTypeVariation = "RemapButton";

        SetProcessUnhandledInput(false);
    }

    public void SetActionKey(string actionItem)
    {
        Action = actionItem;

        SetText();
    }

    private void SetText()
    {
        Text = InputMap.ActionGetEvents(Action)[0].AsText();
    }

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
