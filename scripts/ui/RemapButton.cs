using Godot;

public partial class RemapButton : Button
{
    public InputEvent ActionKey { get; private set; }

    public override void _Ready()
    {
        ThemeTypeVariation = "RemapButton";
    }

    public void SetActionKey(string actionItem)
    {
        ActionKey = InputMap.ActionGetEvents(actionItem)[0];
        Text = ActionKey.AsText();
    }
}