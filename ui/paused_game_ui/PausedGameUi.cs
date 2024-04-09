using Bombino.game;
using Bombino.ui.scripts;
using Godot;

namespace Bombino.ui.paused_game_ui;

/// <summary>
/// Represents the UI layer for the paused game screen.
/// </summary>
internal partial class PausedGameUi : CanvasLayer
{
    private bool _isResumeStarted;

    /// <summary>
    /// Handles input events for the PausedGame class.
    /// </summary>
    /// <param name="event">The input event to handle.</param>
    public override void _Input(InputEvent @event)
    {
        if (!InputEventChecker.IsEscapeKeyPressed(@event) || _isResumeStarted)
            return;

        GameManager.WorldEnvironment.EmitSignal(GameManager.SignalName.ResumeGame);

        _isResumeStarted = true;
    }
}