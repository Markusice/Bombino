namespace Bombino.scripts;

using Godot.Collections;
using persistence;
using Godot;

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

    public static int NumberOfPlayers { get; set; } = 2;
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
		_playerScene = ResourceLoader.Load<PackedScene>("res://scenes/players/blue.tscn");
		var player1 = _playerScene.Instantiate<Player>();
		_playerScene = ResourceLoader.Load<PackedScene>("res://scenes/players/red.tscn");
		var player2 = _playerScene.Instantiate<Player>();
		_playerScene = ResourceLoader.Load<PackedScene>("res://scenes/players/yellow.tscn");
		var player3 = _playerScene.Instantiate<Player>();

		player1.Position = new Vector3(0, 3, 0);
		player2.Position = new Vector3(13, 3, 13);
		player3.Position = new Vector3(13, 3, 0);

		player1.Name = PlayerColor.Blue.ToString();
		player2.Name = PlayerColor.Red.ToString();
		player3.Name = PlayerColor.Yellow.ToString();

		AddChild(player1);
		AddChild(player2);
        AddChild(player3);
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