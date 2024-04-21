using Bombino.game;
using Bombino.ui.scripts;
using Godot;

namespace Bombino.ui.paused_game_ui;

/// <summary>
/// Represents the UI layer for the paused game screen.
/// </summary>
internal partial class PausedGameUi : CanvasLayer
{
    #region Exports

    [Export(PropertyHint.File, "*.tscn")] private string _startingScreenScenePath;

    #endregion

    private AnimationPlayer _animationPlayer;

    private bool _isResumed;

    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("BlurAnimation");

        PlayBlurAnimation();
    }

    #region MethodsForSignals

    private void OnResumeButtonPressed()
    {
        _ = StartCountDownAndResume();
    }

    private void OnSaveAndExitButtonPressed()
    {
        // TODO
        // SaveGameAndGoToStartingScreen();
    }

    #endregion

    /// <summary>
    /// Starts the countdown and after that emits ResumeGame signal to GameManager.
    /// </summary>
    private async Task StartCountDownAndResume()
    {
        _isResumed = true;

        await StartCountDown();
        Visible = false;

        GameManager.WorldEnvironment.EmitSignal(GameManager.SignalName.ResumeGame);

        QueueFree();
    }

    /// <summary>
    /// Starts the countdown from 3 to 1.
    /// </summary>
    private async Task StartCountDown()
    {
        GetNode<PanelContainer>("ButtonsContainer").QueueFree();

        var countDownContainer = GetNode<PanelContainer>("CountDownContainer");
        countDownContainer.Visible = true;

        PlayRemoveBlurAnimation();

        var countDownLabel = GetNode<Label>("%CountDownLabel");
        const int countDownStartingNumber = 3;

        for (var number = countDownStartingNumber; number > 0; number--)
        {
            countDownLabel.Text = number.ToString();

            await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
        }
    }

    /// <summary>
    /// Uses the AnimationPlayer to play the start_resume animation.
    /// </summary>
    private void PlayRemoveBlurAnimation()
    {
        _animationPlayer.Play("start_resume");
    }

    /// <summary>
    /// Uses the AnimationPlayer to play the start_pause animation.
    /// </summary>
    private void PlayBlurAnimation()
    {
        _animationPlayer.Play("start_pause");
    }

    /// <summary>
    /// Handles input events for the PausedGame class.
    /// </summary>
    /// <param name="event">The input event to handle.</param>
    public override void _Input(InputEvent @event)
    {
        if (!InputEventChecker.IsEscapeKeyPressed(@event) || _isResumed)
            return;

        _ = StartCountDownAndResume();
    }
}