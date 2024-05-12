using Bombino.events;
using Bombino.game;
using Bombino.game.persistence.state_storage;
using Bombino.player.input_actions;
using Godot;

namespace Bombino.player;

/// <summary>
/// Represents the player character in the game.
/// </summary>
internal partial class Player : CharacterBody3D
{
    #region Exports

    [Export] public int Speed { get; set; } = 10;

    [Export] public PackedScene BombScene { get; set; }

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
    public Vector3I MapPosition;
    public PlayerInputActions PlayerInputActions { get; } = new();
    public PlayerData PlayerData { get; set; }

    #endregion

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        Position = PlayerData.Position;
        Name = $"Player{PlayerData.Color.ToString()}";

        _animTree = GetNode<AnimationTree>("AnimationTree");
        _animTree.Active = true;
        _stateMachine = (AnimationNodeStateMachinePlayback)_animTree.Get("parameters/playback");
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    /// <param name="delta"> The time passed since the last frame. </param>
    public override void _PhysicsProcess(double delta)
    {
        if (PlayerData.IsDead) return;

        // We create a local variable to store the input direction.
        var direction = Vector3.Zero;

        // We check for each move input and update the direction accordingly.
        CheckActionKeysForInput(ref direction);

        // GD.Print($"Player position: {Position}");

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

        PlayerData.Position = Position;

        SetMapPosition();
    }

    #region MethodsForSignals

    /// <summary>
    /// Called when the player is hit.
    /// </summary>
    public async void OnHit()
    {
        PlayerData.IsDead = true;
        SetStateMachine("Die");
        Events.Instance.EmitSignal(Events.SignalName.PlayerDied, PlayerData.Color.ToString());

        await ToSignal(GetTree().CreateTimer(Mathf.Pi), SceneTreeTimer.SignalName.Timeout);
        Die();
    }

    #endregion

    /// <summary>
    /// Kills the player.
    /// </summary>
    private void Die()
    {
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
    /// <param name="direction"> The direction to be modified. </param>
    private void CheckActionKeysForInput(ref Vector3 direction)
    {
        ModifyDirectionOnMovement(ref direction);
        PlaceBombOnInput();
    }

    /// <summary>
    /// Modifies the direction based on the movement keys.
    /// </summary>
    /// <param name="direction"> The direction to be modified. </param>
    private void ModifyDirectionOnMovement(ref Vector3 direction)
    {
        direction = PlayerInputActions.Movements.Where(movement =>
                Input.IsActionPressed($"{movement.Name}_{PlayerData.Color.ToString().ToLower()}"))
            .Aggregate(direction, (current, movement) => movement.Action.Invoke(current));
    }

    /// <summary>
    /// Places a bomb on input.
    /// </summary>
    private void PlaceBombOnInput()
    {
        if (!Input.IsActionJustPressed
                ($"{PlayerInputActions.BombPlace.Name}_{PlayerData.Color.ToString().ToLower()}"))
            return;

        PlayerInputActions.BombPlace.Action.Invoke(this);
    }

    /// <summary>
    /// Sets the state machine.
    /// </summary>
    /// <param name="stateName"> The name of the state. </param>
    private void SetStateMachine(string stateName)
    {
        _stateMachine = (AnimationNodeStateMachinePlayback)_animTree.Get("parameters/playback");
        _stateMachine.Travel(stateName);
    }

    /// <summary>
    /// Sets the map position.
    /// </summary>
    private void SetMapPosition()
    {
        MapPosition = GameManager.GameMap.LocalToMap(Position);
    }
}