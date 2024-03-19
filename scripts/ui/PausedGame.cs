namespace Bombino.scripts.ui;

using Godot;

internal partial class PausedGame : CanvasLayer
{
    public override void _Input(InputEvent @event)
    {
        if (!InputEventChecker.IsEscapeKeyPressed(@event))
            return;

        GameManager.WorldEnvironment.EmitSignal(GameManager.SignalName.ResumeGame);
    }
}
