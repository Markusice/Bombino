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

	private AnimationTree _animTree;
	private AnimationNodeStateMachinePlayback _stateMachine;
	private Vector3 _targetVelocity = Vector3.Zero;

	private Vector3I _positionOnMap;

	public int PlayerBombRange { get; private set; } = 2;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		base._Ready();
		_animTree = GetNode<AnimationTree>("AnimationTree");
		_animTree.Active = true;
		_stateMachine = (AnimationNodeStateMachinePlayback)_animTree.Get("parameters/playback");
	}

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

		if (Input.IsActionPressed("move_right2") && Name == PlayerColor.Blue.ToString())
		{
			direction.X += 1.0f;
		}

		if (Input.IsActionPressed("move_left2") && Name == PlayerColor.Blue.ToString())
		{
			direction.X -= 1.0f;
		}

		if (Input.IsActionPressed("move_back2") && Name == PlayerColor.Blue.ToString())
		{
			// Notice how we are working with the vector's X and Z axes.
			// In 3D, the XZ plane is the ground plane.
			direction.Z += 1.0f;
		}

		if (Input.IsActionPressed("move_forward2") && Name == PlayerColor.Blue.ToString())
		{
			direction.Z -= 1.0f;
		}

		if (Input.IsActionJustPressed("place_bomb2") && Name == PlayerColor.Blue.ToString()) OnPlaceBomb();

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

		// Snap the target velocity to the grid cell size
		var cellSize = GameManager.GridMap.CellSize;
		_targetVelocity.X = Mathf.Round(_targetVelocity.X / cellSize.X) * cellSize.X;
		_targetVelocity.Z = Mathf.Round(_targetVelocity.Z / cellSize.Z) * cellSize.Z;


		// Vertical velocity
		if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}

		_animTree.Set("parameters/IR/blend_position", new Vector2(direction.X, direction.Z).Length());

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
		_stateMachine.Travel("Die");
		Die();
	}
	private void OnPlaceBomb()
	{
		var bombToPlacePosition = GameManager.GridMap.MapToLocal(_positionOnMap);

		if (CannotPlaceBomb(bombToPlacePosition))
		{
			GD.Print("Can't place bomb here");
			return;
		}

		_stateMachine.Travel("Place");
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
		bombToPlace.Position = new Vector3(bombToPlacePosition.X, 3f, bombToPlacePosition.Z);
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
