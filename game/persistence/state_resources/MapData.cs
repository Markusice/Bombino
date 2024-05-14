using Bombino.map;
using Chickensoft.GoDotCollections;
using Godot;
using Godot.Collections;

namespace Bombino.game.persistence.state_storage;

/// <summary>
/// Represents the data for a map.
/// </summary>
internal partial class MapData : Resource
{
    /// <summary>
    /// The type of the map.
    /// </summary>
    [Export]
    public MapType MapType { get; set; }

    /// <summary>
    /// The initial position of the blue player.
    /// </summary>
    [Export]
    public Vector3 BluePlayerPosition { get; set; }

    /// <summary>
    /// The initial position of the red player.
    /// </summary>
    [Export]
    public Vector3 RedPlayerPosition { get; set; }

    /// <summary>
    /// The initial position of the yellow player.
    /// </summary>
    [Export]
    public Vector3 YellowPlayerPosition { get; set; }

    /// <summary>
    /// The initial positions of the enemies.
    /// </summary>
    [Export]
    public Array<Vector3> EnemyPositions { get; set; } = new Array<Vector3>();

    /// <summary>
    /// The size of the map.
    /// </summary>
    public Tuple<int, int> MapSize { get; set; }

    /// <summary>
    /// The structure of the map.
    /// </summary>
    [Export]
    public string[] Structure { get; set; }

    public MapData(MapType mapType)
    {
        MapType = mapType;
    }

    public MapData() { }
}
