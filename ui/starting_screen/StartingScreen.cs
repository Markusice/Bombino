using Godot;

namespace Bombino.ui.starting_screen;

internal partial class StartingScreen : CanvasLayer
{
    /// <summary>
    /// Called when the node enters the scene tree.
    /// Unpauses the game if it was paused due to changing scene from paused game ui.
    /// </summary>
    public override void _EnterTree()
    {
        if (GetTree().Paused)
        {
            GetTree().Paused = false;
        }
    }
}
