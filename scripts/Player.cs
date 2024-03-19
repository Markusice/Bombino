namespace Bombino.scripts;

using persistence;
using System.Linq;
using Godot;

internal partial class Player : CharacterBody3D
{
    #region Signals

    [Signal]
    public delegate void HitEventHandler();

    #endregion

    #region Exports

    [Export]
    public int Speed { get; set; } = 20;

    [Export]
    public int FallAcceleration { get; set; } = 75;

    [Export]
    private PackedScene BombScene { get; set; }

    #endregion

    private Vector3 _targetVelocity = Vector3.Zero;

    private Vector3I _mapPosition;

    public PlayerData PlayerData { get; set; }

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

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
            if (!absoluteDirection.Z.Equals(1) && !absoluteDirection.Z.Equals(0)) return;
        }

        // Ground velocity
        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;

        // Vertical velocity
        if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
        {
            _targetVelocity.Y -= FallAcceleration * (float)delta;
        }

        var animTree = GetNode<AnimationTree>("AnimationTree");
        var stateMachine = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/playback");

        animTree.Set("parameters/IR/blend_position", _targetVelocity.Length());

        // Moving the character
        Velocity = _targetVelocity;
        MoveAndSlide();

        SetMapPosition();
    }

    private void CheckActionKeysForInput(ref Vector3 direction)
    {
        ModifyDirectionOnMovement(ref direction);
        PlaceBombOnInput();
    }

    private void ModifyDirectionOnMovement(ref Vector3 direction)
    {
        foreach (var movementKey in PlayerData.ActionKeys[..4])
        {
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
    }

    private void PlaceBombOnInput()
    {
        if (Input.IsActionJustPressed(PlayerData.ActionKeys[4])) OnPlaceBomb();
    }

    private void OnPlaceBomb()
    {
        var bombTilePosition = GameManager.GridMap.MapToLocal(_mapPosition);
        var bombToPlacePosition =
            new Vector3(bombTilePosition.X, GameManager.GridMap.CellSize.Y + 1, bombTilePosition.Z);

        if (IsUnableToPlaceBomb(bombToPlacePosition)) return;

        var bombToPlace = CreateBomb(bombToPlacePosition);
        GameManager.WorldEnvironment.AddChild(bombToPlace);
    }

    private bool IsUnableToPlaceBomb(Vector3 bombToPlacePosition)
    {
        var placedBombs = GetTree().GetNodesInGroup("bombs");

        return placedBombs.Cast<Area3D>().Any(bombArea3D => bombArea3D.Position == bombToPlacePosition);
    }

    private Bomb CreateBomb(Vector3 bombToPlacePosition)
    {
        var bombToPlace = BombScene.Instantiate<Bomb>();

        bombToPlace.Position = bombToPlacePosition;
        bombToPlace.Range = PlayerData.BombRange;

        return bombToPlace;
    }

    private void SetMapPosition()
    {
        _mapPosition = GameManager.GridMap.LocalToMap(Position);
    }

    private void OnHit()
    {
        Die();
    }

    private void Die()
    {
        GD.Print($"Player die : {Name}");
        QueueFree();
    }
}