namespace Bombino.scripts;

using System;
using Godot;

internal partial class Enemy : CharacterBody3D
{
    #region Signals

    [Signal]
    public delegate void HitEventHandler();

    #endregion

    private const int _speed = 6;

    private Vector3 _targetVelocity = Vector3.Zero;

    private readonly Vector3[] directions =
    {
        Vector3.Right,
        Vector3.Left,
        Vector3.Back,
        Vector3.Forward
    };

    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
        GD.Print($"Enemy created at: {Position}");

        var direction = Vector3.Zero;

        var randomDirection = GetRandomDirection(directions);

        ChangeDirectionOnSelectedDirection(Vector3.Right, ref direction);

        GD.Print($"Enemy random direction: {randomDirection}");
        GD.Print($"Enemy direction: {direction}");

        _targetVelocity.X = direction.X * _speed;
        _targetVelocity.Z = direction.Z * _speed;

        var targetPosition = Position - direction;
        LookAt(targetPosition, Vector3.Up);

        Velocity = _targetVelocity;
        MoveAndSlide();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Vertical velocity
        if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
        {
            _targetVelocity.Y -= gravity * (float)delta;
        }

        Velocity = _targetVelocity;

        GD.Print($"Enemy velocity: {Velocity}");
        MoveAndSlide();
    }

    private Vector3 GetRandomDirection(Vector3[] directions)
    {
        var randomIndex = new Random().Next(0, directions.Length);

        return directions[randomIndex];
    }

    private static void ChangeDirectionOnSelectedDirection(
        Vector3 selectedDirection,
        ref Vector3 direction
    )
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
