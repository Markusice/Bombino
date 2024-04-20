using Godot;
using Godot.Collections;
using FileAccess = Godot.FileAccess;

namespace Bombino.map;

/// <summary>
/// Represents an interactive grid map in the game.
/// </summary>
internal partial class BombinoMap : GridMap
{
    public Vector3 BluePlayerPosition { get; private set; }
    public Vector3 RedPlayerPosition { get; private set; }
    public Vector3 YellowPlayerPosition { get; private set; }
    public Array<Vector3> EnemyPositions { get; private set; } = new();

    /// <summary>
    /// Sets up the map from a json file.
    /// </summary>
    public void SetUpMapFromTextFile(string filePath)
    {
        var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        var data = Json.ParseString(file.GetAsText()).AsGodotDictionary<string, Variant>();
        var lines = data["structure"].AsStringArray();
        var rowOffset = (lines.Length / 2) + 1;
        var columnOffset = (lines[0].Length / 2) + 1;

        for (var z = 0; z < lines.Length; z++)
        {
            var line = lines[z];

            for (var x = 0; x < line.Length; x++)
            {
                var positionAt0 = new Vector3I(x - columnOffset, 0, z - rowOffset);
                var positionAt1 = new Vector3I(x - columnOffset, 1, z - rowOffset);

                var character = line[x];

                switch (character)
                {
                    case '0':
                        break;
                    case 'W':
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        SetCellItem(positionAt1, (int)GridElement.WallElement);
                        break;
                    case 'F':
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        break;
                    case 'C':
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        SetCellItem(positionAt1, (int)GridElement.CrateElement);
                        break;
                    case 'B':
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        BluePlayerPosition = MapToLocal(positionAt1);
                        break;
                    case 'R':
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        RedPlayerPosition = MapToLocal(positionAt1);
                        break;
                    case 'Y':
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        YellowPlayerPosition = MapToLocal(positionAt1);
                        break;
                    case 'E':
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        EnemyPositions.Add(MapToLocal(positionAt1));
                        break;
                }
            }
        }
    }
}