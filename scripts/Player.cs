using System.Linq;
using Godot;

public partial class Player : CharacterBody3D
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

    private Vector3I _positionOnMap;

    public int PlayerBombRange { get; private set; } = 2;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _PhysicsProcess(double delta)
    {
        // We create a local variable to store the input direction.
        var direction = Vector3.Zero;

        #region PlayerRed

        // We check for each move input and update the direction accordingly.
        if (Input.IsActionPressed("move_right") && Name == PlayerColor.Red.ToString())
        {
            direction.X += 1.0f;
        }

        if (Input.IsActionPressed("move_left") && Name == PlayerColor.Red.ToString())
        {
            direction.X -= 1.0f;
        }

        if (Input.IsActionPressed("move_back") && Name == PlayerColor.Red.ToString())
        {
            // Notice how we are working with the vector's X and Z axes.
            // In 3D, the XZ plane is the ground plane.
            direction.Z += 1.0f;
        }

        if (Input.IsActionPressed("move_forward") && Name == PlayerColor.Red.ToString())
        {
            direction.Z -= 1.0f;
        }

        if (Input.IsActionJustPressed("place_bomb") && Name == PlayerColor.Red.ToString()) OnPlaceBomb();

        #endregion

        #region PlayerBlue

        if (Input.IsActionPressed("move_right_p2") && Name == PlayerColor.Blue.ToString())
        {
            direction.X += 1.0f;
        }

        if (Input.IsActionPressed("move_left_p2") && Name == PlayerColor.Blue.ToString())
        {
            direction.X -= 1.0f;
        }

        if (Input.IsActionPressed("move_back_p2") && Name == PlayerColor.Blue.ToString())
        {
            // Notice how we are working with the vector's X and Z axes.
            // In 3D, the XZ plane is the ground plane.
            direction.Z += 1.0f;
        }

        if (Input.IsActionPressed("move_forward_p2") && Name == PlayerColor.Blue.ToString())
        {
            direction.Z -= 1.0f;
        }

        if (Input.IsActionJustPressed("place_bomb_p2") && Name == PlayerColor.Blue.ToString()) OnPlaceBomb();

        #endregion

        if (direction != Vector3.Zero)
        {
            Vector3 targetPosition = Position - direction;
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

        SetPositionOnMap();
    }

    private void Die()
    {
        GD.Print($"Player die : {Name}");
        QueueFree();
    }

    private void OnHit()
    {
        Die();
    }

    private void OnPlaceBomb()
    {
        var bombTilePosition = GameManager.GridMap.MapToLocal(_positionOnMap);
        var bombToPlacePosition = new Vector3(bombTilePosition.X, GameManager.GridMap.CellSize.Y + 1, bombTilePosition.Z);

        if (CannotPlaceBomb(bombToPlacePosition))
        {
            GD.Print("Can't place bomb here");
            return;
        }

        var bombToPlace = CreateBomb(bombToPlacePosition);
        GameManager.WorldEnvironment.AddChild(bombToPlace);
    }

    private bool CannotPlaceBomb(Vector3 bombToPlacePosition)
    {
        var placedBombs = GetTree().GetNodesInGroup("bombs");

        return placedBombs.Cast<Area3D>().Any(bombArea3D => bombArea3D.Position == bombToPlacePosition);
    }

    private Bomb CreateBomb(Vector3 bombToPlacePosition)
    {
        var bombToPlace = BombScene.Instantiate<Bomb>();
        bombToPlace.Position = bombToPlacePosition;
        bombToPlace.Range = PlayerBombRange;

        return bombToPlace;
    }

    private void SetPositionOnMap()
    {
        _positionOnMap = GameManager.GridMap.LocalToMap(Position);

        // GD.Print($"localToMap: {_positionOnMap}");
        // GD.Print($"mapToLocal: {GameManager.GridMap.MapToLocal(_positionOnMap)}");
        // GD.Print($"real position: {Position}");
    }
}