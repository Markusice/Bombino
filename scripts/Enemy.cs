namespace Bombino.scripts;

using System;
using Godot;

internal partial class Enemy : CharacterBody3D
{
    #region Signals

    [Signal]
    public delegate void HitEventHandler();

    #endregion

    private const int _speed = 10;

    private Vector3 _targetVelocity = Vector3.Zero;

    private readonly Vector3[] directions = { Vector3.Right, Vector3.Left, Vector3.Back, Vector3.Forward };

    public override void _Ready()
	{
        GD.Print($"Enemy created at: {Position}");

        var direction = Vector3.Zero;
        
		var randomDirection = GetRandomDirection(directions);

        ChangeDirectionOnSelectedDirection(randomDirection, ref direction);

        _targetVelocity.X = direction.X * _speed;
        _targetVelocity.Z = direction.Z * _speed;

        MoveAndSlide();
	}

    // public override void _PhysicsProcess(double delta)
    // {
    //     var direction = Vector3.Zero;


    //     var randomDirection = GetRandomDirection(directions);

    //     ChangeDirectionOnSelectedDirection(randomDirection, ref direction);

    //     _targetVelocity.X = direction.X * _speed;
    //     _targetVelocity.Z = direction.Z * _speed;
    // }

    public override void _PhysicsProcess(double delta)
    {
        GD.Print(Position);
    }

    private Vector3 GetRandomDirection(Vector3[] directions)
    {
        var randomIndex = new Random().Next(0, directions.Length);

        return directions[randomIndex];
    }

    private void ChangeDirectionOnSelectedDirection(Vector3 selectedDirection, ref Vector3 direction)
    {
        switch (selectedDirection)
        {
            case var _ when selectedDirection == Vector3.Right:
                direction.X += 1.0f;
                break;
            case var _ when selectedDirection == Vector3.Left:
                direction.X -= 1.0f;
                break;
            case var _ when selectedDirection == Vector3.Back:
                direction.Z += 1.0f;
                break;
            case var _ when selectedDirection == Vector3.Forward:
                direction.Z -= 1.0f;
                break;
        }
    }
}
