namespace Bombino.scripts;

using Godot;

/// <summary>
/// Represents an interactive grid map in the game.
/// </summary>
internal partial class BombinoMap : GridMap
{
    public Vector3 BluePlayerPosition { get; private set; }
    public Vector3 RedPlayerPosition { get; private set; }
    public Vector3 YellowPlayerPosition { get; private set; }

    /// <summary>
    /// Sets up the map from a json file.
    /// </summary>
    public void SetUpMapFromTextFile(string filePath)
    {
        var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        var data = Json.ParseString(file.GetAsText()).AsGodotDictionary<string, Variant>();
        var lines = data["structure"].AsStringArray();

        for (var z = 0; z < lines.Length; z++)
        {
            var line = lines[z];

            for (var x = 0; x < line.Length; x++)
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
                        BluePlayerPosition = MapToLocal(new Vector3I(x, 1, z));
                        break;
                    case 'R':
                        SetCellItem(new Vector3I(x, 0, z), (int)GridElement.BlockElement);
                        RedPlayerPosition = MapToLocal(new Vector3I(x, 1, z));
                        break;
                    case 'Y':
                        SetCellItem(new Vector3I(x, 0, z), (int)GridElement.BlockElement);
                        YellowPlayerPosition = MapToLocal(new Vector3I(x, 1, z));
                        break;
                }
            }
        }
    }
}