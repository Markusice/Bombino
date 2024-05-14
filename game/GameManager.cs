using Bombino.enemy;
using Bombino.events;
using Bombino.map;
using Bombino.player;
using Bombino.ui.main_ui;
using Bombino.ui.paused_game_ui;
using Bombino.ui.scripts;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using EnemyData = Bombino.game.persistence.state_resources.EnemyData;
using PlayerData = Bombino.game.persistence.state_resources.PlayerData;

namespace Bombino.game;

/// <summary>
/// A class that manages the game.
/// </summary>
internal partial class GameManager : WorldEnvironment
{
    #region Exports

    [Export(PropertyHint.File, "*.tscn")]
    private string PausedGameScenePath { get; set; }

    [Export(PropertyHint.File, "*.tscn")]
    private string StartingScreenScenePath { get; set; }

    [Export(PropertyHint.File, "*.tscn")]
    private string RoundStatsScenePath { get; set; }

    [Export(PropertyHint.File, "*.tscn")]
    private string MainUiScenePath { get; set; }

    [Export(PropertyHint.File, "*.tscn")]
    private string MapScenePath { get; set; }

    [Export(PropertyHint.File, "*.tscn")]
    private string EnemyScenePath { get; set; }

    #endregion

    #region Signals

    /// <summary>
    /// A signal that is emitted when the game is resumed.
    /// </summary>
    [Signal]
    public delegate void ResumeGameEventHandler();

    [Signal]
    public delegate void GameEndedEventHandler();

    [Signal]
    public delegate void SceneLoadEventHandler(double progress);

    [Signal]
    public delegate void EverythingLoadedEventHandler();

    #endregion

    #region Fields

    private MainUi MainUi { get; set; }
    public static WorldEnvironment WorldEnvironment { get; private set; }
    public static BombinoMap GameMap { get; set; }

    public static MapType SelectedMap { get; set; } = MapType.Basic;
    public static ushort NumberOfRounds { get; set; }
    public static ushort CurrentRound { get; private set; } = 1;
    public static ushort NumberOfPlayers { get; set; } = 3;
    private ushort _alivePlayers = NumberOfPlayers;
    public static PlayerColor? CurrentWinner { get; private set; }

    public static Array<PlayerData> PlayersData { get; set; } = new();
    public static Array<EnemyData> EnemiesData { get; set; } = new();

    private PackedScene _gameMapScene;
    private readonly string _mapTextFilePath =
        $"res://map/sources/{SelectedMap.ToString().ToLower()}.json";
    private ResourceLoader.ThreadLoadStatus _mapSceneLoadStatus;
    private Array _mapSceneLoadProgress = new();

    private Godot.Collections.Dictionary<string, PlayerData> _playerScenesPathAndData = new();
    private Godot.Collections.Dictionary<
        string,
        ResourceLoader.ThreadLoadStatus
    > _playerScenesPathAndLoadStatus = new();
    private Array _playerSceneLoadProgress = new();
    private double _playerScenesLoadProgressSum;

    private PackedScene _enemyScene;
    private Godot.Collections.Dictionary<ulong, EnemyData> _enemiesDataWithUid = new();
    private ResourceLoader.ThreadLoadStatus _enemySceneLoadStatus;
    private Array _enemySceneLoadProgress = new();

    private double _loadProgress;

    private bool _isEverythingLoaded;
    private bool _isRoundOver;

    #endregion

    #region MethodsForSignals

    /// <summary>
    /// Event handler for the resume game event.
    /// </summary>
    private void OnResumeGame()
    {
        GetTree().Paused = false;
        SetProcessInput(true);
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        SetProcessInput(false);

        WorldEnvironment = this;
        _alivePlayers = NumberOfPlayers;

        Events.Instance.PlayerDied += CheckPlayersAndOpenRoundStats;
        GameEnded += GameOver;
    }

    /// <summary>
    /// Called when the node is removed from the scene tree.
    /// </summary>
    /// <param name="disposing">True if the node is being disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        Events.Instance.PlayerDied -= CheckPlayersAndOpenRoundStats;

