namespace Bombino.scripts;

using Godot.Collections;
using persistence;
using Godot;
using System;

internal partial class GameManager : WorldEnvironment
{
    #region Exports

    [Export]
    private PackedScene _pausedGameScene;

    #endregion

    #region Signals

    [Signal]
    public delegate void ResumeGameEventHandler();

    #endregion

    public static WorldEnvironment WorldEnvironment { get; private set; }
    public static GridMap GameMap { get; private set; }

    public static int NumberOfPlayers { get; set; } = 3;
    public static MapType SelectedMap { get; set; } = MapType.Wide;
    public static int NumberOfRounds { get; set; }

    internal static Array<PlayerData> PlayersData { get; } = new();

    private Node _pausedGameSceneInstance;

    private static PackedScene _playerScene;

    public override void _Ready()
    {
        WorldEnvironment = this;
        
        CheckMapTypeAndCreateIt();

        CheckNumberOfPlayersAndCreateThem();

        // TODO: use GameSaveHandler to load the game or create a new one
    }

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
        CreatePlayer(PlayerColor.Blue, new Vector3(-13, 2, -14));
        CreatePlayer(PlayerColor.Red, new Vector3(-13, 2, 10));
        CreatePlayer(PlayerColor.Yellow, new Vector3(11, 2, 10));
    }

    private void CreateTwoPlayers()
    {
        CreatePlayer(PlayerColor.Blue, new Vector3(1, 2, 1));
        CreatePlayer(PlayerColor.Red, new Vector3(13, 2, 1));
    }
    private void CreatePlayer(PlayerColor playerColor, Vector3 position)
    {
        var scenePath = $"res://scenes/players/{playerColor}.tscn";
        _playerScene = ResourceLoader.Load<PackedScene>(scenePath);
        var player = _playerScene.Instantiate<Player>();

        player.Position = position;
        player.Name = playerColor.ToString();

        player.PlayerData = playerColor switch
        {
            PlayerColor.Blue => new PlayerData(playerColor, PlayersActionKeys.Player1),
            PlayerColor.Red => new PlayerData(playerColor, PlayersActionKeys.Player2),
            PlayerColor.Yellow => new PlayerData(playerColor, PlayersActionKeys.Player3),
            _ => throw new ArgumentOutOfRangeException(nameof(playerColor), playerColor, null),
        };

        AddChild(player);
    }


    public override void _Input(InputEvent @event)
    {
        if (!IsEscapeKeyPressed(@event)) return;

        Pause();
    }

    private static bool IsEscapeKeyPressed(InputEvent @event)
    {
        return @event is InputEventKey { Pressed: true, Keycode: Key.Escape };
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