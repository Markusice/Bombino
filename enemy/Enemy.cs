using Bombino.game;
using Bombino.player;
using Godot;
using EnemyData = Bombino.game.persistence.state_resources.EnemyData;

namespace Bombino.enemy;

/// <summary>
/// Represents an enemy character in the game.
/// </summary>
internal partial class Enemy : CharacterBody3D
{
    #region Signals

    /// <summary>
    /// Signal emitted when the enemy is hit.
    /// </summary>
    [Signal]
    public delegate void HitEventHandler();

    #endregion

    #region Fields

    private const int Speed = 5;
    private Vector3 _targetVelocity = Vector3.Zero;
    private AnimationTree _animTree;
    private AnimationNodeStateMachinePlayback _stateMachine;

    private static readonly Vector3[] Directions =
    {
        Vector3.Right,
        Vector3.Left,
        Vector3.Back,
        Vector3.Forward
    };

    private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    public EnemyData EnemyData { get; set; }

    #endregion

    /// <summary>
    /// Called when the node is added to the scene.
    /// </summary>
    public override async void _Ready()
    {   
        EnemyData.IsDead = false;
        EnemyData.CanKillPlayer = false;
        Position = EnemyData.InitialPosition;
        GD.Print($"Enemy created at: {Position}");

        _animTree = GetNode<AnimationTree>("AnimationTree");
        _animTree.Active = true;
        _stateMachine = (AnimationNodeStateMachinePlayback)_animTree.Get("parameters/playback");

        GD.Print($"Enemy created at: {Position}");

        var direction = Vector3.Zero;
        ChangeDirection(ref direction);

        GD.Print($"Enemy direction: {direction}");

        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;

        var targetPosition = Position - direction;

        LookAt(targetPosition, Vector3.Up);

        Velocity = _targetVelocity;

        MoveAndSlide();

        await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
        EnemyData.CanKillPlayer = true;
    }

    /// <summary>
    /// Called every frame during the physics process.
    /// </summary>
    /// <param name="delta">The time elapsed since the previous frame.</param>
    public override void _PhysicsProcess(double delta)
    {
        if (EnemyData.IsDead)
            return;

        // Vertical velocity
        if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
            _targetVelocity.Y -= _gravity * (float)delta;

        MoveAndSlide();

        if (IsOnWall())
        {
            var direction = Vector3.Zero;
            ChangeDirection(ref direction);

            _targetVelocity.X = direction.X * Speed;
            _targetVelocity.Z = direction.Z * Speed;

            var targetPosition = Position - direction;
            LookAt(targetPosition, Vector3.Up);
        }

        // Randomly change direction without any reason
        if (new Random().NextDouble() < 0.005) // 0.5% chance to change direction
        {
            GD.Print("Changing direction randomly");
            var direction = Vector3.Zero;
            ChangeDirection(ref direction);

            _targetVelocity.X = direction.X * Speed;
            _targetVelocity.Z = direction.Z * Speed;

            var targetPosition = Position - direction;
            LookAt(targetPosition, Vector3.Up);
        }

        EnemyData.Position = Position;

        BlendMovementAnimation();

        Velocity = _targetVelocity;
    }

    /// <summary>
    /// Called when the enemy enters the area.
    /// </summary>
    private void BlendMovementAnimation()
    {
        var animTree = GetNode<AnimationTree>("AnimationTree");
        animTree.Set("parameters/IR/blend_position", new Vector2(Velocity.X, Velocity.Z).Length());
    }

    /// <summary>
    /// Sets the state machine.
    /// </summary>
    /// <param name="stateName">The name of the state to set.</param>
    private void SetStateMachine(string stateName)
    {
        _stateMachine = (AnimationNodeStateMachinePlayback)_animTree.Get("parameters/playback");
        _stateMachine.Travel(stateName);
    }

    #region MethodsForSignals

    /// <summary>
    /// Handler for the Hit event.
    /// </summary>
    public async void OnHit()
    {
        EnemyData.IsDead = true;
        SetStateMachine("Die");
        await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
        Die();
    }

    /// <summary>
    /// Kills the enemy.
    /// </summary>
    private void Die()
    {
        QueueFree();
    }

    #endregion

    /// <summary>
    /// Changes the direction based on the selected direction.
    /// </summary>
    /// <param name="direction">The current direction.</param>
    private void ChangeDirection(ref Vector3 direction)
    {
        var selectedDirection = SelectDirection();
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
            case var _ when selectedDirection == Vector3.Zero:
                direction = Vector3.Zero;
                break;
        }

        Position = new Vector3(MathF.Round(Position.X), Position.Y, MathF.Round(Position.Z));
    }

    /// <summary>
    /// Selects a direction for the enemy to move to.
    /// </summary>
    private Vector3 SelectDirection()
    {
        var randomDirections = GetRandomDirectionsArray(Directions);
        Vector3 selectedDirection = Vector3.Zero;

        foreach (var randomDirection in randomDirections)
        {
            if (!CanMoveToTile(randomDirection))
                continue;

            selectedDirection = randomDirection;
            break;
        }

        return selectedDirection;
    }

    /// <summary>
    /// Gets a randomly sorted directions array.
    /// </summary>
    /// <param name="directions">An array of directions to choose from.</param>
    /// <returns>A randomly sorted directions array.</returns>
    private static Vector3[] GetRandomDirectionsArray(Vector3[] directions)
    {
        return directions.OrderBy(_ => Guid.NewGuid()).ToArray();
    }

    /// <summary>
    /// Checks if the enemy can move to the specified tile.
    /// </summary>
    /// <param name="direction">The direction to check.</param>
    /// <returns>True if the enemy can move to the tile; otherwise, false.</returns>
    private bool CanMoveToTile(Vector3 direction)
    {
        Vector3I tilePosition = GetTilePositionBasedOnDirection(direction);
        var tileId = GameManager.GameMap.GetCellItem(tilePosition);

        return tileId == -1;
    }

    /// <summary>
    /// Gets the tile position based on the specified direction.
    /// </summary>
    /// <param name="direction">The direction to get the tile position from.</param>
    /// <returns>The tile position based on the specified direction.</returns>
    private Vector3I GetTilePositionBasedOnDirection(Vector3 direction)
    {
        var cellSize = GameManager.GameMap.CellSize.X;
        return direction switch
        {
            _ when direction == Vector3.Right
                => GameManager.GameMap.LocalToMap(
                    new Vector3(Position.X + cellSize, Position.Y, Position.Z)
                ),
            _ when direction == Vector3.Left
                => GameManager.GameMap.LocalToMap(
                    new Vector3(Position.X - cellSize, Position.Y, Position.Z)
                ),
            _ when direction == Vector3.Forward
                => GameManager.GameMap.LocalToMap(
                    new Vector3(Position.X, Position.Y, Position.Z - cellSize)
                ),
            _ when direction == Vector3.Back
                => GameManager.GameMap.LocalToMap(
                    new Vector3(Position.X, Position.Y, Position.Z + cellSize)
                ),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    /// <summary>
    /// Called when the area enters the enemy's area.
    /// </summary>
    /// <param name="body">The body that entered the area.</param>
    private void OnAreaEntered(Node3D body)
    {
        if (EnemyData.IsDead || !EnemyData.CanKillPlayer)
            return;

        if (body.IsInGroup("players"))
        {
            body.EmitSignal(Player.SignalName.Hit);
        }
    }
}
