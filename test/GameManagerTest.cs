namespace Bombino.test;

using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Chickensoft.GoDotTest;
using Chickensoft.AutoInject;
using GodotTestDriver;
using GodotTestDriver.Drivers;
using NUnit.Framework;
using Shouldly;
using LightMoq;
using Bombino.game;
using Bombino.player;
using LightMock.Generator;
using Bombino.game.persistence.state_storage;
using Bombino.map;
using GodotTestDriver.Input;

public class GameManagerTest : TestClass {
    private GameManager _game = default!;
    private BombinoMap _map = default!;
    private Player _player1 = default!;
    private PlayerData _playerData1 = default!;
    private Player _player2 = default!;
    private PlayerData _playerData2 = default!;
    private Fixture _fixture = default!;

    public GameManagerTest(Node testScene) : base(testScene) { }


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
    public void GameManager_IsNotNull()
    {
        _game.ShouldNotBeNull();
    }

    [Test]
    public void GameManager_HasTwoPlayers()
    {
        GameManager.PlayersData.Count.ShouldBe(2);

    }

    [Test]
    public void GameManager_HasMap()
    {
        GameManager.GameMap.ShouldNotBeNull();

    }

}