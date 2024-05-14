using Chickensoft.GoDotTest;
using Godot;

namespace Bombino.ui.starting_screen;

#if DEBUG
using System.Reflection;
#endif

/// <summary>
/// The main scene of the game.
/// </summary>
public partial class Main : Node3D {
#if DEBUG
  public TestEnvironment Environment = default!;
#endif

  public override void _Ready() {
#if DEBUG
    // If this is a debug build, use GoDotTest to examine the
    // command line arguments and determine if we should run tests.
    Environment = TestEnvironment.From(OS.GetCmdlineArgs());
    if (Environment.ShouldRunTests) {
      CallDeferred("RunTests");
      return;
    }
#endif
    // If we don't need to run tests, we can just switch to the game scene.
    GetTree().CallDeferred("change_scene_to_file", "res://ui/starting_screen/starting_screen.tscn");
  }

#if DEBUG
  private void RunTests()
    => _ = GoTest.RunTests(Assembly.GetExecutingAssembly(), this, Environment);
#endif
}