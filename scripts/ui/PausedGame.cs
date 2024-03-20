namespace Bombino.scripts.ui;

using Godot;

internal partial class PausedGame : CanvasLayer
{
    private bool _isResumeStarted;

    public override void _Input(InputEvent @event)
    {
        if (!InputEventChecker.IsEscapeKeyPressed(@event) || _isResumeStarted)
            return;

        GameManager.WorldEnvironment.EmitSignal(GameManager.SignalName.ResumeGame);

        _isResumeStarted = true;
    }
}
