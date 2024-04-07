namespace Bombino.scripts;

using Godot;
using System;
using System.Dynamic;

/// <summary>
/// Represents an interactive grid map in the game.
/// </summary>
internal partial class BombinoMap : GridMap
{
	private Vector3 BluePlayerPosition;

	private Vector3 RedPlayerPosition;

	private Vector3 YellowPlayerPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// Gets the player position for the given color.
	/// </summary>
	/// <param name="color"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public Vector3 GetPlayerPosition(PlayerColor color)
	{
        return color switch
        {
            PlayerColor.Blue => ToGlobal(BluePlayerPosition),
            PlayerColor.Red => ToGlobal(RedPlayerPosition),
            PlayerColor.Yellow => ToGlobal(YellowPlayerPosition),
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null),
        };
    }

	/// <summary>
	/// Sets the player position for the given color.
	/// </summary>
	/// <param name="color"></param>
	/// <param name="position"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public void SetPlayerPosition(PlayerColor color, Vector3 position)
	{
		switch (color)
		{
			case PlayerColor.Blue:
				BluePlayerPosition = position;
				break;
			case PlayerColor.Red:
				RedPlayerPosition = position;
				break;
			case PlayerColor.Yellow:
				YellowPlayerPosition = position;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(color), color, null);
		}
	}

	/// <summary>
    /// Sets up the map from a json file.
    /// </summary>
    public void SetUpMapFromTextFile(string filePath)
    {
        var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        var data = Json.ParseString(file.GetAsText()).AsGodotDictionary<string, Variant>();
        String[] lines = (string[])data["structure"];

        for (int z = 0; z < lines.Length; z++)
        {
            string line = lines[z];

            for (int x = 0; x < line.Length; x++)
            {
                var character = line[x];

                switch (character)
                {
                    case '0':
                        break;
                    case 'W':
                        SetCellItem(new Vector3I(x, 0, z), (int)GridElement.BlockElement);
                        SetCellItem(new Vector3I(x, 1, z), (int)GridElement.WallElement);
                        break;
                    case 'F':
                        SetCellItem(new Vector3I(x, 0, z), (int)GridElement.BlockElement);
                        break;
                    case 'C':
                        SetCellItem(new Vector3I(x, 0, z), (int)GridElement.BlockElement);
                        SetCellItem(new Vector3I(x, 1, z), (int)GridElement.CrateElement);
                        break;
                    case 'B':
                        SetCellItem(new Vector3I(x, 0, z), (int)GridElement.BlockElement);
                        SetPlayerPosition(PlayerColor.Blue, new Vector3(x, 1, z));
                        break;
                    case 'R':
                        SetCellItem(new Vector3I(x, 0, z), (int)GridElement.BlockElement);
                        SetPlayerPosition(PlayerColor.Red, new Vector3(x, 1, z));
                        break;
                    case 'Y':
                        SetCellItem(new Vector3I(x, 0, z), (int)GridElement.BlockElement);
                        SetPlayerPosition(PlayerColor.Yellow, new Vector3(x, 1, z));
                        break;
                }
            }
        }
    }
}