using Bombino.game;
using Bombino.game.persistence.state_storage;
using Bombino.player;
using Godot;

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

    private const int Speed = 6;
    private bool _isDead;
    private Vector3 _targetVelocity = Vector3.Zero;
    private AnimationTree _animTree;
    private AnimationNodeStateMachinePlayback _stateMachine;

    private static readonly Vector3[] _directions =
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
    public override void _Ready()
    {
        Position = EnemyData.Position;

        _animTree = GetNode<AnimationTree>("AnimationTree");
        _animTree.Active = true;
        _stateMachine = (AnimationNodeStateMachinePlayback)_animTree.Get("parameters/playback");

        GD.Print($"Enemy created at: {Position}");

        var direction = Vector3.Zero;
        ChangeDirection(ref direction);

        GD.Print($"Enemy direction: {direction}");

        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;

        var targetPosition = Position - direction + new Vector3(0.01f, 0.01f, 0.01f);

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
        if (_isDead)
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

            var targetPosition = Position - direction + new Vector3(0.01f, 0.01f, 0.01f);
            LookAt(targetPosition, Vector3.Up);
        }
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
    private void OnHit()
    {
        _isDead = true;
        SetStateMachine("Die");
        Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ => QueueFree());
    }

    #endregion

    /// <summary>
    /// Changes the direction based on the selected direction.
    /// </summary>
    /// <param name="direction">The current direction.</param>
    private void ChangeDirection(ref Vector3 direction)
    {   
        GD.Print("\n");
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
        GD.Print($"Enemy changed direction, position: {Position}, direction: {direction}");
        Position = new Vector3(MathF.Round(Position.X), Position.Y, MathF.Round(Position.Z));
        GD.Print($"Enemy rounded position: {Position}");
    }

    /// <summary>
    /// Selects a direction for the enemy to move to.
    /// </summary>
    private Vector3 SelectDirection()
    {
        var randomDirections = GetRandomDirectionsArray(_directions);
        Vector3 selectedDirection = Vector3.Zero;

        foreach (var randomDirection in randomDirections)
        {   
            GD.Print($"Random direction: {randomDirection}");
            GD.Print($"Can move to tile: {CanMoveToTile(randomDirection)}");
            if (!CanMoveToTile(randomDirection)) continue;
            
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
        GD.Print($"Tile position: {tilePosition}");
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
        return direction switch
        {
            _ when direction == Vector3.Right
                => GameManager.GameMap.LocalToMap(
                    new Vector3(Position.X + 2, Position.Y + 1, Position.Z)
                ),
            _ when direction == Vector3.Left
                => GameManager.GameMap.LocalToMap(
                    new Vector3(Position.X - 2, Position.Y + 1, Position.Z)
                ),
            _ when direction == Vector3.Forward
                => GameManager.GameMap.LocalToMap(
                    new Vector3(Position.X, Position.Y + 1, Position.Z + 2)
                ),
            _ when direction == Vector3.Back
                => GameManager.GameMap.LocalToMap(
                    new Vector3(Position.X, Position.Y + 1, Position.Z - 2)
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
        if (_isDead)
            return;

        if (body.IsInGroup("players"))
        {
            body.EmitSignal(Player.SignalName.Hit);
            GD.Print($"Player hit by enemy at position: {Position} by {body.Name}");
        }
    }
}