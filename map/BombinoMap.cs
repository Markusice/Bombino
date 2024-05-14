using Bombino.file_system_helpers.file;
using Bombino.game.persistence.state_storage;
using Godot;
using Godot.Collections;
using FileAccess = Godot.FileAccess;

namespace Bombino.map;

/// <summary>
/// Represents an interactive grid map in the game.
/// </summary>
internal partial class BombinoMap : GridMap
{
    #region Fields
    private readonly IFileAccessManager _fileAccessManager = new FileAccessManager();

    public MapData MapData { get; set; } = new MapData();

    #endregion

    /// <summary>
    /// Sets up the map from a json file.
    /// </summary>
    /// <param name="filePath">The path to the JSON file.</param>
    public void SetUpMapFromTextFile(string filePath)
    {
        var loadFile = _fileAccessManager.LoadFile(filePath, FileAccess.ModeFlags.Read);
        var error = loadFile.Item1;
        if (error != Error.Ok)
        {
            return;
        }

        using var file = loadFile.Item2;
        var data = _fileAccessManager.GetJsonData(file);

        var lines = data["structure"].AsStringArray();
        var rowOffset = (lines.Length / 2) + 1;
        var columnOffset = (lines[0].Length / 2) + 1;

        MapData.MapSize = new Tuple<int, int>(lines[0].Length, lines.Length);

        for (var z = 0; z < lines.Length; z++)
        {
            var line = lines[z];

            for (var x = 0; x < line.Length; x++)
            {
                var positionAt0 = new Vector3I(x - columnOffset, 0, z - rowOffset);
                var positionAt1 = new Vector3I(x - columnOffset, 1, z - rowOffset);

                var cellCharacter = line[x];
                var mapCellCharacter = (MapCellCharacter)cellCharacter;

                switch (mapCellCharacter)
                {
                    case MapCellCharacter.Empty:
                        break;
                    case MapCellCharacter.Wall:
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        SetCellItem(positionAt1, (int)GridElement.WallElement);

                        break;
                    case MapCellCharacter.Floor:
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);

                        break;
                    case MapCellCharacter.Crate:
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        SetCellItem(positionAt1, (int)GridElement.CrateElement);

                        break;
                    case MapCellCharacter.BluePlayer:
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        MapData.BluePlayerPosition = SetCharacterPosition(positionAt1);

                        break;
                    case MapCellCharacter.RedPlayer:
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        MapData.RedPlayerPosition = SetCharacterPosition(positionAt1);

                        break;
                    case MapCellCharacter.YellowPlayer:
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        MapData.YellowPlayerPosition = SetCharacterPosition(positionAt1);

                        break;
                    case MapCellCharacter.Enemy:
                        SetCellItem(positionAt0, (int)GridElement.BlockElement);
                        MapData.EnemyPositions.Add(SetCharacterPosition(positionAt1));

                        break;
                    default:
                        GD.PushError(
                            $"Unknown character '{cellCharacter}' in map source file: {filePath}"
                        );
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Sets the cell item at the specified position.
    /// </summary>
    /// <param name="position">The position to set the cell item at.</param>
    /// <returns>True if the cell item was successfully set; otherwise, false.</returns>
    private Vector3 SetCharacterPosition(Vector3I position)
    {
        var characterMapPosition = MapToLocal(position);
        characterMapPosition = FixCharacterYPosition(characterMapPosition);

        return characterMapPosition;
    }

    /// <summary>
    /// Fixes the character's Y position.
    /// </summary>
    /// <param name="position">The position to fix.</param>
    /// <returns>The fixed position.</returns>
    private static Vector3 FixCharacterYPosition(Vector3 position)
    {
        return new Vector3(position.X, position.Y - 1, position.Z);
    }

    public string[] SaveCurrentGameMapState()
    {
        var mapState = new string[MapData.MapSize.Item2];

        for (var z = 0; z < MapData.MapSize.Item2; z++)
        {
            for (var x = 0; x < MapData.MapSize.Item1; x++)
            {
                var positionAt0 = new Vector3I(x, 0, z);
                var positionAt1 = new Vector3I(x, 1, z);

                var cellItem = GetCellItem(positionAt1);
                if (cellItem == InvalidCellItem)
                {
                    cellItem = GetCellItem(positionAt0);

                    if (cellItem == InvalidCellItem)
                    {
                        mapState[z] += MapCellCharacter.Empty;
                        continue;
                    }
                }
                var cellItemValue = (GridElement)cellItem;

                switch (cellItemValue)
                {
                    case GridElement.BlockElement:
                        mapState[z] += MapCellCharacter.Floor;
                        break;
                    case GridElement.WallElement:
                        mapState[z] += MapCellCharacter.Wall;
                        break;
                    case GridElement.CrateElement:
                        mapState[z] += MapCellCharacter.Crate;
                        break;
                    default:
                        mapState[z] += MapCellCharacter.Empty;
                        break;
                }
            }
        }

        return mapState;

    }
        
}
