using Godot;

public partial class GameManager : WorldEnvironment
{

	private static PackedScene _playerScene;
	public static WorldEnvironment WorldEnvironment { get; private set; }
	public static GridMap GridMap { get; private set; }

	public override void _Ready()
	{
		WorldEnvironment = this;
		GridMap = GetNode<GridMap>("GridMap");

		SetUpPlayers();
	}

	private void SetUpPlayers()
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
}
