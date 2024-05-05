using System.Reflection;
using Godot;
using Chickensoft.GoDotTest;

namespace Bombino;

public partial class Tests : Node3D {
  public override async void _Ready()
    => await GoTest.RunTests(Assembly.GetExecutingAssembly(), this);
}