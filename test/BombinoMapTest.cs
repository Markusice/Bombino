using Bombino.game;
using Bombino.game.persistence.state_resources;
using Bombino.map;
using Bombino.player;
using Chickensoft.GoDotTest;
using Godot;
using Godot.Collections;
using GodotTestDriver;
using Shouldly;

namespace Bombino.test;

public class BombinoMapTest : TestClass {
    private GameManager _game = default!;
    private BombinoMap _map = default!;
    private Player _player1 = default!;
    private PlayerData _playerData1 = default!;
    private Player _player2 = default!;
    private PlayerData _playerData2 = default!;
    private Fixture _fixture = default!;

    public BombinoMapTest(Node testScene) : base(testScene) { }


    [Setup]
    public async Task Setup()
    {
        _fixture = new(TestScene.GetTree());

        Node mainNode = await _fixture.LoadScene<Node>("res://game/game.tscn");
        _game = mainNode.GetChild<GameManager>(0);
        mainNode.RemoveChild(_game);
        GameManager.NumberOfPlayers = 2;

        _map = await _fixture.LoadScene<BombinoMap>("res://map/map.tscn");
        _map.SetUpMapFromTextFile("res://map/sources/basic.json");
        GameManager.GameMap = _map;

        _player1 = await _fixture.LoadScene<Player>("res://player/player_blue/player_blue.tscn");
        _playerData1 = new PlayerData(_map.BluePlayerPosition, PlayerColor.Blue);
        _player1.PlayerData = _playerData1;

        _player2 = await _fixture.LoadScene<Player>("res://player/player_red/player_red.tscn");
        _playerData2 = new PlayerData(_map.RedPlayerPosition, PlayerColor.Red);
        _player2.PlayerData = _playerData2;

        _game.AddChild(_player1);
        _game.AddChild(_player2);
        GameManager.PlayersData = new Array<PlayerData>
        {
            _playerData1,
            _playerData2
        };

        await _fixture.AddToRoot(_game);
    }

    [Cleanup]
    public async Task Cleanup()
    {
        await _fixture.Cleanup();
    }

    [Test]
    public void Map_OnGameLoad_IsNotNull()
    {
        _map.ShouldNotBeNull();
    }

    [Test]
    public void BluePlayerPosition_BasicMapLoaded_SetToCorrectValue()
    {
        var expected = _map.MapToLocal(new Vector3I(-6, 1, -6));
        expected.Y--;
        _map.BluePlayerPosition.ShouldBe(expected);
    }

    [Test]
    public void RedPlayerPosition_BasicMapLoaded_SetToCorrectValue()
    {
        var expected = _map.MapToLocal(new Vector3I(-6, 1, 4));
        expected.Y--;
        _map.RedPlayerPosition.ShouldBe(expected);
    }

    [Test]
    public void YellowPlayerPosition_BasicMapLoaded_SetToCorrectValue()
    {
        var expected = _map.MapToLocal(new Vector3I(4, 1, 4));
        expected.Y--;
        _map.YellowPlayerPosition.ShouldBe(expected);
    }

    [Test]
    public void EnemyPositions_BasicMapLoaded_SetToCorrectValue()
    {
        var expected = _map.MapToLocal(new Vector3I(4, 1, -6));
        expected.Y--;
        _map.EnemyPositions[0].ShouldBe(expected);
    }

    [Test]
    public void BluePlayerPosition_WideMapLoaded_SetToCorrectValue()
    {
        _map.Clear();
        _map.SetUpMapFromTextFile("res://map/sources/wide.json");
        var expected = _map.MapToLocal(new Vector3I(-12, 1, -6));
        expected.Y--;
        _map.BluePlayerPosition.ShouldBe(expected);
    }

    [Test]
    public void RedPlayerPosition_WideMapLoaded_SetToCorrectValue()
    {
        _map.Clear();
        _map.SetUpMapFromTextFile("res://map/sources/wide.json");
        var expected = _map.MapToLocal(new Vector3I(-12, 1, 4));
        expected.Y--;
        _map.RedPlayerPosition.ShouldBe(expected);
    }

    [Test]
    public void YellowPlayerPosition_WideMapLoaded_SetToCorrectValue()
    {
        _map.Clear();
        _map.SetUpMapFromTextFile("res://map/sources/wide.json");
        var expected = _map.MapToLocal(new Vector3I(10, 1, 4));
        expected.Y--;
        _map.YellowPlayerPosition.ShouldBe(expected);
    }

    [Test]
    public void EnemyPositions_WideMapLoaded_SetToCorrectValue()
    {
        _map.Clear();
        _map.SetUpMapFromTextFile("res://map/sources/wide.json");
        var enemyPosition1 = _map.MapToLocal(new Vector3I(10, 1, -6));
        enemyPosition1.Y--;
        var enemyPosition2 = _map.MapToLocal(new Vector3I(0, 1, -2));
        enemyPosition2.Y--;
        var expected = new Array<Vector3>
        {
            enemyPosition1,
            enemyPosition2
        };

        foreach (var position in expected)
        {
            _map.EnemyPositions.Contains(position).ShouldBeTrue();
        }
    }

    [Test]
    public void BluePlayerPosition_CrossMapLoaded_SetToCorrectValue()
    {
        _map.Clear();
        _map.SetUpMapFromTextFile("res://map/sources/cross.json");
        var expected = _map.MapToLocal(new Vector3I(-5, 1, -9));
        expected.Y--;
        _map.BluePlayerPosition.ShouldBe(expected);
    }

