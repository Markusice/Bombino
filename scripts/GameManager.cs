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
    public static GridMap GridMap { get; private set; }

    public static int NumberOfPlayers { get; set; } = 3;
    public static string SelectedMap { get; set; }
    public static int NumberOfRounds { get; set; }

    internal static Array<PlayerData> PlayersData { get; } = new();

    private Node _pausedGameSceneInstance;

    private static PackedScene _playerScene;

    public override void _Ready()
    {
        WorldEnvironment = this;
        GridMap = GetNode<GridMap>("GridMap");

        CreatePlayers();

        // TODO: use GameSaveHandler to load the game or create a new one
    }

    private void CreatePlayers()
	{
		switch (NumberOfPlayers)
        {
            case 2:
                CreatePlayer("blue", new Vector3(1, 1, 1));
                CreatePlayer("red", new Vector3(13, 1, 13));
                break;
            case 3:
                CreatePlayer("blue", new Vector3(1, 1, 1));
                CreatePlayer("red", new Vector3(13, 1, 13));
                CreatePlayer("yellow", new Vector3(13, 1, 1));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(NumberOfPlayers), NumberOfPlayers, null);
        }
    }

    private void CreatePlayer(String playerColor, Vector3 position)
    {
        _playerScene = ResourceLoader.Load<PackedScene>($"res://scenes/players/{playerColor}.tscn");
        var player = _playerScene.Instantiate<Player>();
        player.Position = position;
        switch (playerColor)
        {
            case "blue":
                player.PlayerData = new PlayerData(PlayerColor.Blue, PlayersActionKeys.Player1);
                break;
            case "red":
                player.PlayerData = new PlayerData(PlayerColor.Red, PlayersActionKeys.Player2);
                break;
            case "yellow":
                player.PlayerData = new PlayerData(PlayerColor.Yellow, PlayersActionKeys.Player3);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(playerColor), playerColor, null);
        }
        player.Name = playerColor;
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