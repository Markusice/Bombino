using Bombino.bomb.explosion_effect;
using Bombino.enemy;
using Bombino.game;
using Bombino.game.persistence.state_storage;
using Bombino.player;
using Bombino.power_up;
using Godot;
using Godot.Collections;
using Timer = Godot.Timer;

namespace Bombino.bomb;

/// <summary>
/// Represents a bomb in the game.
/// </summary>
internal partial class Bomb : Area3D
{
    #region Exports

    [Export(PropertyHint.Range, "1,4")] private float _explodeTime = Mathf.Pi;

    [Export] private PackedScene _effectScene;

    [Export(PropertyHint.File, "*.tscn")] private string _powerUpScenePath;

    #endregion

    #region Signals

    /// <summary>
    /// Signal emitted when the bomb explodes sooner.
    /// <param name="newTimerWaitTime"></param>
    /// </summary>
    [Signal]
    public delegate void ExplodeSoonerEventHandler(float newTimerWaitTime);

    #endregion

    #region Fields

    public BombData BombData { get; set; } = new BombData();

    private readonly Array<Node3D> _bodiesToExplode = new();
    private readonly Array<Area3D> _bombsInRange = new();

    private const float ExplosionDistanceDivider = Mathf.E;

    private Area3D _bombCollisionArea;

    public Player Player { get; set; }

    #endregion

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        GD.Print($"bomb pos: {Position}");
        BombData.Position = Position;
        BombData.PlayerData = Player.PlayerData;
        _bombCollisionArea = GetNode<Area3D>("%CollisionArea");

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

    /// <summary>
    /// Called when a body enters the area.
    /// </summary>
    /// <param name="body"></param>
    private void OnBodyEntered(Node3D body)
    {
        if (body.IsInGroup("players") || body.IsInGroup("enemies"))
            _bodiesToExplode.Add(body);
    }

    /// <summary>
    /// Called when a body exits the area.
    /// </summary>
    /// <param name="body"></param>
    private void OnBodyExited(Node3D body)
    {
        if (body.IsInGroup("players") || body.IsInGroup("enemies"))
            _bodiesToExplode.Remove(body);
    }

    /// <summary>
    /// Called when the bomb enters an area
    /// </summary>
    /// <param name="area">The area that the bomb entered.</param>
    private void OnAreaEntered(Area3D area)
    {
        if (!area.IsInGroup("collisionareas"))
            return;

        if (_bombCollisionArea.GetRid() == area.GetRid())
            return;

        _bombsInRange.Add(GetBombFromBombObject(area));
    }

    /// <summary>
    /// Called when the bomb exits an area.
    /// </summary>
    /// <param name="area">The area that the bomb exited.</param>
    private void OnAreaExited(Area3D area)
    {
        if (!area.IsInGroup("collisionareas"))
            return;

        if (_bombCollisionArea.GetRid() == area.GetRid())
            return;

        _bombsInRange.Remove(GetBombFromBombObject(area));
    }

    /// <summary>
    /// Called when the Player exits a bomb's Area3D.
    /// </summary>
    /// <param name="body">The body that exited the bomb's Area3D.</param>
    private void OnEnableBombCollisionForPlacer(Node3D body)
    {
        if (body != Player)
            return;

        var bombObject = GetNode<StaticBody3D>("%BombObject");
        var layerValue = GetNextLayerValueFromPlayerName(Player); 

        if (!bombObject.GetCollisionLayerValue(layerValue))
            bombObject.SetCollisionLayerValue(layerValue, true);

    }

    /// <summary>
    /// Gets the next layer value from the player's actual bomb layer.
    /// </summary>
    /// <param name="player">The player to get the layer value from.</param>
    /// <returns>The next layer value from the player's actual bomb layer.</returns>
    private static int GetNextLayerValueFromPlayerName(Player player)
    {
        return player.PlayerData.Color switch
        {
            PlayerColor.Blue => GetMaskValueFromPlayerColor(PlayerColor.Red),
            PlayerColor.Red => GetMaskValueFromPlayerColor(PlayerColor.Yellow),
            PlayerColor.Yellow => GetMaskValueFromPlayerColor(PlayerColor.Blue),
            _ => 0
        };
    }

