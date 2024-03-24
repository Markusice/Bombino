namespace Bombino.scripts;

using System;
using Godot;

internal partial class Enemy : CharacterBody3D
{
    #region Signals

    [Signal]
    public delegate void HitEventHandler();

    #endregion

    public int Speed { get; set; } = 10;

    public override void _PhysicsProcess(double delta)
    {
        var directions = new Vector3[]
        {
            Vector3.Forward,
            Vector3.Back,
            Vector3.Left,
            Vector3.Right
        };

        var randomDirection = GetRandomDirection(directions);


    }

    private Vector3 GetRandomDirection(Vector3[] directions)
    {
        var randomIndex = new Random().Next(0, directions.Length);

        return directions[randomIndex];
    }
}
