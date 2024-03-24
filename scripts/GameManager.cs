namespace Bombino.scripts;

using System;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using persistence;
using ui;

internal partial class GameManager : WorldEnvironment
{
    #region Exports

    [Export]
    private PackedScene _pausedGameScene;

    [Export]
    private PackedScene _startingScreenScene;

    [Export]
    private PackedScene _enemyScene;

    #endregion

    #region Signals

    [Signal]
    public delegate void ResumeGameEventHandler();

    #endregion

    public static WorldEnvironment WorldEnvironment { get; private set; }
    public static GridMap GameMap { get; private set; }

    public static int NumberOfPlayers { get; set; } = 3;
    public static MapType SelectedMap { get; set; } = MapType.Basic;
    public static int NumberOfRounds { get; set; }

    internal static Array<PlayerData> PlayersData { get; } = new();

    private Node _pausedGameSceneInstance;

    private static PackedScene _playerScene;

    public override void _Ready()
    {
        WorldEnvironment = this;

        CheckMapTypeAndCreateIt();

        CheckNumberOfPlayersAndCreateThem();

        CheckForSavedDataAndSetUpGame();

        CreateEnemy(new Vector3I(-10, 2, -15));
    }

    private static Vector3 GetPositionOnTileMap(Vector3I position)
    {
        return GameMap.MapToLocal(position);
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

    private void CheckMapTypeAndCreateIt()
    {
        var scenePath = $"res://scenes/maps/{SelectedMap}.tscn";
        var mapScene = ResourceLoader.Load<PackedScene>(scenePath);
        GameMap = mapScene.Instantiate<GridMap>();

        AddChild(GameMap);
    }

    private void CheckNumberOfPlayersAndCreateThem()
    {
        if (NumberOfPlayers == 3)
        {
            CreateThreePlayers();
            return;
        }

        CreateTwoPlayers();
    }

    private void CreateThreePlayers()
    {
        CreateTwoPlayers();
        CreatePlayer(PlayerColor.Yellow, new Vector3I(5, 1, 4));
    }

    private void CreateTwoPlayers()
    {
        CreatePlayer(PlayerColor.Blue, new Vector3I(-7, 1, -8));
        CreatePlayer(PlayerColor.Red, new Vector3I(-7, 1, 4));
    }

    private void CreatePlayer(PlayerColor playerColor, Vector3I position)
    {
        var scenePath = $"res://scenes/players/{playerColor}.tscn";
        _playerScene = ResourceLoader.Load<PackedScene>(scenePath);
        var player = _playerScene.Instantiate<Player>();

        player.Position = GetPositionOnTileMap(position);
        player.Name = playerColor.ToString();

        GD.Print($"{player.Name} pos: {player.Position}");

        player.PlayerData = playerColor switch
        {
            PlayerColor.Blue => new PlayerData(playerColor, PlayersActionKeys.Player1),
            PlayerColor.Red => new PlayerData(playerColor, PlayersActionKeys.Player2),
            PlayerColor.Yellow => new PlayerData(playerColor, PlayersActionKeys.Player3),
            _ => throw new ArgumentOutOfRangeException(nameof(playerColor), playerColor, null),
        };

        AddChild(player);
    }

    private void CreateEnemy(Vector3 position)
    {
        var enemy = _enemyScene.Instantiate<Enemy>();
        enemy.Position = position;

        AddChild(enemy);
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
