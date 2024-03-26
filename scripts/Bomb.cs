namespace Bombino.scripts;

using Godot;
using Godot.Collections;

/// <summary>
/// Represents a bomb in the game.
/// </summary>
internal partial class Bomb : Area3D
{
    #region Exports

    [Export(PropertyHint.Range, "1,4")]
    private float _explodeTime = Mathf.Pi;

    [Export]
    private PackedScene _effect;

    #endregion

    #region Signals

    /// <summary>
    /// Signal emitted when the bomb explodes sooner.
    /// </summary>
    /// <param name="newTimerWaitTime"></param>
    [Signal]
    public delegate void ExplodeSoonerEventHandler(float newTimerWaitTime);

    #endregion

    #region Fields

    private readonly Array<Node3D> _bodiesToExplode = new();
    private readonly Array<Area3D> _bombsInRange = new();

    private const float ExplosionDistanceDivider = Mathf.E;

    private Area3D _bombObject;

    public int Range { get; set; }

    #endregion

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _bombObject = GetNode<Area3D>("Bomb/BombObject");

        var timer = GetNode<Timer>("BombTimer");
        timer.WaitTime = _explodeTime;

        timer.Start();
    }

    #region MethodsForSignals

    /// <summary>
    /// Called when the bomb explodes sooner.
    /// </summary>
    /// <param name="newTimerWaitTime"></param>
    private void OnExplodeSooner(float newTimerWaitTime)
    {
        var bombTimer = GetNode<Timer>("BombTimer");

        if (bombTimer.TimeLeft <= newTimerWaitTime)
            return;

        bombTimer.Stop();

        bombTimer.WaitTime = newTimerWaitTime;

        bombTimer.Start();
    }

    // for players / monsters
    /// <summary>
    /// Called when a body enters the area.
    /// </summary>
    /// <param name="body"></param>
    private void OnBodyEntered(Node3D body)
    {
        if (!body.IsInGroup("players") && !body.IsInGroup("enemies"))
            return;

        _bodiesToExplode.Add(body);
    }

    // for players / monsters
    /// <summary>
    /// Called when a body exits the area.
    /// </summary>
    /// <param name="body"></param>
    private void OnBodyExited(Node3D body)
    {
        if (!body.IsInGroup("players") && !body.IsInGroup("enemies"))
            return;

        _bodiesToExplode.Remove(body);
    }

    // for bombs
    /// <summary>
    /// Called when an area enters the area.
    /// </summary>
    /// <param name="area"></param>
    private void OnAreaEntered(Area3D area)
    {
        if (!area.IsInGroup("bombobjects"))
            return;

        if (_bombObject.GetRid() == area.GetRid())
            return;

        _bombsInRange.Add(GetBombFromBombObject(area));
    }

    private static Area3D GetBombFromBombObject(Area3D area)
    {
        return area.GetParent<MeshInstance3D>().GetParent<Area3D>();
    }

    // for bombs
    /// <summary>
    /// Called when an area exits the area.
    /// </summary>
    /// <param name="area"></param>
    private void OnAreaExited(Area3D area)
    {
        if (!area.IsInGroup("bombobjects"))
            return;

        if (_bombObject.GetRid() == area.GetRid())
            return;

        _bombsInRange.Remove(GetBombFromBombObject(area));
    }

    /// <summary>
    /// Called when the timer times out.
    /// </summary>
    private void OnTimerTimeout()
    {
        PlayExplodeAnimation();

        CreateExplosionsOnTilesOnXAndZAxis();

        ExplodeBodies();

        SendSignalToBombsInRange();

        QueueFree();
    }

    #endregion

    /// <summary>
    /// Explodes the bodies in the area.
    /// </summary>
    private void ExplodeBodies()
    {
        foreach (var body in _bodiesToExplode)
        {
            if (!body.IsInGroup("players") && !body.IsInGroup("enemies"))
                return;

            var rayDirections = GetRayCastDirections();

            var bodyShouldNotDie = CastRaysInDirectionsAndCheckIfPlayerShouldDie(
                body,
                rayDirections
            );

            if (bodyShouldNotDie)
                continue;

            body.EmitSignal(
                body.IsInGroup("players") ? Player.SignalName.Hit : Enemy.SignalName.Hit
            );
        }
    }

    /// <summary>
    /// Gets the raycast directions.
    /// </summary>
    /// <returns></returns>
    private static Vector3[] GetRayCastDirections()
    {
        return new[] { Vector3.Left, Vector3.Right, Vector3.Back, Vector3.Forward, };
    }

    /// <summary>
    /// Casts rays in the specified directions and checks if the player should die.
    /// </summary>
    /// <param name="body"></param>
    /// <param name="rayDirections"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Casts a ray and gets the result.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="end"></param>
    /// <param name="rayMask"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Sends a signal to the bombs in range.
    /// </summary>
    private void SendSignalToBombsInRange()
    {
        foreach (var bomb in _bombsInRange)
        {
            var distanceBetweenTwoBombs = (Position - bomb.Position).Length();
            var bombTimeoutTime = distanceBetweenTwoBombs / ExplosionDistanceDivider;

            GD.Print(
                $"sent signal to bomb: {bomb} timeout time: {bombTimeoutTime} (distance: {distanceBetweenTwoBombs})"
            );

            bomb.EmitSignal(SignalName.ExplodeSooner, bombTimeoutTime);
        }
    }

    /// <summary>
    /// Plays the explode animation.
    /// </summary>
    private void PlayExplodeAnimation()
    {
        var bombMeshInstance3D = GetNode<MeshInstance3D>("Bomb");
        bombMeshInstance3D.Hide();

        var effectInstance = _effect.Instantiate<VfxExplosion>();
        effectInstance.Position = Position;

        var effectAnimationPlayer = effectInstance.GetNode<AnimationPlayer>("AnimationPlayer");

        GameManager.GameMap.AddChild(effectInstance);

        effectAnimationPlayer.Play("explosionEffect");
    }

    /// <summary>
    /// Creates explosions on tiles on the X and Z axis.
    /// </summary>
    private void CreateExplosionsOnTilesOnXAndZAxis()
    {
        CreateExplosionsOnTilesOnAxis(ExplosionAxis.X);
        CreateExplosionsOnTilesOnAxis(ExplosionAxis.Z);
    }

    /// <summary>
    /// Creates explosions on tiles on the specified axis.
    /// </summary>
    /// <param name="explosionAxis"></param>
    private void CreateExplosionsOnTilesOnAxis(ExplosionAxis explosionAxis)
    {
        for (var axis = -1; axis < 1; axis++)
        {
            for (var nthTile = 1; nthTile <= Range; nthTile++)
            {
                var effectPositionOnNthTile = GetEffectPositionOnAxisOnNthTile(
                    explosionAxis,
                    axis,
                    nthTile
                );

                if (!CanCreateExplosionAtPosition(effectPositionOnNthTile))
                    break;

                CreateExplosionAtPosition(effectPositionOnNthTile);
            }
        }
    }

    /// <summary>
    /// Gets the effect position on the specified axis.
    /// </summary>
    /// <param name="explosionAxis"></param>
    /// <param name="axis"></param>
    /// <param name="nthTile"></param>
    /// <returns></returns>
    private Vector3 GetEffectPositionOnAxisOnNthTile(
        ExplosionAxis explosionAxis,
        int axis,
        int nthTile
    )
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

    /// <summary>
    /// Checks if an explosion can be created at the specified position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private static bool CanCreateExplosionAtPosition(Vector3 position)
    {
        var mapCoordinates = GameManager.GameMap.LocalToMap(position);
        var tileId = GameManager.GameMap.GetCellItem(mapCoordinates);

        return tileId == -1;
    }

    /// <summary>
    /// Creates an explosion at the specified position.
    /// </summary>
    /// <param name="position"></param>
    private void CreateExplosionAtPosition(Vector3 position)
    {
        var effectInstance = _effect.Instantiate<VfxExplosion>();
        effectInstance.Position = position;

        var animationPlayer = effectInstance.GetNode<AnimationPlayer>("AnimationPlayer");

        GameManager.GameMap.AddChild(effectInstance);

        animationPlayer.Play("explosionEffect");
    }
}
