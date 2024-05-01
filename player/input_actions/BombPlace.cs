using Bombino.bomb;
using Bombino.events;
using Bombino.game;
using Godot;
using Timer = Godot.Timer;

namespace Bombino.player.input_actions;

/// <summary>
/// Represents the action of placing a bomb for the player.
/// </summary>
internal class BombPlace
{
    /// <summary>
    /// Gets the name of the bomb placement action.
    /// </summary>
    public string Name => "place_bomb";

    /// <summary>
    /// Action that places a bomb for the player.
    /// </summary>
    public Action<Player> Action { get; } = player =>
    {
        if (
            player.PlayerData.NumberOfPlacedBombs >= player.PlayerData.MaxNumberOfAvailableBombs
        )
            return;

        var playerCollisionObject = player as CollisionObject3D;
        
        int maskValue = Bomb.GetMaskValueFromPlayerName(player);
        if (playerCollisionObject.GetCollisionMaskValue(maskValue))
            playerCollisionObject.SetCollisionMaskValue(maskValue, false);

        var bombTilePosition = GameManager.GameMap.MapToLocal(player.MapPosition);
        var bombToPlacePosition = new Vector3(
            bombTilePosition.X,
            GameManager.GameMap.CellSize.Y + 1,
            bombTilePosition.Z
        );

        if (IsUnableToPlaceBomb(player, bombToPlacePosition))
            return;

        player.PlayerData.NumberOfPlacedBombs++;

        Events.Instance.EmitSignal(
            Events.SignalName.PlayerBombNumberDecreased,
            player.PlayerData.Color.ToString(),
            player.PlayerData.MaxNumberOfAvailableBombs - player.PlayerData.NumberOfPlacedBombs
        );

        var bombToPlace = CreateBomb(player, bombToPlacePosition);

        var bombCollisionObject = bombToPlace.GetNode<StaticBody3D>("%BombObject");
        bombCollisionObject.SetCollisionLayerValue(maskValue, true);

        var timer = bombToPlace.GetNode<Timer>("BombTimer");
        timer.Timeout += () =>
        {
            if (player.PlayerData.IsDead) return;

            Events.Instance.EmitSignal(
                Events.SignalName.PlayerBombNumberIncremented,
                player.PlayerData.Color.ToString(),
                player.PlayerData.MaxNumberOfAvailableBombs
                - player.PlayerData.NumberOfPlacedBombs
            );
        };

        GameManager.WorldEnvironment.AddChild(bombToPlace);
    };

    /// <summary>
    /// Checks if the player is unable to place a bomb.
    /// </summary>
    /// <param name="player">The player node.</param>
    /// <param name="bombToPlacePosition">The position where the bomb is to be placed.</param>
    /// <returns>True if the player is unable to place a bomb, false otherwise.</returns>
    private static bool IsUnableToPlaceBomb(Player player, Vector3 bombToPlacePosition)
    {
        var placedBombs = player.GetTree().GetNodesInGroup("bombs");

        var isBodyNearBombPlacement = IsBodyNearBombPosition(player, bombToPlacePosition);

        return isBodyNearBombPlacement ||
               placedBombs
                   .Cast<Area3D>()
                   .Any(bombArea3D => bombArea3D.Position == bombToPlacePosition);
    }

    /// <summary>
    /// Checks if a body is near the bomb placement.
    /// </summary>
    /// <param name="player">The player who is placing the bomb.</param>
    /// <param name="bombToPlacePosition">The position where the bomb is to be placed.</param>
    /// <returns>True if a body other than the placer is near the bomb placement, false otherwise.</returns>
    private static bool IsBodyNearBombPosition(Player player, Vector3 bombToPlacePosition)
    {
        var collisionGroups = new[] { "players", "enemies" };
        var safeDistance = GameManager.GameMap.CellSize.X;

        return collisionGroups.SelectMany(group => player.GetTree()
                .GetNodesInGroup(group)
                .Cast<CharacterBody3D>()
                .Where(body => body != player))
            .Any(body => body.Position.DistanceTo(bombToPlacePosition) < safeDistance);
    }

    /// <summary>
    /// Creates a bomb for the player.
    /// </summary>
    /// <param name="player">The player who is placing the bomb.</param>
    /// <param name="bombToPlacePosition">The position where the bomb is to be placed.</param>
    /// <returns>The created bomb.</returns>
    private static Bomb CreateBomb(Player player, Vector3 bombToPlacePosition)
    {
        var bombToPlace = player.BombScene.Instantiate<Bomb>();

        bombToPlace.Position = bombToPlacePosition;
        bombToPlace.Range = player.PlayerData.BombRange;
        bombToPlace.Player = player;

        var collisionShapeX = bombToPlace.GetNode<CollisionShape3D>("CollisionShapeX");
        var collisionShapeZ = bombToPlace.GetNode<CollisionShape3D>("CollisionShapeZ");

        const int collisionShapeOneTileLengthInBothDirection = 4;
        var bombInstanceCollisionLength = GameManager.GameMap.CellSize.X;

        collisionShapeX.Shape = new BoxShape3D
        {
            Size = new Vector3(
                bombInstanceCollisionLength + bombToPlace.Range * collisionShapeOneTileLengthInBothDirection,
                ((BoxShape3D)collisionShapeX.Shape).Size.Y,
                ((BoxShape3D)collisionShapeX.Shape).Size.Z)
        };

        collisionShapeZ.Shape = new BoxShape3D
        {
            Size = new Vector3(((BoxShape3D)collisionShapeZ.Shape).Size.X,
                ((BoxShape3D)collisionShapeZ.Shape).Size.Y,
                bombInstanceCollisionLength + bombToPlace.Range * collisionShapeOneTileLengthInBothDirection)
        };

        return bombToPlace;
    }
}