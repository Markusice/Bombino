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

public class PlayerTest : TestClass {
    private GameManager _game = default!;
    private BombinoMap _map = default!;
    private Player _player1 = default!;
    private PlayerData _playerData1 = default!;
    private Player _player2 = default!;
    private PlayerData _playerData2 = default!;
    private Fixture _fixture = default!;

    public PlayerTest(Node testScene) : base(testScene) { }


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
    public void Player_Ready_PositionIsSet()
    {
        _player1._Ready();

        _player1.Position.ShouldBe(_playerData1.Position);
    }

    [Test]
    public void Player_Ready_PlayerInputActionsIsNotNull()
    {
        _player1._Ready();

        _player1.PlayerInputActions.ShouldNotBeNull();
    }

    [Test]
    public void Player_Ready_PlayerDataIsSet()
    {
        _player1._Ready();

        _player1.PlayerData.ShouldBe(_playerData1);
    }

    [Test]
    public async Task Player_MovesLeft()
    {
        _player1._Ready();
        Vector3 initialPosition = _player1.Position;
        await _player1.HoldKeyFor(0.2f, Key.A);


        _player1.Position.X.ShouldBeLessThan(initialPosition.X);
    }

    [Test]
    public async Task Player_MovesRight()
    {
        _player1._Ready();
        Vector3 initialPosition = _player1.Position;
        await _player1.HoldKeyFor(0.2f, Key.D);

        _player1.Position.X.ShouldBeGreaterThan(initialPosition.X);
    }

    [Test]
    public async Task Player_MovesUp()
    {
        _player1._Ready();
        Vector3 initialPosition = _player1.Position;
        await _player1.HoldKeyFor(0.2f, Key.W);

        _player1.Position.Z.ShouldBeLessThan(initialPosition.Z);
    }

    [Test]
    public async Task Player_MovesDown()
    {
        _player1._Ready();
        Vector3 initialPosition = _player1.Position;
        await _player1.HoldKeyFor(0.2f, Key.S);

        _player1.Position.Z.ShouldBeGreaterThan(initialPosition.Z);
    }

    [Test]
    public async Task Player_PlacesBomb()
    {
        _player1._Ready();
        await _player1.TypeKey(Key.F);

        _player1.PlayerData.NumberOfPlacedBombs.ShouldBe(1);
    }


}