using System.Reflection;
using Chickensoft.GoDotTest;
using Godot;

namespace Bombino.test;

public partial class Tests : Node3D
{
    public override async void _Ready() =>
        await GoTest.RunTests(Assembly.GetExecutingAssembly(), this);
}