    /// <summary>
    /// Gets the mask value from the player's color.
    /// </summary>
    /// <param name="color">The player's color.</param>
    /// <returns>The mask value from the player's color.</returns>
    public static int GetMaskValueFromPlayerColor(PlayerColor color)
    {
        return color switch
        {
            PlayerColor.Blue => 8,
            PlayerColor.Red => 9,
            PlayerColor.Yellow => 10,
            _ => 0
        };
    }

    /// <summary>
    /// Called when the timer times out.
    /// </summary>
    private void OnTimerTimeout()
    {
        DecreaseNumberOfPlacedBombs();

        DestroyCratesOnXAndZAxis();

        PlayExplodeAnimation();

        CreateExplosionsOnTilesOnXAndZAxis();

        ExplodeBodies();

        SendSignalToBombsInRange();

        QueueFree();
    }

    #endregion

    /// <summary>
    /// Called when the bomb explodes.
    /// </summary>
    private void DecreaseNumberOfPlacedBombs()
    {
        Player.PlayerData.NumberOfPlacedBombs--;
    }

    /// <summary>
    /// Gets the bomb from the bomb object.
    /// </summary>
    /// <param name="body">The bomb object.</param>
    /// <returns>The bomb.</returns>
    private static Area3D GetBombFromBombObject(Node3D body)
    {
        return body.GetParent<MeshInstance3D>().GetParent<Area3D>();
    }

    /// <summary>
    /// Explodes the bodies in the area.
    /// </summary>
    private void ExplodeBodies()
    {
        foreach (var bodyToExplode in _bodiesToExplode)
        {
            if (!bodyToExplode.IsInGroup("players") && !bodyToExplode.IsInGroup("enemies"))
                return;

            var rayDirections = GetRayCastDirections();

            var bodyShouldNotDie = CastRaysInDirectionsAndGetBodyShouldNotDie(
                bodyToExplode,
                rayDirections
            );

            if (bodyShouldNotDie)
                continue;

            bodyToExplode.EmitSignal(
                bodyToExplode.IsInGroup("players") ? Player.SignalName.Hit : Enemy.SignalName.Hit
            );
        }
    }

    /// <summary>
    /// Gets the raycast directions.
    /// </summary>
    /// <returns>An array of raycast directions.</returns>
    private static Vector3[] GetRayCastDirections()
    {
        return new[] { Vector3.Left, Vector3.Right, Vector3.Back, Vector3.Forward };
    }

    /// <summary>
    /// Casts rays in the specified directions and checks if the player should die.
    /// </summary>
    /// <param name="bodyToExplode"></param>
    /// <param name="rayDirections"></param>
    /// <returns>True if the body should not die, false otherwise.</returns>
    private bool CastRaysInDirectionsAndGetBodyShouldNotDie(
        Node3D bodyToExplode,
        Vector3[] rayDirections
    )
    {
        var bodyShouldNotDie = false;

        foreach (var rayDirection in rayDirections)
        {
            var origin = GlobalPosition;
            var end = origin + rayDirection * (BombData.Range * 2);
            var rayMask = GameManager.GameMap.CollisionMask;

            var result = CastRayAndGetResult(origin, end, rayMask);

            if (result.Count == 0)
                continue;

            var colliderPosition = result["position"].AsVector3();

            bodyShouldNotDie = GetBodyShouldNotDieInDirection(
                rayDirection,
                colliderPosition,
                bodyToExplode
            );

            if (bodyShouldNotDie)
                break;
        }

        return bodyShouldNotDie;
    }

