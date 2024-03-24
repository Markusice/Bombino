namespace Bombino.scripts;

using System;
using Godot;

/// <summary>
/// Represents an enemy character in the game.
/// </summary>
internal partial class Enemy : CharacterBody3D
{
    #region Signals

    /// <summary>
    /// Signal emitted when the enemy is hit.
    /// </summar
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

    /// <summary>
    /// Called when the node is added to the scene.
    /// </summary>
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

    /// <summary>
    /// Called every frame during the physics process.
    /// </summary>
    /// <param name="delta">The time elapsed since the previous frame.</param>
    public override void _PhysicsProcess(double delta)
    {
        // Vertical velocity
        if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
        {
            _targetVelocity.Y -= gravity * (float)delta;
        }

        if (IsOnWall())
        {
            var direction = Vector3.Zero;

            var randomDirection = GetRandomDirection(directions);
            ChangeDirectionOnSelectedDirection(randomDirection, ref direction);

            GD.Print($"Enemy random direction: {randomDirection}");

            _targetVelocity.X = direction.X * _speed;
            _targetVelocity.Z = direction.Z * _speed;

            var targetPosition = Position - direction;
            LookAt(targetPosition, Vector3.Up);
        }

        Velocity = _targetVelocity;

        MoveAndSlide();

        GD.Print($"CanMoveToTileOnRight: {CanMoveToTileOnRight(Position)}");
    }

    /// <summary>
    /// Checks if the enemy can move to the tile on the right.
    /// </summary>
    /// <param name="position">The current position of the enemy.</param>
    /// <returns>True if the enemy can move to the tile on the right; otherwise, false.</returns>
    private static bool CanMoveToTileOnRight(Vector3 position)
    {
        var mapCoordinates = GameManager.GameMap.LocalToMap(
            new Vector3(position.X + 1, position.Y + 1, position.Z)
        );
        var tileId = GameManager.GameMap.GetCellItem(mapCoordinates);

        return tileId == -1;
    }

    /// <summary>
    /// Gets a random direction from the specified array of directions.
    /// </summary>
    /// <param name="directions">An array of directions to choose from.</param>
    /// <returns>A random direction.</returns>
    private static Vector3 GetRandomDirection(Vector3[] directions)
    {
        var randomIndex = new Random().Next(0, directions.Length);

        return directions[randomIndex];
    }

    /// <summary>
    /// Changes the direction based on the selected direction.
    /// </summary>
    /// <param name="selectedDirection">The selected direction.</param>
    /// <param name="direction">The current direction.</param>
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

    /// <summary>
    /// Handler for the Hit event.
    /// </summary>
    private void OnHit()
    {
        QueueFree();
    }
}
