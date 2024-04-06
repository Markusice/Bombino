namespace Bombino.scripts;

using System.Linq;
using System;
using Godot;
using Godot.Collections;

internal partial class MapLoader : Node
{

    /// <summary>
    /// Creates a loadable scene from a text file.
    /// </summary>
    public static void CreateMapFromTextFile(string filePath, string scenePath)
    {
        var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        var data = Json.ParseString(file.GetAsText()).AsGodotDictionary<string, Variant>();
        String[] lines = (string[])data["structure"];
        InteractiveGridMap map = new();

        for (int y = 0; y < lines.Length; y++)
        {
            string line = lines[y];

            for (int x = 0; x < line.Length; x++)
            {
                var character = line[x];

                switch (character)
                {
                    case '0':
                        break;
                    case 'W':
                        map.SetCellItem(new Vector3I(x, y, 0), (int)GridElement.BlockElement);
                        map.SetCellItem(new Vector3I(x, y, 1), (int)GridElement.WallElement);
                        break;
                    case 'F':
                        map.SetCellItem(new Vector3I(x, y, 0), (int)GridElement.BlockElement);
                        break;
                    case 'C':
                        map.SetCellItem(new Vector3I(x, y, 0), (int)GridElement.BlockElement);
                        map.SetCellItem(new Vector3I(x, y, 1), (int)GridElement.CrateElement);
                        break;
                    case 'B':
                        map.SetCellItem(new Vector3I(x, y, 0), (int)GridElement.BlockElement);
                        map.BluePlayerPosition = new Vector3(x, y, 1);
                        break;
                    case 'R':
                        map.SetCellItem(new Vector3I(x, y, 0), (int)GridElement.BlockElement);
                        map.RedPlayerPosition = new Vector3(x, y, 1);
                        break;
                    case 'Y':
                        map.SetCellItem(new Vector3I(x, y, 0), (int)GridElement.BlockElement);
                        map.YellowPlayerPosition = new Vector3(x, y, 1);
                        break;
                }
                GD.Print($"Character: {character}, x: {x}, y: {y}");
            }
        }

        // save the map to a scene file
        PackedScene packedScene = new();
        packedScene.Pack(map);
        ResourceSaver.Save(packedScene, scenePath);
        GD.Print($"Map saved to {scenePath}");
    }
}