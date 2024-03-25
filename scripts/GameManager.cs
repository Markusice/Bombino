namespace Bombino.scripts;

using System;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using persistence;
using ui;

/// <summary>
/// A class that manages the game.
/// </summary>
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

    /// <summary>
    /// A signal that is emitted when the game is resumed.
    /// </summary>
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

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        WorldEnvironment = this;

        CheckMapTypeAndCreateIt();

        CheckNumberOfPlayersAndCreateThem();

        CheckForSavedDataAndSetUpGame();

        CreateEnemy(new Vector3I(-10, 2, -15));
    }

    /// <summary>
    /// Gets the position on the tile map.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private static Vector3 GetPositionOnTileMap(Vector3I position)
    {
        return GameMap.MapToLocal(position);
    }

    /// <summary>
    /// Checks for saved data and sets up the game.
    /// </summary>
    private void CheckForSavedDataAndSetUpGame()
    {
        if (!GameSaveHandler.IsThereSavedData(outputData: out var receivedData))
        {
            CreateNewGame();

            return;
        }

        CreateGameFromSavedData(receivedData);
    }

    /// <summary>
    /// Creates a new game.
    /// </summary>
    private void CreateNewGame() { }

    /// <summary>
    /// Creates a game from the saved data.
    /// </summary>
    /// <param name="data"></param>
    private void CreateGameFromSavedData(Dictionary<string, Variant> data) { }

    /// <summary>
    /// Checks the map type and creates it.
    /// </summary>
    private void CheckMapTypeAndCreateIt()
    {
        var scenePath = $"res://scenes/maps/{SelectedMap}.tscn";
        var mapScene = ResourceLoader.Load<PackedScene>(scenePath);
        GameMap = mapScene.Instantiate<GridMap>();

        AddChild(GameMap);
    }

    /// <summary>
    /// Checks the number of players and creates them.
    /// </summary>
    private void CheckNumberOfPlayersAndCreateThem()
    {
        if (NumberOfPlayers == 3)
        {
            CreateThreePlayers();
            return;
        }

        CreateTwoPlayers();
    }

    /// <summary>
    /// Creates three players.
    /// </summary>
    private void CreateThreePlayers()
    {
        CreateTwoPlayers();
        CreatePlayer(PlayerColor.Yellow, new Vector3I(5, 1, 4));
    }

    /// <summary>
    /// Creates two players.
    /// </summary>
    private void CreateTwoPlayers()
    {
        CreatePlayer(PlayerColor.Blue, new Vector3I(-7, 1, -8));
        CreatePlayer(PlayerColor.Red, new Vector3I(-7, 1, 4));
    }

    /// <summary>
    /// Creates a player.
    /// </summary>
    /// <param name="playerColor"></param>
    /// <param name="position"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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

    /// <summary>
    /// Creates an enemy.
    /// </summary>
    /// <param name="position"></param>
    private void CreateEnemy(Vector3 position)
    {
        var enemy = _enemyScene.Instantiate<Enemy>();
        enemy.Position = position;

        AddChild(enemy);
    }

    /// <summary>
    /// Called every frame. 'delta' is the elapsed time since the previous frame.
    /// </summary>
    /// <param name="event">Event when a key is pressed.</param>
    public override void _Input(InputEvent @event)
    {
        if (!InputEventChecker.IsEscapeKeyPressed(@event))
            return;

        Pause();
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    private void Pause()
    {
        GetTree().Paused = true;
        SetProcessInput(false);

        SetAndAddPausedGame();

        PlayBlurAnimation();

        AddEventToResumeButton();
        AddEventToSaveAndExitButton();
    }

    /// <summary>
    /// Sets and adds the paused game.
    /// </summary>
    private void SetAndAddPausedGame()
    {
        _pausedGameSceneInstance = _pausedGameScene.Instantiate();
        GetParent().AddChild(_pausedGameSceneInstance);
    }

    /// <summary>
    /// Plays the blur animation.
    /// </summary>
    private void PlayBlurAnimation()
    {
        var blurAnimation = _pausedGameSceneInstance.GetNode<AnimationPlayer>("BlurAnimation");
        blurAnimation.Play("start_pause");
    }

    /// <summary>
    /// Adds an event to the resume button.
    /// </summary>
    private void AddEventToResumeButton()
    {
        var resumeButton = _pausedGameSceneInstance.GetNode<TextureButton>(
            "ButtonsContainer/GridContainer/ResumeButton"
        );
        resumeButton.Pressed += OnResumeGame;
    }

    /// <summary>
    /// Adds an event to the save and exit button.
    /// </summary>
    private void AddEventToSaveAndExitButton()
    {
        var saveAndExitButton = _pausedGameSceneInstance.GetNode<TextureButton>(
            "ButtonsContainer/GridContainer/SaveAndExitButton"
        );
        saveAndExitButton.Pressed += OnSaveAndExit;
    }

    /// <summary>
    /// Event handler for the resume game event.
    /// </summary>
    private void OnResumeGame()
    {
        Resume();
    }

    /// <summary>
    /// Event handler for the save and exit event.
    /// </summary>
    private void OnSaveAndExit()
    {
        GameSaveHandler.SaveGame();

        GetTree().ChangeSceneToPacked(_startingScreenScene);
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    private async void Resume()
    {
        RemoveButtonsAndShowCountDownContainer();

        await StartCountDown();

        GetParent().RemoveChild(_pausedGameSceneInstance);

        GetTree().Paused = false;
        SetProcessInput(true);
    }

    /// <summary>
    /// Removes the buttons and shows the countdown container.
    /// </summary>
    private void RemoveButtonsAndShowCountDownContainer()
    {
        _pausedGameSceneInstance.GetNode<PanelContainer>("ButtonsContainer").QueueFree();

        var countDownContainer = _pausedGameSceneInstance.GetNode<PanelContainer>(
            "CountDownContainer"
        );
        countDownContainer.Visible = true;
    }

    /// <summary>
    /// Starts the countdown.
    /// </summary>
    /// <returns></returns>
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
