namespace Bombino.scripts;

using Godot;
using Godot.Collections;

internal partial class Bomb : Area3D
{
    #region Exports

    [Export]
    private PackedScene _effect;

    #endregion

    #region Signals

    [Signal]
    public delegate void ExplodeSoonerEventHandler(float newTimerWaitTime);

    #endregion

    private readonly Array<Node3D> _bodiesToExplode = new();
    private readonly Array<Bomb> _bombsInRange = new();

    private const float ExplosionDistanceDivider = 5.0f;

    public int Range { get; set; }

    public override void _Ready()
    {
        GD.Print($"bomb position : {Position}");
        GD.Print($"tile: {GameManager.GridMap.LocalToMap(Position)}");

        var bombTimer = GetNode<Timer>("BombTimer");

        bombTimer.WaitTime = Mathf.Pi;
        bombTimer.Start();
    }

    private void OnBodyEntered(Node3D body)
    {
        GD.Print($"body entered: {body}");

        if (!body.IsInGroup("players"))
            return;

        _bodiesToExplode.Add(body);
    }

    // for bombs
    private void OnAreaEntered(Area3D area)
    {
        if (!area.IsInGroup("bombs"))
            return;

        GD.Print($"bomb entered: {area.Position}");
        _bombsInRange.Add(area as Bomb);
    }

    private void OnAreaExited(Area3D area)
    {
        if (!area.IsInGroup("bombs"))
            return;

        _bombsInRange.Remove(area as Bomb);
    }

    // for players / monsters
    private void OnBodyExited(Node3D body)
    {
        _bodiesToExplode.Remove(body);

        GD.Print($"body exited: {body}");
    }

    private void OnTimerTimeout()
    {
        GD.Print("Bomb exploded");

        PlayExplosion();

        ExplodeBodies();

        SendSignalToBombsInRange();

        // remove bomb from scene
        QueueFree();
    }

    private void ExplodeBodies()
    {
        GD.Print($"bodies to explode: {_bodiesToExplode}");

        foreach (var body in _bodiesToExplode)
        {
            if (!body.IsInGroup("players"))
                return;

            GD.Print("start casting rays");
            var spaceState = GetWorld3D().DirectSpaceState;

            var playerShouldNotDie = false;

            var rayDirections = new[]
            {
                Vector3.Left,
                Vector3.Right,
                Vector3.Back,
                Vector3.Forward,
            };

            foreach (var rayDirection in rayDirections)
            {
                var origin = GlobalPosition;
                var end = origin + rayDirection * (Range * 2);

                // only collide with the grid map
                var rayMask = GameManager.GridMap.CollisionMask;

                var query = PhysicsRayQueryParameters3D.Create(
                    origin,
                    end,
                    rayMask,
                    new Array<Rid> { GetRid() }
                );

                var result = spaceState.IntersectRay(query);
                if (result.Count == 0)
                    continue;

                var collider = result["collider"].AsGodotObject();
                var colliderPosition = result["position"].AsVector3();
                GD.Print($"collider: {collider} at position: {colliderPosition}");
                GD.Print($"player: {body} at position: {body.GlobalPosition}");

                var vectorFromBombToPlayer = GlobalPosition.DirectionTo(body.GlobalPosition);

                var playerDirectionDotProduct = Mathf.RoundToInt(
                    vectorFromBombToPlayer.Dot(rayDirection)
                );
                GD.Print($"player dot product at {rayDirection}: {playerDirectionDotProduct}");

                if (playerDirectionDotProduct != 1)
                    continue;

                if (rayDirection == Vector3.Left)
                {
                    playerShouldNotDie =
                        (colliderPosition.X - body.Position.X) >= GameManager.GridMap.CellSize.X;
                }
                else if (rayDirection == Vector3.Right)
                {
                    playerShouldNotDie =
                        (body.Position.X - colliderPosition.X) >= GameManager.GridMap.CellSize.X;
                }
                else if (rayDirection == Vector3.Back)
                {
                    playerShouldNotDie =
                        (body.Position.Z - colliderPosition.Z) >= GameManager.GridMap.CellSize.Z;
                }
                else
                {
                    playerShouldNotDie =
                        (colliderPosition.Z - body.Position.Z) >= GameManager.GridMap.CellSize.Z;
                }
            }

            GD.Print($"player should not die: {playerShouldNotDie}");
            if (playerShouldNotDie)
                continue;

            body.EmitSignal(Player.SignalName.Hit);
        }
    }

    private void OnExplodeSooner(float newTimerWaitTime)
    {
        var bombTimer = GetNode<Timer>("BombTimer");

        if (bombTimer.TimeLeft <= newTimerWaitTime)
            return;

        bombTimer.Stop();

        bombTimer.WaitTime = newTimerWaitTime;

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
        var effectInstance = _effect.Instantiate<VFX_Explosion>();
        effectInstance.Position = Position;

        var effectInstances = effectInstance.GetChildren();

        using var effectInstanceEnumerator = effectInstances.GetEnumerator();
        while (effectInstanceEnumerator.MoveNext())
        {
            var child = effectInstanceEnumerator.Current;
            if (child is GpuParticles3D gpuParticle3D)
                gpuParticle3D.Emitting = true;
        }

        GameManager.GridMap.AddChild(effectInstance);
    }
}
