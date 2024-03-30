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

    [Signal]
    public delegate void SceneLoadEventHandler(double progress);

    [Signal]
    public delegate void EverythingLoadedEventHandler();

    #endregion

    #region Fields

    public static WorldEnvironment WorldEnvironment { get; private set; }
    public static GridMap GameMap { get; private set; }

    public static int NumberOfPlayers { get; set; } = 3;
    public static MapType SelectedMap { get; set; } = MapType.Basic;
    public static int NumberOfRounds { get; set; }

    public static Array<PlayerData> PlayersData { get; } = new();

    private Node _pausedGameSceneInstance;

    private readonly string _mapScenePath =
        $"res://scenes/maps/{SelectedMap.ToString().ToLower()}.tscn";

    private ResourceLoader.ThreadLoadStatus _mapSceneLoadStatus;
    private Godot.Collections.Array _mapSceneLoadProgress = new();

    private Dictionary<string, PlayerData> _playersScenePathAndData = new();

    private Dictionary<string, ResourceLoader.ThreadLoadStatus> _playersScenePathAndLoadStatus =
        new();

    private Godot.Collections.Array _playerSceneLoadProgress = new();

    #endregion

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        SetProcessInput(false);
        WorldEnvironment = this;

        // CheckForSavedDataAndSetUpGame();

        // CreateEnemy(new Vector3I(-10, 2, -15));
    }

    public override void _Process(double delta)
    {
        if (_playersScenePathAndData == null)
            return;

        // If map scene is not loaded then get its status and send signal with progress

        if (_mapSceneLoadStatus != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            _mapSceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(
                _mapScenePath,
                _mapSceneLoadProgress
            );

            EmitSignal(SignalName.SceneLoad, (double)_mapSceneLoadProgress[0]);

            return;
        }

        // mapScene is loaded
        // Get the game map scene and instantiate it

        var gameMapScene = (PackedScene)ResourceLoader.LoadThreadedGet(_mapScenePath);
        if (gameMapScene != null) // If gameMapScene is not null then instantiate it
        {
            GameMap = gameMapScene.Instantiate<GridMap>();
            AddChild(GameMap);
        }

        var playerSceneLoadProgressSum = 0.0;

        var loadedPlayersScenes = new Array<string>();

        foreach (var item in _playersScenePathAndData)
        {
            if (_playersScenePathAndLoadStatus[item.Key] != ResourceLoader.ThreadLoadStatus.Loaded)
            {
                _playersScenePathAndLoadStatus[item.Key] = ResourceLoader.LoadThreadedGetStatus(
                    item.Key,
                    _playerSceneLoadProgress
                );

                playerSceneLoadProgressSum += (double)_playerSceneLoadProgress[0];

                continue;
            }

            // playerScene is loaded

            var playerScene = (PackedScene)ResourceLoader.LoadThreadedGet(item.Key);
            var player = playerScene.Instantiate<Player>();

            player.PlayerData = item.Value;

            AddChild(player);

            loadedPlayersScenes.Add(item.Key);
        }

        foreach (var loadedPlayerScene in loadedPlayersScenes)
        {
            _playersScenePathAndData.Remove(loadedPlayerScene);
        }

        if (_playersScenePathAndData.Count != 0) // If there are still players to load then return
        {
            var playerSceneLoadProgressAverage =
                playerSceneLoadProgressSum / _playersScenePathAndData.Count;

            EmitSignal(SignalName.SceneLoad, playerSceneLoadProgressAverage);

            return;
        }

        EmitSignal(SignalName.EverythingLoaded);
        _playersScenePathAndData = null;
    }

    #region MethodsForSignals

    /// <summary>
    /// Event handler for the resume game event.
    /// </summary>
    private void OnResumeGame()
    {
        Resume();
    }

    #endregion

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
    public void CreateNewGame()
    {
        CheckMapTypeAndCreateIt();

        CheckNumberOfPlayersAndCreateThem();
    }

    /// <summary>
    /// Creates a game from the saved data.
    /// </summary>
    /// <param name="data"></param>
    private void CreateGameFromSavedData(Dictionary<string, Variant> data)
    {
    }

    /// <summary>
    /// Checks the map type and creates it.
    /// </summary>
    private void CheckMapTypeAndCreateIt()
    {
        ResourceLoader.LoadThreadedRequest(_mapScenePath);
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
        CreatePlayer(PlayerColor.Yellow, new Vector3(11, 2, 9));
    }

    /// <summary>
    /// Creates two players.
    /// </summary>
    private void CreateTwoPlayers()
    {
        CreatePlayer(PlayerColor.Blue, new Vector3(-13, 2, -15));
        CreatePlayer(PlayerColor.Red, new Vector3(-13, 2, 9));
    }

    /// <summary>
    /// Creates a player.
    /// </summary>
    /// <param name="playerColor"></param>
    /// <param name="position"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void CreatePlayer(PlayerColor playerColor, Vector3 position)
    {
        var playerScenePath = $"res://scenes/players/{playerColor.ToString().ToLower()}.tscn";
        var playerData = new PlayerData(position, playerColor);

        _playersScenePathAndData.Add(playerScenePath, playerData);
        _playersScenePathAndLoadStatus.Add(
            playerScenePath,
            ResourceLoader.ThreadLoadStatus.InProgress
        );

        ResourceLoader.LoadThreadedRequest(playerScenePath);
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
        if (CannotResumeGame())
            return;

        RemoveButtonsAndShowCountDownContainer();

        PlayUnBlurAnimation();

        await StartCountDown();

        _pausedGameSceneInstance.QueueFree();

        GetTree().Paused = false;
        SetProcessInput(true);
    }

    private bool CannotResumeGame() =>
        _pausedGameSceneInstance.GetNodeOrNull<PanelContainer>("ButtonsContainer") == null;

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

    private void PlayUnBlurAnimation()
    {
        var blurAnimation = _pausedGameSceneInstance.GetNode<AnimationPlayer>("BlurAnimation");
        blurAnimation.Play("start_resume");
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
