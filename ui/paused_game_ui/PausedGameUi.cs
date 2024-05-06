using Bombino.game;
using Bombino.game.persistence.storage_layers.game_state;
using Bombino.ui.scripts;
using Godot;

namespace Bombino.ui.paused_game_ui;

/// <summary>
/// Represents the UI layer for the paused game screen.
/// </summary>
internal partial class PausedGameUi : CanvasLayer
{
    #region Exports

    [Export(PropertyHint.File, "*.tscn")]
    private string MainMenuScenePath { get; set; }

    #endregion

    #region Fields

    private AnimationPlayer AnimationPlayer { get; set; }

    private bool IsResumed { get; set; }
    private bool IsSaveClicked { get; set; }

    #endregion

    #region Overrides

    public override void _Ready()
    {
        AnimationPlayer = GetNode<AnimationPlayer>("BlurAnimation");

        PlayBlurAnimation();
    }

    /// <summary>
    /// Handles input events for the PausedGame class.
    /// </summary>
    /// <param name="event">The input event to handle.</param>
    public override async void _Input(InputEvent @event)
    {
        if (!InputEventChecker.IsEscapeKeyPressed(@event))
            return;

        await StartCountDownAndResume();
    }

    #endregion

    #region MethodsForSignals

    private async void OnResumeButtonPressed()
    {
        await StartCountDownAndResume();
    }

    private void OnSaveAndExitButtonPressed()
    {
        SaveGame();

        GetTree().ChangeSceneToFile(MainMenuScenePath);
    }

    #endregion

    /// <summary>
    /// Starts the countdown and after that emits ResumeGame signal to GameManager.
    /// </summary>
    private async Task StartCountDownAndResume()
    {
        if (IsResumed)
            return;

        IsResumed = true;

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
        AnimationPlayer.Play("start_resume");
    }

    /// <summary>
    /// Uses the AnimationPlayer to play the start_pause animation.
    /// </summary>
    private void PlayBlurAnimation()
    {
        AnimationPlayer.Play("start_pause");
    }

    private void SaveGame()
    {
        if (IsSaveClicked)
            return;

        var gameSaver = new GameSaver();
        var gameSaveHandler = new GameSaveHandler(gameSaver);

        gameSaveHandler.SaveGame();

        IsSaveClicked = true;
    }
}
