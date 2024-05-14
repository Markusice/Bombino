using Bombino.enemy;
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

public class EnemyTest : TestClass
{
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

    public EnemyTest(Node testScene)
        : base(testScene) { }

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
        _playerData1 = new PlayerData(_map.MapData.BluePlayerPosition, PlayerColor.Blue);
        _player1.PlayerData = _playerData1;

        _player2 = await _fixture.LoadScene<Player>("res://player/player_red/player_red.tscn");
        _playerData2 = new PlayerData(_map.MapData.RedPlayerPosition, PlayerColor.Red);
        _player2.PlayerData = _playerData2;

        _game.AddChild(_player1);
        _game.AddChild(_player2);
        GameManager.PlayersData = new Array<PlayerData> { _playerData1, _playerData2 };

        _enemy1 = await _fixture.LoadScene<Enemy>("res://enemy/enemy.tscn");
        _enemyData1 = new EnemyData(_map.MapData.EnemyPositions[0]);
        _enemy1.EnemyData = _enemyData1;
        //_enemy2 = await _fixture.LoadScene<Enemy>("res://enemy/enemy.tscn");

        _game.AddChild(_enemy1);

        GameManager.EnemiesData = new Array<EnemyData> { _enemyData1 };
        //_game.AddChild(_enemy2);

        await _fixture.AddToRoot(_game);
    }

    [Cleanup]
    public async Task Cleanup()
    {
        await _fixture.Cleanup();
    }

    [Test]
    public void Enemy_Ready_IsNotNull()
    {
        _enemy1.ShouldNotBeNull();
    }

    [Test]
    public void Enemy_Ready_EnemyDataIsSet()
    {
        _enemy1._Ready();

        _enemy1.EnemyData.ShouldBe(_enemyData1);
    }

    [Test]
    public void OnHit_EnemyHit_EnemyIsDead()
    {
        _enemy1.OnHit();

        _enemy1.EnemyData.IsDead.ShouldBeTrue();
    }
}