    /// <summary>
    /// Gets if the body should not die in the specified direction.
    /// </summary>
    /// <param name="direction">The direction to check.</param>
    /// <param name="colliderPosition">The position of the collider.</param>
    /// <param name="body">The body to check.</param>
    /// <returns>True if the body should not die in the specified direction, false otherwise.</returns>
    private static bool GetBodyShouldNotDieInDirection(
        Vector3 direction,
        Vector3 colliderPosition,
        Node3D body
    )
    {
        var isBodySafeInDirections = new System.Collections.Generic.Dictionary<
            Vector3,
            Func<Vector3, Node3D, bool>
        >
        {
            { Vector3.Left, IsBodySafeFromBombOnLeft },
            { Vector3.Right, IsBodySafeFromBombOnRight },
            { Vector3.Back, IsBodySafeFromBombOnBack },
            { Vector3.Forward, IsBodySafeFromBombOnForward }
        };

        return isBodySafeInDirections[direction].Invoke(colliderPosition, body);
    }

    /// <summary>
    /// Checks if the body is safe from the bomb on the left.
    /// </summary>
    /// <param name="colliderPosition">The position of the collider.</param>
    /// <param name="body">The body to check.</param>
    /// <returns>True if the body is safe from the bomb on the left, false otherwise.</returns>
    private static bool IsBodySafeFromBombOnLeft(Vector3 colliderPosition, Node3D body)
    {
        return colliderPosition.X - body.Position.X >= GameManager.GameMap.CellSize.X;
    }

    /// <summary>
    /// Checks if the body is safe from the bomb on the right.
    /// </summary>
    /// <param name="colliderPosition">The position of the collider.</param>
    /// <param name="body">The body to check.</param>
    /// <returns>True if the body is safe from the bomb on the right, false otherwise.</returns>
    private static bool IsBodySafeFromBombOnRight(Vector3 colliderPosition, Node3D body)
    {
        return body.Position.X - colliderPosition.X >= GameManager.GameMap.CellSize.X;
    }

    /// <summary>
    /// Checks if the body is safe from the bomb on the back.
    /// </summary>
    /// <param name="colliderPosition">The position of the collider.</param>
    /// <param name="body">The body to check.</param>
    /// <returns>True if the body is safe from the bomb on the back, false otherwise.</returns>
    private static bool IsBodySafeFromBombOnBack(Vector3 colliderPosition, Node3D body)
    {
        return body.Position.Z - colliderPosition.Z >= GameManager.GameMap.CellSize.Z;
    }

