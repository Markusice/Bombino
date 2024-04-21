using Bombino.enemy;
using Bombino.events;
using Bombino.game.persistence.state_storage;
using Bombino.game.persistence.storage_layers.game_state;
using Bombino.map;
using Bombino.player;
using Bombino.ui.game_loading_screen;
using Bombino.ui.scripts;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace Bombino.game;

/// <summary>
/// A class that manages the game.
/// </summary>
internal partial class GameManager : WorldEnvironment
{
    #region Exports

    [Export] private PackedScene _pausedGameScene;

    [Export] private PackedScene _startingScreenScene;

    [Export(PropertyHint.File, "*.tscn")] private string _mapScenePath;

    [Export(PropertyHint.File, "*.tscn")] private string _enemyScenePath;

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
    public static BombinoMap GameMap { get; private set; }

    public static int NumberOfPlayers { get; set; } = 3;
    private int _alivePlayers = NumberOfPlayers;
    
    public static MapType SelectedMap { get; set; } = MapType.Basic;
    public static int NumberOfRounds { get; set; }
    public static int CurrentRound { get; set; } = 1;

    public static Array<PlayerData> PlayersData { get; } = new();
    public static Array<EnemyData> EnemiesData { get; } = new();

    private GameLoadingScene _pausedGameSceneInstance;

    private readonly string _mapTextFilePath = $"res://map/sources/{SelectedMap.ToString().ToLower()}.json";
    private ResourceLoader.ThreadLoadStatus _mapSceneLoadStatus;
    private Array _mapSceneLoadProgress = new();

    private Godot.Collections.Dictionary<string, PlayerData> _playerScenesPathAndData = new();
    private Godot.Collections.Dictionary<string, ResourceLoader.ThreadLoadStatus> _playerScenesPathAndLoadStatus = new();
    private Array _playerSceneLoadProgress = new();
    private double _playerScenesLoadProgressSum;

    private Godot.Collections.Dictionary<ulong, EnemyData> _enemiesData = new();
    private PackedScene _enemyScene;
    private PackedScene _gameMapScene;
    private ResourceLoader.ThreadLoadStatus _enemySceneLoadStatus;
    private Array _enemySceneLoadProgress = new();

    private double _loadProgress;
    private bool _isEverythingLoaded;

    private bool _isRoundOver = false;

    #endregion

    #region MethodsForSignals

    /// <summary>
    /// Event handler for the resume game event.
    /// </summary>
    private void OnResumeGame()
    {
        Resume();
    }

    #endregion

    #region SceneUpdateMethods

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        GD.Print("---------------\tnew GameManager created\t---------------");
        SetProcessInput(false);
        WorldEnvironment = this;

        _alivePlayers = NumberOfPlayers;
        Events.Instance.PlayerDied += CheckPlayersAndOpenRoundStats;

