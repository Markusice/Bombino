namespace Bombino.scripts;

using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using persistence;
using ui;

internal partial class GameManager : WorldEnvironment
{
    #region Exports

    [Export]
    private PackedScene _playerScene;

    [Export]
    private PackedScene _pausedGameScene;

    [Export]
    private PackedScene _startingScreenScene;

    #endregion

    #region Signals

    [Signal]
    public delegate void ResumeGameEventHandler();

    #endregion

    public static WorldEnvironment WorldEnvironment { get; private set; }
    public static GridMap GridMap { get; private set; }

    public static int NumberOfPlayers { get; set; } = 2;
    public static string SelectedMap { get; set; }
    public static int NumberOfRounds { get; set; }

    internal static Array<PlayerData> PlayersData { get; } = new();

    private Node _pausedGameSceneInstance;

    public override void _Ready()
    {
        WorldEnvironment = this;
        GridMap = GetNode<GridMap>("GridMap");

        CreatePlayers();

        CheckForSavedDataAndSetUpGame();
    }

    private void CheckForSavedDataAndSetUpGame()
    {
        if (!GameSaveHandler.IsThereSavedData(outputData: out var receivedData))
        {
            CreateNewGame();

            return;
        }

        CreateGameFromSavedData(receivedData);
    }

    private void CreateNewGame() { }

    private void CreateGameFromSavedData(Dictionary<string, Variant> data) { }

    private void CreatePlayers()
    {
        var player1 = _playerScene.Instantiate<Player>();
        var player2 = _playerScene.Instantiate<Player>();

        player1.Position = new Vector3(0, 2, 0);
        player2.Position = new Vector3(13, 2, 13);

        player1.PlayerData = new PlayerData(PlayerColor.Red, PlayersActionKeys.Player1);
        player2.PlayerData = new PlayerData(PlayerColor.Blue, PlayersActionKeys.Player2);

        PlayersData.Add(player1.PlayerData);
        PlayersData.Add(player2.PlayerData);

        AddChild(player1);
        AddChild(player2);
    }

    public override void _Input(InputEvent @event)
    {
        if (!InputEventChecker.IsEscapeKeyPressed(@event))
            return;

        Pause();
    }

    private void Pause()
    {
        GetTree().Paused = true;
        SetProcessInput(false);

        SetAndAddPausedGame();

        PlayBlurAnimation();

        AddEventToResumeButton();
        AddEventToSaveAndExitButton();
    }

    private void SetAndAddPausedGame()
    {
        _pausedGameSceneInstance = _pausedGameScene.Instantiate();
        GetParent().AddChild(_pausedGameSceneInstance);
    }

    private void PlayBlurAnimation()
    {
        var blurAnimation = _pausedGameSceneInstance.GetNode<AnimationPlayer>("BlurAnimation");
        blurAnimation.Play("start_pause");
    }

    private void AddEventToResumeButton()
    {
        var resumeButton = _pausedGameSceneInstance.GetNode<TextureButton>(
            "ButtonsContainer/GridContainer/ResumeButton"
        );
        resumeButton.Pressed += OnResumeGame;
    }

    private void AddEventToSaveAndExitButton()
    {
        var saveAndExitButton = _pausedGameSceneInstance.GetNode<TextureButton>(
            "ButtonsContainer/GridContainer/SaveAndExitButton"
        );
        saveAndExitButton.Pressed += OnSaveAndExit;
    }

    private void OnResumeGame()
    {
        Resume();
    }

    private void OnSaveAndExit()
    {
        GameSaveHandler.SaveGame();

        GetTree().ChangeSceneToPacked(_startingScreenScene);
    }

    private async void Resume()
    {
        RemoveButtonsAndShowCountDownContainer();

        await StartCountDown();

        GetParent().RemoveChild(_pausedGameSceneInstance);

        GetTree().Paused = false;
        SetProcessInput(true);
    }

    private void RemoveButtonsAndShowCountDownContainer()
    {
        _pausedGameSceneInstance.GetNode<PanelContainer>("ButtonsContainer").QueueFree();

        var countDownContainer = _pausedGameSceneInstance.GetNode<PanelContainer>(
            "CountDownContainer"
        );
        countDownContainer.Visible = true;
    }

    private async Task StartCountDown()
    {
        var countDownLabel = _pausedGameSceneInstance.GetNode<Label>(
            "CountDownContainer/CountDownLabel"
        );

        int countDownNumber = 3;
        for (int number = countDownNumber; number > 0; number--)
        {
            countDownLabel.Text = number.ToString();

            await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
        }
    }
}
