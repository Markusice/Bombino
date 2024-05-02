namespace Bombino.test;

using System.Threading.Tasks;
using Godot;
using Chickensoft.GoDotTest;
using Chickensoft.GoDotLog;
using GodotTestDriver;
using GodotTestDriver.Drivers;
using NUnit.Framework;
using Shouldly;
using Bombino.game;
using Bombino;


public class GameManagerTest : TestClass {
  private readonly ILog _log = new GDLog(nameof(GameManagerTest));

  private Node _mainNode = default!;
  private GameManager _game = default!;
  private Fixture _fixture = default!;

  public GameManagerTest(Node testScene) : base(testScene) { }

  [SetupAll]
  public async Task SetupAll() 
  {
    _log.Print("Setup everything");
    _fixture = new Fixture(TestScene.GetTree());
    _mainNode = await _fixture.LoadAndAddScene<Node>("res://game/game.tscn");
    _game = _mainNode.GetChild<GameManager>(0);
    
  } 

  [Setup]
  public void SetupGameManagerIsNull()
  {
  _log.Print("Setup");
  }

  [Test]
  public void TestGameManagerIsNull()
  {
    _game.ShouldNotBeNull();
  } 

  [Cleanup]
  public void CleanupGameManagerIsNull() 
  {
    _log.Print("Cleanup");
  }

  [CleanupAll]
  public void CleanupAll() => _fixture.Cleanup();

  [Failure]
  public void Failure() =>
    _log.Print("Runs whenever any of the tests in this suite fail.");
}