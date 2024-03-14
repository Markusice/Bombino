using Godot;

public partial class PausedGame : CanvasLayer
{
    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey { Pressed: true, Keycode: Key.Escape }) return;

        GameManager.WorldEnvironment.EmitSignal(GameManager.SignalName.ResumeGame);
    }
}