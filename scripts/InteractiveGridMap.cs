namespace Bombino.scripts;

using Godot;
using System;

/// <summary>
/// Represents an interactive grid map in the game.
/// </summary>
internal partial class InteractiveGridMap : GridMap
{
	public Vector3 BluePlayerPosition { get; set; }

	public Vector3 RedPlayerPosition { get; set; }

	public Vector3 YellowPlayerPosition { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}