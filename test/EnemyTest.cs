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
using Bombino.enemy;
using LightMock.Generator;
using Bombino.game.persistence.state_storage;
using Bombino.map;
using GodotTestDriver.Input;
using GodotTestDriver.Util;

public class EnemyTest : TestClass {
    private GameManager _game = default!;
    private BombinoMap _map = default!;
    private Player _player1 = default!;
    private PlayerData _playerData1 = default!;
    private Player _player2 = default!;
    private PlayerData _playerData2 = default!;
    private Enemy _enemy1 = default!;
    private EnemyData _enemyData1 = default!;
    //private Enemy _enemy2 = default!;
    //private EnemyData _enemyData2 = default!;
    private Fixture _fixture = default!;

    public EnemyTest(Node testScene) : base(testScene) { }


    [Setup]
    public async Task Setup()
    {
        _fixture = new(TestScene.GetTree());

        Node mainNode = await _fixture.LoadScene<Node>("res://game/game.tscn");
        _game = mainNode.GetChild<GameManager>(0);
        mainNode.RemoveChild(_game);
        mainNode.QueueFree();
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

        _enemy1 = await _fixture.LoadScene<Enemy>("res://enemy/enemy.tscn");
        _enemyData1 = new EnemyData(_map.EnemyPositions[0]);
        //_enemy2 = await _fixture.LoadScene<Enemy>("res://enemy/enemy.tscn");
        
        _game.AddChild(_enemy1);
        //_game.AddChild(_enemy2);

        await _fixture.AddToRoot(_game);
    }

    [Cleanup]
    public async Task Cleanup()
    {
        await _fixture.Cleanup();
    }

    [Test]
    public void Enemy_IsNotNull()
    {
        _enemy1.ShouldNotBeNull();
    }

}