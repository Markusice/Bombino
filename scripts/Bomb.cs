using Godot;
using Godot.Collections;

public partial class Bomb : Area3D
{
	#region Signals

	[Signal]
	public delegate void ExplodeSoonerEventHandler(float bombTimerWaitTime);

	#endregion

	#region Exports

	[Export]
	private PackedScene _effect;

	#endregion

	public int Range { get; set; }

	private readonly Array<Node3D> _bodiesToExplode = new();
	private readonly Array<Bomb> _bombsInRange = new();

	private const float ExplosionDistanceDivider = 5.0f;

	public override void _Ready()
	{
		GD.Print($"bomb posX : {Position.X} posZ : {Position.Z}");
		GD.Print($"tile: {GameManager.GridMap.LocalToMap(Position)}");

		GetNode<Timer>("BombTimer").Start();
	}

	private void OnBodyEntered(Node3D body)
	{
		GD.Print($"body entered: {body}");

		if (!body.IsInGroup("players")) return;

		GD.Print("start casting rays");
		var spaceState = GetWorld3D().DirectSpaceState;

		var directions = new[]
		{
			Vector3.Left,
			Vector3.Right,
			Vector3.Back,
			Vector3.Forward,
		};

		foreach (var direction in directions)
		{
			var origin = GlobalPosition;
			var end = origin + direction * (Range * 2);
			var rayMask = GameManager.GridMap.CollisionMask;

			// only collide with the grid map
			var query = PhysicsRayQueryParameters3D.Create(origin, end, rayMask,
				new Array<Rid> { GetRid() });

			var result = spaceState.IntersectRay(query);
			if (result.Count == 0) continue;

			var collider = result["collider"];
			GD.Print(collider);
		}

		//TODO: if the player is standing behind a wall, don't explode the player (remove from the _bodiesToExplode array)

		// get the direction where the player is standing
		GD.Print(Position.DirectionTo(body.Position).Round());

		_bodiesToExplode.Add(body);
	}

	// for bombs
	private void OnAreaEntered(Area3D area)
	{
		if (area.IsInGroup("bombs"))
		{
			_bombsInRange.Add(area as Bomb);
		}
	}

	private void OnAreaExited(Area3D area)
	{
		if (area.IsInGroup("bombs"))
		{
			_bombsInRange.Remove(area as Bomb);
		}
	}

	// for players / monsters
	private void OnBodyExited(Node3D body)
	{
		_bodiesToExplode.Remove(body);
		GD.Print($"body exited {body}");
	}

	private void OnTimerTimeout()
	{
		GD.Print("Bomb exploded");

		//TODO: add bomb disappearing & explosion effect

		PlayExplosion();

		ExplodeBodies();

		SendSignalToBombsInRange();

		// remove bomb from scene
		QueueFree();
	}

	private void ExplodeBodies()
	{
		GD.Print($"bodies to explode: {_bodiesToExplode}");

		foreach (var item in _bodiesToExplode)
		{
			if (item.IsInGroup("players")) item.EmitSignal(Player.SignalName.Hit);
		}
	}

	private void OnExplodeSooner(float bombTimerWaitTime)
	{
		var bombTimer = GetNode<Timer>("BombTimer");
		bombTimer.Stop();

		bombTimer.WaitTime = bombTimerWaitTime;

		bombTimer.Start();
	}

	private void SendSignalToBombsInRange()
	{
		GD.Print($"bombs in range: {_bombsInRange}");

		foreach (var bomb in _bombsInRange)
		{
			var distanceBetweenTwoBombs = (Position - bomb.Position).Length();
			var bombTimeoutTime = distanceBetweenTwoBombs / ExplosionDistanceDivider;

			GD.Print($"sent signal to bomb: {bomb} with timeout time: {bombTimeoutTime}");

			bomb.EmitSignal(SignalName.ExplodeSooner, bombTimeoutTime);
		}
	}

	private void PlayExplosion()
	{
		var effectInstance = _effect.Instantiate<VFX_Explosion2>();
		effectInstance.Position = Position;

		var sparks = effectInstance.GetNode<GpuParticles3D>("Sparks");
		sparks.Emitting = true;

		GameManager.GridMap.AddChild(effectInstance);
	}

	private void CastRay()
	{
		GD.Print("start casting rays");
		var spaceState = GetWorld3D().DirectSpaceState;

		var directions = new[]
		{
			Vector3.Left,
			Vector3.Right,
			Vector3.Back,
			Vector3.Forward,
		};

		foreach (var direction in directions)
		{
			var origin = GlobalPosition;
			var end = origin + direction * (Range * 2);

			// only collide with the grid map
			var rayMask = GameManager.GridMap.CollisionMask;

			var query = PhysicsRayQueryParameters3D.Create(origin, end, rayMask,
				new Array<Rid> { GetRid() });

			var result = spaceState.IntersectRay(query);
			if (result.Count == 0) continue;

			var collider = result["collider"].AsGodotObject();
			GD.Print($"collider: {collider}");
		}
	}
}