    /// <summary>
    /// Checks if the body is safe from the bomb on the forward.
    /// </summary>
    /// <param name="colliderPosition">The position of the collider.</param>
    /// <param name="body">The body to check.</param>
    /// <returns>True if the body is safe from the bomb on the forward, false otherwise.</returns>
    private static bool IsBodySafeFromBombOnForward(Vector3 colliderPosition, Node3D body)
    {
        return colliderPosition.Z - body.Position.Z >= GameManager.GameMap.CellSize.Z;
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
        GD.Print($"bombsInRange: {_bombsInRange}");
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
    /// Destroys the crates on the X and Z axis.
    /// </summary>
    private void DestroyCratesOnXAndZAxis()
    {
        DestroyCratesOnAxis(ExplosionAxis.X);
        DestroyCratesOnAxis(ExplosionAxis.Z);
    }

    /// <summary>
    /// Destroys the first crates on the specified axis.
    /// </summary>
    /// <param name="explosionAxis">The axis to destroy the crates on.</param>
    private void DestroyCratesOnAxis(ExplosionAxis explosionAxis)
    {
        for (var axis = -1; axis < 1; axis++)
        {
            for (var nthTile = 1; nthTile <= BombData.Range; nthTile++)
            {
                var positionOnNthTile = GetEffectPositionOnAxisOnNthTile(
                    explosionAxis,
                    axis,
                    nthTile
                );

                if (IsTileTypeAtPosition(positionOnNthTile, 3))
                    break;

                if (IsTileTypeAtPosition(positionOnNthTile, -1))
                    continue;

                if (IsTileTypeAtPosition(positionOnNthTile, 2))
                    DestroyCrateAtPosition(positionOnNthTile);

                break;
            }
        }
    }

    /// <summary>
    /// Checks if the tile type is at the specified position.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <param name="tileType">The tile type to check.</param>
    /// <returns>True if the tile type is at the specified position, false otherwise.</returns>
    private static bool IsTileTypeAtPosition(Vector3 position, long tileType)
    {
        var mapCoordinates = GameManager.GameMap.LocalToMap(position);
        var tileId = GameManager.GameMap.GetCellItem(mapCoordinates);
        return tileId == tileType;
    }

    /// <summary>
    /// Destroys the crate at the specified position.
    /// </summary>
    /// <param name="position">The position of the crate to destroy.</param>
    private void DestroyCrateAtPosition(Vector3 position)
    {
        var mapCoordinates = GameManager.GameMap.LocalToMap(position);
        GameManager.GameMap.SetCellItem(mapCoordinates, -1);

        if (GetRandomPowerUpSpawnChance() <= 2)
        {
            GD.Print($"Spawned power up at position {position}");
            SpawnPowerUp(position);
        }
    }

    /// <summary>
    /// Gets a random power up spawn chance.
    /// </summary>
    /// <returns>A random integer from 0 to 10.</returns>
    private static int GetRandomPowerUpSpawnChance()
    {
        return new Random().Next(0, 10);
    }

    /// <summary>
    /// Spawns a power up at the specified position.
    /// </summary>
    /// <param name="position">The position to spawn the power up at.</param>
    private void SpawnPowerUp(Vector3 position)
    {
        var powerUpInstance = ResourceLoader.Load<PackedScene>(_powerUpScenePath);
        var powerUp = powerUpInstance.Instantiate<PowerUp>();
        powerUp.Position = position;

        GameManager.GameMap.AddChild(powerUp);
    }

    /// <summary>
    /// Plays the explode animation.
    /// </summary>
    private void PlayExplodeAnimation()
    {
        var bombMeshInstance3D = GetNode<MeshInstance3D>("Bomb");
        bombMeshInstance3D.Hide();

        var effectInstance = _effectScene.Instantiate<VfxExplosion>();
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
        for (var nthTile = 1; nthTile <= BombData.Range; nthTile++)
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

    /// <summary>
    /// Gets the effect position on the specified axis.
    /// </summary>
    /// <param name="explosionAxis"> The axis to get the effect position on.</param>
    /// <param name="axis"> The axis to get the effect position on.</param>
    /// <param name="nthTile"> The nth tile to get the effect position on.</param>
    /// <returns>The effect position on the specified axis.</returns>
    private Vector3 GetEffectPositionOnAxisOnNthTile(
        ExplosionAxis explosionAxis,
        int axis,
        int nthTile
    )
    {
        var isAxisDirectionNegative = axis < 0;

        if (explosionAxis == ExplosionAxis.X)
            return new Vector3(
                isAxisDirectionNegative
                    ? Position.X - nthTile * GameManager.GameMap.CellSize.X
                    : Position.X + nthTile * GameManager.GameMap.CellSize.X,
                Position.Y,
                Position.Z
            );

        return new Vector3(
            Position.X,
            Position.Y,
            isAxisDirectionNegative
                ? Position.Z - nthTile * GameManager.GameMap.CellSize.Z
                : Position.Z + nthTile * GameManager.GameMap.CellSize.Z
        );
    }

    /// <summary>
    /// Checks if an explosion can be created at the specified position.
    /// </summary>
    /// <param name="position"> The position to check.</param>
    /// <returns>True if an explosion can be created at the specified position, false otherwise.</returns>
    private static bool CanCreateExplosionAtPosition(Vector3 position)
    {
        var mapCoordinates = GameManager.GameMap.LocalToMap(position);
        var tileId = GameManager.GameMap.GetCellItem(mapCoordinates);

        return tileId == -1;
    }

    /// <summary>
    /// Creates an explosion at the specified position.
    /// </summary>
    /// <param name="position"> The position to create the explosion at.</param>
    private void CreateExplosionAtPosition(Vector3 position)
    {
        var effectInstance = _effectScene.Instantiate<VfxExplosion>();
        effectInstance.Position = position;

        var animationPlayer = effectInstance.GetNode<AnimationPlayer>("AnimationPlayer");

        GameManager.GameMap.AddChild(effectInstance);

        animationPlayer.Play("explosionEffect");
    }
}