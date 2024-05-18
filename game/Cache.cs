using Godot;
using Array = Godot.Collections.Array;

namespace Bombino.game;

internal partial class Cache : Node
{
    #region Fields
    private GodotThread _thread;

    #endregion

    #region Overrides

    public override void _Ready()
    {
        _thread = new GodotThread();
        _thread.Start(Callable.From(LoadResources));
    }

    public override void _ExitTree()
    {
        _thread.WaitToFinish();
    }

    #endregion

    private static void LoadResources()
    {
        ResourceLoader.Load<PackedScene>("res://bomb/bomb.tscn");
        ResourceLoader.Load<PackedScene>("res://power_up/power_up.tscn");
    }
}
