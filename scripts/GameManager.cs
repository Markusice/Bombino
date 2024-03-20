namespace Bombino.scripts;

using Bombino.scripts.ui;
using Godot;
using Godot.Collections;
using persistence;

internal partial class GameManager : WorldEnvironment
{
    #region Exports

    [Export]
    private PackedScene _playerScene;

    [Export]
    private PackedScene _pausedGameScene;

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

        _pausedGameSceneInstance = _pausedGameScene.Instantiate();

        GetParent().AddChild(_pausedGameSceneInstance);

        PlayBlurAnimation();

        var resumeButton = GetResumeButton();

        resumeButton.Pressed += Resume;
    }

    private TextureButton GetResumeButton()
    {
        var buttonsContainer = _pausedGameSceneInstance.GetNode<Control>("ButtonsContainer");
        var gridContainer = buttonsContainer.GetNode<GridContainer>("GridContainer");
        var resumeButton = gridContainer.GetNode<TextureButton>("ResumeButton");
        return resumeButton;
    }

    private void PlayBlurAnimation()
    {
        var blurAnimation = _pausedGameSceneInstance.GetNode<AnimationPlayer>("BlurAnimation");
        blurAnimation.Play("start_pause");
    }

    private void Resume()
    {
        GetParent().RemoveChild(_pausedGameSceneInstance);

        GetTree().Paused = false;
        SetProcessInput(true);
    }

    private void OnResumeGame()
    {
        Resume();
    }
}
