using Godot;

public partial class GameManager : WorldEnvironment
{
	#region Exports

	[Export]
	private PackedScene _playerScene;

	#endregion

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
		var player1 = _playerScene.Instantiate<Player>();
		var player2 = _playerScene.Instantiate<Player>();

		player1.Position = new Vector3(0, 3, 0);
		player2.Position = new Vector3(13, 3, 13);

		player1.Name = PlayerColor.Red.ToString();
		player2.Name = PlayerColor.Blue.ToString();

		AddChild(player1);
		AddChild(player2);
	}
}
