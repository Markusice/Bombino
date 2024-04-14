using System;
using System.Linq;
using Bombino.bomb;
using Bombino.events;
using Bombino.game;
using Bombino.game.persistence.state_storage;
using Godot;

namespace Bombino.player;

/// <summary>
/// Represents the player character in the game.
/// </summary>
internal partial class Player : CharacterBody3D
{
    #region Exports

    [Export] public int Speed { get; set; } = 10;

    [Export] private PackedScene _bombScene;

    #endregion

    #region Signals

    /// <summary>
    /// Signal emitted when the player is hit.
    /// </summary>
    [Signal]
    public delegate void HitEventHandler();

    #endregion

    #region Fields

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    private Vector3 _targetVelocity = Vector3.Zero;
    private AnimationTree _animTree;
    private AnimationNodeStateMachinePlayback _stateMachine;
    private Vector3I _mapPosition;

    public PlayerData PlayerData { get; set; }

    #endregion

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        Position = PlayerData.Position;

        _animTree = GetNode<AnimationTree>("AnimationTree");
        _animTree.Active = true;
        _stateMachine = (AnimationNodeStateMachinePlayback)_animTree.Get("parameters/playback");
    }

    /// <summary>
    /// Called every frame. 'delta' is the elapsed time since the previous frame.
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
    {
        // We create a local variable to store the input direction.
        var direction = Vector3.Zero;

        CheckActionKeysForInput(ref direction);

        if (direction != Vector3.Zero)
        {
            var targetPosition = Position - direction;
            LookAt(targetPosition, Vector3.Up);

            direction = direction.Normalized();

            var absoluteDirection = direction.Abs();

            // Player can't move diagonally
            if (!absoluteDirection.Z.Equals(1) && !absoluteDirection.Z.Equals(0))
                return;
        }

        // Ground velocity
        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;

        // Vertical velocity
        if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
            _targetVelocity.Y -= _gravity * (float)delta;

        BlendMovementAnimation();

        // Moving the character
        Velocity = _targetVelocity;
        MoveAndSlide();

        SetMapPosition();
    }

    #region MethodsForSignals

    /// <summary>
    /// Called when the player enters the area.
    /// </summary>
    private void OnHit()
    {
        SetStateMachine("Die");
        Die();
    }

    #endregion

    /// <summary>
    /// Kills the player.
    /// </summary>
    private void Die()
    {
        Events.Instance.EmitSignal(Events.SignalName.PlayerDied, PlayerData.Color.ToString());

        QueueFree();
    }

    /// <summary>
    /// Called when the player enters the area.
    /// </summary>
    private void BlendMovementAnimation()
    {
        var animTree = GetNode<AnimationTree>("AnimationTree");
        animTree.Set("parameters/IR/blend_position", new Vector2(Velocity.X, Velocity.Z).Length());
    }

    /// <summary>
    /// Checks the action keys for input.
    /// </summary>
    /// <param name="direction"></param>
    private void CheckActionKeysForInput(ref Vector3 direction)
    {
        ModifyDirectionOnMovement(ref direction);
        PlaceBombOnInput();
    }

    /// <summary>
    /// Modifies the direction based on the movement keys.
    /// </summary>
    /// <param name="direction"></param>
    private void ModifyDirectionOnMovement(ref Vector3 direction)
    {
        foreach (var movementKey in PlayerData.ActionKeys[..4])
            if (Input.IsActionPressed(movementKey))
            {
                var actionKeyDirection = movementKey[5];

                switch (actionKeyDirection)
                {
                    case 'r':
                        direction.X += 1.0f;
                        break;
                    case 'l':
                        direction.X -= 1.0f;
                        break;
                    case 'b':
                        direction.Z += 1.0f;
                        break;
                    case 'f':
                        direction.Z -= 1.0f;
                        break;
                }
            }
    }

    /// <summary>
    /// Places a bomb on input.
    /// </summary>
    private void PlaceBombOnInput()
    {
        if (Input.IsActionJustPressed(PlayerData.ActionKeys[4]))
            OnPlaceBomb();
    }

    /// <summary>
    /// Sets the state machine.
    /// </summary>
    /// <param name="stateName"></param>
    private void SetStateMachine(string stateName)
    {
        _stateMachine = (AnimationNodeStateMachinePlayback)_animTree.Get("parameters/playback");
        _stateMachine.Travel(stateName);
    }

    /// <summary>
    /// Called when the player enters the area.
    /// </summary>
    private void OnPlaceBomb()
    {
        var collisionObject = this as CollisionObject3D;
        collisionObject.SetCollisionMaskValue(5, false);

        var bombTilePosition = GameManager.GameMap.MapToLocal(_mapPosition);
        var bombToPlacePosition = new Vector3(
            bombTilePosition.X,
            GameManager.GameMap.CellSize.Y + 1,
            bombTilePosition.Z
        );

        if (IsUnableToPlaceBomb(bombToPlacePosition))
            return;
        SetStateMachine("Place");

        var bombToPlace = CreateBomb(bombToPlacePosition);
        GameManager.WorldEnvironment.AddChild(bombToPlace);
    }

    /// <summary>
    /// Checks if the player is unable to place a bomb.
    /// </summary>
    /// <param name="bombToPlacePosition"></param>
    /// <returns></returns>
    private bool IsUnableToPlaceBomb(Vector3 bombToPlacePosition)
    {
        var placedBombs = GetTree().GetNodesInGroup("bombs");

        return placedBombs
            .Cast<Area3D>()
            .Any(bombArea3D => bombArea3D.Position == bombToPlacePosition);
    }

    /// <summary>
    /// Creates a bomb.
    /// </summary>
    /// <param name="bombToPlacePosition"></param>
    /// <returns></returns>
    private Bomb CreateBomb(Vector3 bombToPlacePosition)
    {
        var bombToPlace = _bombScene.Instantiate<Bomb>();

        bombToPlace.Position = bombToPlacePosition;
        bombToPlace.Range = PlayerData.BombRange;

        return bombToPlace;
    }

    /// <summary>
    /// Sets the map position.
    /// </summary>
    private void SetMapPosition()
    {
        _mapPosition = GameManager.GameMap.LocalToMap(Position);
    }
}