        base.Dispose(disposing);
    }

    /// <summary>
    /// A method that is called every frame.
    /// </summary>
    /// <param name="delta">The time since the previous frame.</param>
    public override void _Process(double delta)
    {
        if (_isEverythingLoaded)
            return;

        // default initialized value is InvalidResource
        if (_mapSceneLoadStatus == ResourceLoader.ThreadLoadStatus.InvalidResource)
            _mapSceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(
                MapScenePath,
                _mapSceneLoadProgress
            );

        if (_mapSceneLoadStatus != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            _mapSceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(
                MapScenePath,
                _mapSceneLoadProgress
            );

            _loadProgress = (double)_mapSceneLoadProgress[0];
            EmitSignal(SignalName.SceneLoad, _loadProgress);

            return;
        }

        EmitLoadedProgressAndAddGameMap_IfGameMapNotAdded();

        if (_playerScenesLoadProgressSum == 0)
            _playerScenesLoadProgressSum = SetPlayerScenesStatus_And_GetLoadProgressSum();

        if (_enemySceneLoadStatus == ResourceLoader.ThreadLoadStatus.InvalidResource)
            _enemySceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(
                EnemyScenePath,
                _enemySceneLoadProgress
            );

        if (
            _enemySceneLoadStatus != ResourceLoader.ThreadLoadStatus.Loaded
            || IsNotEveryPlayerLoaded()
        )
        {
            _enemySceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(
                EnemyScenePath,
                _enemySceneLoadProgress
            );
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
    /// Pauses the game if Escape key is pressed.
    /// </summary>
    /// <param name="event">Event when a key is pressed.</param>
    public override void _Input(InputEvent @event)
    {
        if (!InputEventChecker.IsEscapeKeyPressed(@event))
            return;

        Pause();
    }

    #endregion

    /// <summary>
    /// Checks the players and opens the round stats.
    /// </summary>
    /// <param name="color">The color of the player.</param>
    private async void CheckPlayersAndOpenRoundStats(string color)
    {
        _alivePlayers--;

        await ToSignal(GetTree().CreateTimer(Mathf.Pi, false), SceneTreeTimer.SignalName.Timeout);

        if (_alivePlayers > 1 || _isRoundOver)
            return;

        if (_alivePlayers == 1)
            CheckForWinner();
        else if (_alivePlayers == 0)
            CurrentWinner = null;

        EndRound();
    }

    /// <summary>
    /// Checks for the winner of the round.
    /// </summary>
    private void CheckForWinner()
    {
        foreach (var playerData in PlayersData)
        {
            if (playerData.IsDead)
                continue;

            CurrentWinner = playerData.Color;
            playerData.Wins++;

            break;
        }
    }

    /// <summary>
    /// Ends the round.
    /// </summary>
    private void EndRound()
    {
        _isRoundOver = true;
        DestroyCurrentGameState();
        OpenRoundStatsScreen();
    }

    /// <summary>
    /// Opens the round stats screen.
    /// </summary>
    private void OpenRoundStatsScreen()
    {
        MainUi.QueueFree();

        var roundStatsScene = ResourceLoader.Load<PackedScene>(RoundStatsScenePath);

        GetParent().AddChild(roundStatsScene.Instantiate());
    }

    /// <summary>
    /// Starts the next round.
    /// </summary>
    public void StartNextRound()
    {
        CurrentRound++;
        _isRoundOver = false;
        _alivePlayers = NumberOfPlayers;

        GameMap.SetUpMapFromTextFile(_mapTextFilePath);

        RecreatePlayersForNextRound();
        AddMainUi(); // add here to reflect changes happened in the players' data

        RecreateEnemiesForNextRound();
    }

    /// <summary>
    /// Ends the game by setting all the static fields to default values,
    /// clearing the player and enemy data, and destroying the game map.
    /// </summary>
    public void GameOver()
    {
        CurrentRound = 1;
        PlayersData = new Array<PlayerData>();
        EnemiesData = new Array<EnemyData>();

        foreach (var node in GameMap.GetChildren())
        {
            node.QueueFree();
        }

        QueueFree();
    }

    /// <summary>
    /// Destroys the current game state, making it ready for the next round.
    /// </summary>
    private void DestroyCurrentGameState()
    {
        foreach (var node in GameMap.GetChildren())
        {
            if (node is BombinoMap map)
            {
                map.Clear();
                continue;
            }

            node.QueueFree();
        }
    }

    /// <summary>
    /// Recreates the players for the next round.
    /// </summary>
    private void RecreatePlayersForNextRound()
    {
        foreach (var playerData in PlayersData)
        {
            var playerScenePath =
                $"res://player/player_{playerData.Color.ToString().ToLower()}/player_{playerData.Color.ToString().ToLower()}.tscn";
            var playerScene = (PackedScene)ResourceLoader.Load(playerScenePath);
            var player = playerScene.Instantiate<Player>();

            playerData.ResetToNewRound();
            var tempPlayerData = playerData;
            playerData.Position = playerData.Color switch
            {
                PlayerColor.Blue => GameMap.MapData.BluePlayerPosition,
                PlayerColor.Red => GameMap.MapData.RedPlayerPosition,
                PlayerColor.Yellow => GameMap.MapData.YellowPlayerPosition,
                _ => tempPlayerData.Position
            };
            player.PlayerData = playerData;

            GameMap.AddChild(player);
        }
    }

    /// <summary>
    /// Recreates the enemies for the next round.
    /// </summary>
    private void RecreateEnemiesForNextRound()
    {
        foreach (var enemyData in _enemiesDataWithUid.Values)
        {
            var enemy = _enemyScene.Instantiate<Enemy>();
            enemy.EnemyData = enemyData;

            GameMap.AddChild(enemy);
        }
    }

    /// <summary>
    /// Emits the loaded progress and adds the game map if it is not added.
    /// </summary>
    private void EmitLoadedProgressAndAddGameMap_IfGameMapNotAdded()
    {
        _gameMapScene = (PackedScene)ResourceLoader.LoadThreadedGet(MapScenePath);

        if (_gameMapScene == null)
            return;

        GameMap = _gameMapScene.Instantiate<BombinoMap>();
        GameMap.SetUpMapFromTextFile(_mapTextFilePath);
        AddChild(GameMap);

        CheckNumberOfPlayersAndCreateThem();
        CreateEnemies();
    }

    /// <summary>
    /// Sets the player scenes status and gets the load progress sum.
    /// </summary>
    private double SetPlayerScenesStatus_And_GetLoadProgressSum()
    {
        var playerSceneLoadProgressSum = 0.0;

        foreach (var playerScenePathAndData in _playerScenesPathAndData)
        {
            SetSceneLoadStatus_And_Progress(
                playerScenePathAndData,
                _playerScenesPathAndLoadStatus,
                _playerSceneLoadProgress
            );

            playerSceneLoadProgressSum += (double)_playerSceneLoadProgress[0];
        }

        return playerSceneLoadProgressSum;
    }

    /// <summary>
    /// Sets the scene load status and progress.
    /// </summary>
    private static void SetSceneLoadStatus_And_Progress<T>(
        KeyValuePair<string, T> item,
        IDictionary<string, ResourceLoader.ThreadLoadStatus> dictionary,
        Array progress
    )
    {
        dictionary[item.Key] = ResourceLoader.LoadThreadedGetStatus(item.Key, progress);
    }

    /// <summary>
    /// Emits the scene load signal.
    /// </summary>
    /// <returns>True if not every player is loaded; otherwise, false.</returns>
    private bool IsNotEveryPlayerLoaded()
    {
        return _playerScenesPathAndLoadStatus.Any(item =>
            item.Value == ResourceLoader.ThreadLoadStatus.InProgress
        );
    }

    /// <summary>
    /// Emits the scene load signal.
    /// </summary>
    /// <param name="sceneLoadProgressSum">The sum of the scene load progress.</param>
    /// <param name="count">The number of players.</param>
    private void EmitAverageLoadProgress(double sceneLoadProgressSum, int count)
    {
        var sceneLoadProgressAverage = sceneLoadProgressSum / count;

        EmitSignal(SignalName.SceneLoad, sceneLoadProgressAverage);
    }

    /// <summary>
    /// Creates the players from the saved data.
    /// </summary>
    private void CreatePlayersFromSavedData()
    {
        foreach (var playerScenePathAndData in _playerScenesPathAndData)
        {
            var playerScene = (PackedScene)
                ResourceLoader.LoadThreadedGet(playerScenePathAndData.Key);
            var player = playerScene.Instantiate<Player>();

            var playerData = playerScenePathAndData.Value;
            player.PlayerData = playerData;

            PlayersData.Add(playerData);

            GameMap.AddChild(player);
        }
    }

    /// <summary>
    /// Creates the enemies from the saved data.
    /// </summary>
    private void CreateEnemiesFromSavedData()
    {
        _enemyScene ??= (PackedScene)ResourceLoader.LoadThreadedGet(EnemyScenePath);

        foreach (var enemyData in _enemiesDataWithUid.Values)
        {
            var enemy = _enemyScene.Instantiate<Enemy>();

            enemy.EnemyData = enemyData;
            EnemiesData.Add(enemyData);

            GameMap.AddChild(enemy);
        }
    }

    /// <summary>
    /// Emits the everything loaded signal and enables the input process.
    /// </summary>
    private void EmitAndSetEverythingLoaded_And_EnableInputProcess()
    {
        EmitSignal(SignalName.EverythingLoaded);
        _isEverythingLoaded = true;

        AddMainUi();

        SetProcessInput(true);
    }

    /// <summary>
    /// Adds the main UI.
    /// </summary>
    private void AddMainUi()
    {
        var mainUiScene = ResourceLoader.Load<PackedScene>(MainUiScenePath);

        MainUi = mainUiScene.Instantiate<MainUi>();

        AddChild(MainUi);
    }

    /// <summary>
    /// Creates a new game.
    /// </summary>
    public void CreateNewGame()
    {
        RequestMapLoad();
    }

    /// <summary>
    /// Requests the map load.
    /// </summary>
    private void RequestMapLoad()
    {
        ResourceLoader.LoadThreadedRequest(MapScenePath);
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
        SavePlayerDataAndRequestLoad(PlayerColor.Yellow, GameMap.MapData.YellowPlayerPosition);
    }

    /// <summary>
    /// Creates two players.
    /// </summary>
    private void CreateTwoPlayers()
    {
        SavePlayerDataAndRequestLoad(PlayerColor.Blue, GameMap.MapData.BluePlayerPosition);
        SavePlayerDataAndRequestLoad(PlayerColor.Red, GameMap.MapData.RedPlayerPosition);
    }

    /// <summary>
    /// Creates enemies from the game map.
    /// </summary>
    private void CreateEnemies()
    {
        foreach (var enemyPosition in GameMap.MapData.EnemyPositions)
            SaveEnemyDataAndRequestLoad(enemyPosition);
    }

    /// <summary>
    /// Saves the player data and requests the load.
    /// </summary>
    /// <param name="playerColor"></param>
    /// <param name="position"></param>
    private void SavePlayerDataAndRequestLoad(PlayerColor playerColor, Vector3 position)
    {
        var playerScenePath =
            $"res://player/player_{playerColor.ToString().ToLower()}/player_{playerColor.ToString().ToLower()}.tscn";

        var playerData = new PlayerData(position, playerColor);

        _playerScenesPathAndData.Add(playerScenePath, playerData);
        _playerScenesPathAndLoadStatus.Add(
            playerScenePath,
            ResourceLoader.ThreadLoadStatus.InProgress
        );

        ResourceLoader.LoadThreadedRequest(playerScenePath);
    }

    /// <summary>
    /// Saves the enemy data and requests the load.
    /// </summary>
    /// <param name="position"></param>
    private void SaveEnemyDataAndRequestLoad(Vector3 position)
    {
        if (_enemyScene == null)
            ResourceLoader.LoadThreadedRequest(EnemyScenePath);

        var enemyData = new EnemyData(position);
        _enemiesDataWithUid.Add(enemyData.GetInstanceId(), enemyData);
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    private void Pause()
    {
        GetTree().Paused = true;
        SetProcessInput(false);

        AddPausedScreen();
    }

    /// <summary>
    /// Adds the paused screen.
    /// </summary>
    private void AddPausedScreen()
    {
        var pausedGamePackedScene = ResourceLoader.Load<PackedScene>(PausedGameScenePath);

        GetParent().AddChild(pausedGamePackedScene.Instantiate<PausedGameUi>());
    }
}
