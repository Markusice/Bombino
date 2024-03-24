namespace Bombino.scripts;

using Godot;
using Godot.Collections;

internal partial class Bomb : Area3D
{
    #region Exports

    [Export(PropertyHint.Range, "1,4")]
    private float _explodeTime = Mathf.Pi;

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
        var timer = GetNode<Timer>("BombTimer");

        timer.WaitTime = _explodeTime;
        timer.Start();
    }

    // for players / monsters
    private void OnBodyEntered(Node3D body)
    {
        if (!body.IsInGroup("players"))
            return;

        _bodiesToExplode.Add(body);
    }

    // for players / monsters
    private void OnBodyExited(Node3D body)
    {
        _bodiesToExplode.Remove(body);
    }

    // for bombs
    private void OnAreaEntered(Area3D area)
    {
        if (!area.IsInGroup("bombs"))
            return;

        _bombsInRange.Add(area as Bomb);
    }

    // for bombs
    private void OnAreaExited(Area3D area)
    {
        if (!area.IsInGroup("bombs"))
            return;

        _bombsInRange.Remove(area as Bomb);
    }

    private void OnTimerTimeout()
    {
        PlayExplodeAnimation();

        CreateExplosionsOnTilesOnXAndZAxis();

        ExplodeBodies();

        SendSignalToBombsInRange();

        QueueFree();
    }

    private void ExplodeBodies()
    {
        foreach (var body in _bodiesToExplode)
        {
            if (!body.IsInGroup("players"))
                return;

            var rayDirections = GetRayCastDirections();

            var playerShouldNotDie = CastRaysInDirectionsAndCheckIfPlayerShouldDie(
                body,
                rayDirections
            );

            if (playerShouldNotDie)
                continue;

            body.EmitSignal(Player.SignalName.Hit);
        }
    }

    private static Vector3[] GetRayCastDirections()
    {
        return new[] { Vector3.Left, Vector3.Right, Vector3.Back, Vector3.Forward, };
    }

    private bool CastRaysInDirectionsAndCheckIfPlayerShouldDie(Node3D body, Vector3[] rayDirections)
    {
        var playerShouldNotDie = false;

        foreach (var rayDirection in rayDirections)
        {
            if (playerShouldNotDie)
                break;

            var origin = GlobalPosition;
            var end = origin + rayDirection * (Range * 2);
            var rayMask = GameManager.GameMap.CollisionMask;

            var result = CastRayAndGetResult(origin, end, rayMask);

            if (result.Count == 0)
                continue;

            var colliderPosition = result["position"].AsVector3();

            playerShouldNotDie = rayDirection switch
            {
                _ when rayDirection == Vector3.Left
                    => (colliderPosition.X - body.Position.X) >= GameManager.GameMap.CellSize.X,
                _ when rayDirection == Vector3.Right
                    => (body.Position.X - colliderPosition.X) >= GameManager.GameMap.CellSize.X,
                _ when rayDirection == Vector3.Back
                    => (body.Position.Z - colliderPosition.Z) >= GameManager.GameMap.CellSize.Z,
                _ when rayDirection == Vector3.Forward
                    => (colliderPosition.Z - body.Position.Z) >= GameManager.GameMap.CellSize.Z,
                _ => false
            };
        }

        return playerShouldNotDie;
    }

    private Dictionary CastRayAndGetResult(Vector3 origin, Vector3 end, uint rayMask)
    {
        var spaceState = GetWorld3D().DirectSpaceState;

        var query = PhysicsRayQueryParameters3D.Create(
            origin,
            end,
            rayMask,
            new Array<Rid> { GetRid() }
        );

        var result = spaceState.IntersectRay(query);

        return result;
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
        foreach (var bomb in _bombsInRange)
        {
            var distanceBetweenTwoBombs = (Position - bomb.Position).Length();
            var bombTimeoutTime = distanceBetweenTwoBombs / ExplosionDistanceDivider;

            GD.Print($"sent signal to bomb: {bomb} with timeout time: {bombTimeoutTime}");

            bomb.EmitSignal(SignalName.ExplodeSooner, bombTimeoutTime);
        }
    }

    private void PlayExplodeAnimation()
    {
        var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        animationPlayer.Play("explode");
    }

    private void CreateExplosionsOnTilesOnXAndZAxis()
    {
        CreateExplosionsOnTilesOnAxis(ExplosionAxis.X);
        CreateExplosionsOnTilesOnAxis(ExplosionAxis.Z);
    }

    private void CreateExplosionsOnTilesOnAxis(ExplosionAxis explosionAxis)
    {
        for (var axis = -1; axis < 1; axis++)
        {
            for (var nthTile = 1; nthTile <= Range; nthTile++)
            {
                var effectPosition = GetEffectPositionOnAxis(explosionAxis, axis, nthTile);

                if (!CanCreateExplosionAtPosition(effectPosition))
                    break;

                CreateExplosionAtPosition(effectPosition);
            }
        }
    }

    private Vector3 GetEffectPositionOnAxis(ExplosionAxis explosionAxis, int axis, int nthTile)
    {
        if (explosionAxis == ExplosionAxis.X)
            return new Vector3(
                axis < 0
                    ? Position.X - (nthTile * GameManager.GameMap.CellSize.X)
                    : Position.X + (nthTile * GameManager.GameMap.CellSize.X),
                Position.Y,
                Position.Z
            );

        return new Vector3(
            Position.X,
            Position.Y,
            axis < 0
                ? Position.Z - (nthTile * GameManager.GameMap.CellSize.Z)
                : Position.Z + (nthTile * GameManager.GameMap.CellSize.Z)
        );
    }

    private static bool CanCreateExplosionAtPosition(Vector3 position)
    {
        var mapCoordinates = GameManager.GameMap.LocalToMap(position);
        var tileId = GameManager.GameMap.GetCellItem(mapCoordinates);

        return tileId == -1;
    }

    private void CreateExplosionAtPosition(Vector3 position)
    {
        var effectInstance = _effect.Instantiate<VFX_Explosion>();
        effectInstance.Position = position;

        var animationPlayer = effectInstance.GetNode<AnimationPlayer>("AnimationPlayer");

        GameManager.GameMap.AddChild(effectInstance);

        animationPlayer.Play("explosionEffect");
    }
}