    [Test]
    public void RedPlayerPosition_CrossMapLoaded_SetToCorrectValue()
    {
        _map.Clear();
        _map.SetUpMapFromTextFile("res://map/sources/cross.json");
        var expected = _map.MapToLocal(new Vector3I(-5, 1, 7));
        expected.Y--;
        _map.RedPlayerPosition.ShouldBe(expected);
    }

    [Test]
    public void YellowPlayerPosition_CrossMapLoaded_SetToCorrectValue()
    {
        _map.Clear();
        _map.SetUpMapFromTextFile("res://map/sources/cross.json");
        var expected = _map.MapToLocal(new Vector3I(3, 1, 7));
        expected.Y--;
        _map.YellowPlayerPosition.ShouldBe(expected);
    }

    [Test]
    public void EnemyPositions_CrossMapLoaded_SetToCorrectValue()
    {
        _map.Clear();
        _map.SetUpMapFromTextFile("res://map/sources/cross.json");
        var enemyPosition1 = _map.MapToLocal(new Vector3I(3, 1, -9));
        enemyPosition1.Y--;
        var enemyPosition2 = _map.MapToLocal(new Vector3I(-1, 1, -4));
        enemyPosition2.Y--;
        var enemyPosition3 = _map.MapToLocal(new Vector3I(2, 1, -1));
        enemyPosition3.Y--;
        var expected = new Array<Vector3>
        {
            enemyPosition1,
            enemyPosition2,
            enemyPosition3
        };
        foreach (var position in expected)
        {
            _map.EnemyPositions.Contains(position).ShouldBeTrue();
        }
    }

    [Test]
    public void SetUpMapFromTextFile_BasicMapLoaded_MapCellsAreCorrect()
    {
        var block1 = _map.GetCellItem(new Vector3I(-7, 0, -7));
        var block2 = _map.GetCellItem(new Vector3I(-6, 0, -7));
        var block3 = _map.GetCellItem(new Vector3I(-5, 0, -7));
        var wall = _map.GetCellItem(new Vector3I(-7, 1, -7));
        var wall2 = _map.GetCellItem(new Vector3I(-6, 1, -7));
        var crate = _map.GetCellItem(new Vector3I(-4, 1, -6));
        var crate2 = _map.GetCellItem(new Vector3I(-3, 1, -6));

        block1.ShouldBe((int)GridElement.BlockElement);
        block2.ShouldBe((int)GridElement.BlockElement);
        block3.ShouldBe((int)GridElement.BlockElement);
        wall.ShouldBe((int)GridElement.WallElement);
        wall2.ShouldBe((int)GridElement.WallElement);
        crate.ShouldBe((int)GridElement.CrateElement);
        crate2.ShouldBe((int)GridElement.CrateElement);

    }

    [Test]
    public void SetUpMapFromTextFile_WideMapLoaded_MapCellsAreCorrect()
    {
        _map.Clear();
        _map.SetUpMapFromTextFile("res://map/sources/wide.json");
        var block1 = _map.GetCellItem(new Vector3I(-13, 0, -7));
        var block2 = _map.GetCellItem(new Vector3I(-12, 0, -7));
        var block3 = _map.GetCellItem(new Vector3I(-11, 0, -7));
        var wall = _map.GetCellItem(new Vector3I(-13, 1, -7));
        var wall2 = _map.GetCellItem(new Vector3I(-12, 1, -7));
        var crate = _map.GetCellItem(new Vector3I(-10, 1, -6));
        var crate2 = _map.GetCellItem(new Vector3I(-9, 1, -6));

        block1.ShouldBe((int)GridElement.BlockElement);
        block2.ShouldBe((int)GridElement.BlockElement);
        block3.ShouldBe((int)GridElement.BlockElement);
        wall.ShouldBe((int)GridElement.WallElement);
        wall2.ShouldBe((int)GridElement.WallElement);
        crate.ShouldBe((int)GridElement.CrateElement);
        crate2.ShouldBe((int)GridElement.CrateElement);
    }

    [Test]
    public void SetUpMapFromTextFile_CrossMapLoaded_MapCellsAreCorrect()
    {
        _map.Clear();
        _map.SetUpMapFromTextFile("res://map/sources/cross.json");
        var block1 = _map.GetCellItem(new Vector3I(-6, 0, -10));
        var block2 = _map.GetCellItem(new Vector3I(-5, 0, -10));
        var block3 = _map.GetCellItem(new Vector3I(-4, 0, -10));
        var wall = _map.GetCellItem(new Vector3I(-6, 1, -10));
        var wall2 = _map.GetCellItem(new Vector3I(-5, 1, -10));
        var crate = _map.GetCellItem(new Vector3I(-3, 1, -9));
        var crate2 = _map.GetCellItem(new Vector3I(-2, 1, -9));

        block1.ShouldBe((int)GridElement.BlockElement);
        block2.ShouldBe((int)GridElement.BlockElement);
        block3.ShouldBe((int)GridElement.BlockElement);
        wall.ShouldBe((int)GridElement.WallElement);
        wall2.ShouldBe((int)GridElement.WallElement);
        crate.ShouldBe((int)GridElement.CrateElement);
        crate2.ShouldBe((int)GridElement.CrateElement);
    }

        
}