        // CheckForSavedDataAndSetUpGame();
    }

    public override void _Process(double delta)
    {
        if (_isEverythingLoaded)
        {
            return;
        }

        // default initialized value is InvalidResource
        if (_mapSceneLoadStatus == ResourceLoader.ThreadLoadStatus.InvalidResource)
            _mapSceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(_mapScenePath, _mapSceneLoadProgress);

        if (_mapSceneLoadStatus != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            _mapSceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(_mapScenePath, _mapSceneLoadProgress);

            _loadProgress = (double)_mapSceneLoadProgress[0];
            EmitSignal(SignalName.SceneLoad, _loadProgress);

            return;
        }

        EmitLoadedProgressAndAddGameMap_IfGameMapNotAdded();

        if (_playerScenesLoadProgressSum == 0)
            _playerScenesLoadProgressSum = SetPlayerScenesStatus_And_GetLoadProgressSum();

        if (_enemySceneLoadStatus == ResourceLoader.ThreadLoadStatus.InvalidResource)
            _enemySceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(_enemyScenePath, _enemySceneLoadProgress);

        if (_enemySceneLoadStatus != ResourceLoader.ThreadLoadStatus.Loaded || IsNotEveryPlayerLoaded())
        {
            _enemySceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(_enemyScenePath, _enemySceneLoadProgress);
            _playerScenesLoadProgressSum = SetPlayerScenesStatus_And_GetLoadProgressSum();

            var loadProgressSum = _playerScenesLoadProgressSum + (double)_enemySceneLoadProgress[0];
            _loadProgress = loadProgressSum / (_playerScenesPathAndData.Count + 1);
            EmitSignal(SignalName.SceneLoad, _loadProgress);

            return;
        }

        CreatePlayersFromSavedData();
        CreateEnemiesFromSavedData();

        EmitAndSetEverythingLoaded_And_EnableInputProcess();
    }

    /// <summary>
    /// Checks the players and opens the round stats.
    /// </summary>
    private async void CheckPlayersAndOpenRoundStats(string color)
    {
        _alivePlayers--;

        if (_alivePlayers == 1)
        {
            foreach (var playerData in PlayersData)
            {
                if (!playerData.IsDead)
                {
                    playerData.Wins++;
                    break;
                }
            }
            await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
            EndRound();
        }
    }

    private void EndRound()
    {
        _isRoundOver = true;
        OpenRoundStatsScreen();
    }

    /// <summary>
    /// Opens the round stats screen.
    /// </summary>
    private void OpenRoundStatsScreen()
    {
        var roundStatsScene = ResourceLoader.Load("res://ui/rounds_stats_screen/round_stats.tscn") as PackedScene;
        GetTree().ChangeSceneToPacked(roundStatsScene);
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

    #endregion

    private void EmitLoadedProgressAndAddGameMap_IfGameMapNotAdded()
    {
        _gameMapScene = (PackedScene)ResourceLoader.LoadThreadedGet(_mapScenePath);
        
        if (_gameMapScene == null)
            return;

        GameMap = _gameMapScene.Instantiate<BombinoMap>();
        GameMap.SetUpMapFromTextFile(_mapTextFilePath);
        AddChild(GameMap);

        CheckNumberOfPlayersAndCreateThem();
        CreateEnemies();
    }

    private double SetPlayerScenesStatus_And_GetLoadProgressSum()
    {
        var playerSceneLoadProgressSum = 0.0;

        foreach (var playerScenePathAndData in _playerScenesPathAndData)
        {
            SetSceneLoadStatus_And_Progress(playerScenePathAndData, _playerScenesPathAndLoadStatus,
                _playerSceneLoadProgress);

            playerSceneLoadProgressSum += (double)_playerSceneLoadProgress[0];
        }

        return playerSceneLoadProgressSum;
    }

    private static void SetSceneLoadStatus_And_Progress<T>(KeyValuePair<string, T> item,
        IDictionary<string, ResourceLoader.ThreadLoadStatus> dictionary, Array progress)
    {
        dictionary[item.Key] = ResourceLoader.LoadThreadedGetStatus(item.Key, progress);
    }

    private bool IsNotEveryPlayerLoaded()
    {
        return _playerScenesPathAndLoadStatus.Any(item => item.Value == ResourceLoader.ThreadLoadStatus.InProgress);
    }

    private void EmitAverageLoadProgress(double sceneLoadProgressSum, int count)
    {
        var sceneLoadProgressAverage = sceneLoadProgressSum / count;

        EmitSignal(SignalName.SceneLoad, sceneLoadProgressAverage);
    }

    private void CreatePlayersFromSavedData()
    {
        PlayersData.Clear();

        foreach (var playerScenePathAndData in _playerScenesPathAndData)
        {
            var playerScene = (PackedScene)ResourceLoader.LoadThreadedGet(playerScenePathAndData.Key);
            var player = playerScene.Instantiate<Player>();

            var playerData = playerScenePathAndData.Value;

            player.PlayerData = playerData;

            PlayersData.Add(playerData);

            AddChild(player);
        }
    }

    private void CreateEnemiesFromSavedData()
    {
        _enemyScene ??= (PackedScene)ResourceLoader.LoadThreadedGet(_enemyScenePath);

        foreach (var enemyData in _enemiesData.Values)
        {
            var enemy = _enemyScene.Instantiate<Enemy>();
            enemy.EnemyData = enemyData;

            AddChild(enemy);
        }
    }

    private void EmitAndSetEverythingLoaded_And_EnableInputProcess()
    {
        EmitSignal(SignalName.EverythingLoaded);
        _isEverythingLoaded = true;

        var mainUiScene = ResourceLoader.Load("res://ui/main_ui/main_ui.tscn") as PackedScene;
        var mainUiSceneInstance = mainUiScene.Instantiate();

        AddChild(mainUiSceneInstance);

        SetProcessInput(true);
    }

    /// <summary>
    /// Checks for saved data and sets up the game.
    /// </summary>
    private void CheckForSavedDataAndSetUpGame()
    {
        if (!GameSaveHandler.IsThereSavedData(out var receivedData))
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
        RequestMapLoad();
    }

    /// <summary>
    /// Creates a game from the saved data.
    /// </summary>
    /// <param name="data"></param>
    private void CreateGameFromSavedData(Godot.Collections.Dictionary<string, Variant> data)
    {
    }

    /// <summary>
    /// Checks the map type and creates it.
    /// </summary>
    private void RequestMapLoad()
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
        SavePlayerDataAndRequestLoad(PlayerColor.Yellow, GameMap.YellowPlayerPosition);
    }

    /// <summary>
    /// Creates two players.
    /// </summary>
    private void CreateTwoPlayers()
    {
        SavePlayerDataAndRequestLoad(PlayerColor.Blue, GameMap.BluePlayerPosition);
        SavePlayerDataAndRequestLoad(PlayerColor.Red, GameMap.RedPlayerPosition);
    }

    /// <summary>
    /// Creates enemies from the game map.
    /// </summary>
    private void CreateEnemies()
    {
        foreach (var enemyPosition in GameMap.EnemyPositions) SaveEnemyDataAndRequestLoad(enemyPosition);
    }

    /// <summary>
    /// Creates a player.
    /// </summary>
    /// <param name="playerColor"></param>
    /// <param name="position"></param>
    private void SavePlayerDataAndRequestLoad(PlayerColor playerColor, Vector3 position)
    {
        var playerScenePath =
            $"res://player/player_{playerColor.ToString().ToLower()}/player_{playerColor.ToString().ToLower()}.tscn";
        
        var _playerData = new PlayerData(position, playerColor);

        foreach (var playerData in PlayersData)
        {
            if (playerData.Color == playerColor)
            {
                GD.Print("Player already exists");
                _playerData = playerData;
                _playerData.Position = position;
                break;
            }
        }

        _playerScenesPathAndData.Add(playerScenePath, _playerData);
        _playerScenesPathAndLoadStatus.Add(playerScenePath, ResourceLoader.ThreadLoadStatus.InProgress);

        ResourceLoader.LoadThreadedRequest(playerScenePath);
    }

    /// <summary>
    /// Creates an enemy.
    /// </summary>
    /// <param name="position"></param>
    private void SaveEnemyDataAndRequestLoad(Vector3 position)
    {
        if (_enemyScene == null)
            ResourceLoader.LoadThreadedRequest(_enemyScenePath);

        var enemyData = new EnemyData(position);
        _enemiesData.Add(enemyData.GetInstanceId(), enemyData);
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
        _pausedGameSceneInstance = _pausedGameScene.Instantiate<GameLoadingScene>();
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
        var resumeButton =
            _pausedGameSceneInstance.GetNode<TextureButton>("ButtonsContainer/GridContainer/ResumeButton");
        resumeButton.Pressed += OnResumeGame;
    }

    /// <summary>
    /// Adds an event to the save and exit button.
    /// </summary>
    private void AddEventToSaveAndExitButton()
    {
        var saveAndExitButton = _pausedGameSceneInstance.GetNode<TextureButton>(
            "ButtonsContainer/GridContainer/SaveAndExitButton");
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

    private bool CannotResumeGame()
    {
        return _pausedGameSceneInstance.GetNodeOrNull<PanelContainer>("ButtonsContainer") == null;
    }

    /// <summary>
    /// Removes the buttons and shows the countdown container.
    /// </summary>
    private void RemoveButtonsAndShowCountDownContainer()
    {
        _pausedGameSceneInstance.GetNode<PanelContainer>("ButtonsContainer").QueueFree();

        var countDownContainer = _pausedGameSceneInstance.GetNode<PanelContainer>("CountDownContainer");
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
        var countDownLabel = _pausedGameSceneInstance.GetNode<Label>("CountDownContainer/CountDownLabel");

        const int countDownStartingNumber = 3;
        for (var number = countDownStartingNumber; number > 0; number--)
        {
            countDownLabel.Text = number.ToString();

            await ToSignal(GetTree().CreateTimer(1), SceneTreeTimer.SignalName.Timeout);
        }
    }
}