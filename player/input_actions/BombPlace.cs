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
    public Action<Player> Action { get; } =
        (player) =>
        {
            if (
                player.PlayerData.NumberOfPlacedBombs >= player.PlayerData.MaxNumberOfAvailableBombs
            )
                return;

             var playerCollisionObject = player as CollisionObject3D;
            playerCollisionObject.SetCollisionMaskValue(5, false);

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
            bombCollisionObject.SetCollisionLayerValue(6, false);

            var timer = bombToPlace.GetNode<Timer>("BombTimer");
            timer.Timeout += () =>
                Events.Instance.EmitSignal(
                    Events.SignalName.PlayerBombNumberIncremented,
                    player.PlayerData.Color.ToString(),
                    player.PlayerData.MaxNumberOfAvailableBombs
                    - player.PlayerData.NumberOfPlacedBombs
                );
                
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
        var collisionGroups = new string[] { "players", "enemies" };
        const float safeDistance = 2.0f;

        foreach (var group in collisionGroups)
        {
            var bodiesInCollisionGroup = player.GetTree().GetNodesInGroup(group)
                .Cast<CharacterBody3D>()
                .Where(body => body != player);

            foreach (CharacterBody3D body in bodiesInCollisionGroup)
            {   
                //GD.Print($"{body.Name} is near bomb placement {body.Position.DistanceTo(bombToPlacePosition)}: {body.Position.DistanceTo(bombToPlacePosition) < safeDistance}");
                
                if (body.Position.DistanceTo(bombToPlacePosition) < safeDistance)
                {   
                    return true;
                }
            }
        }

        return false;
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

        collisionShapeX.Shape = new BoxShape3D { Size = new Vector3(bombToPlace.Range * 4 + 2, 1, 1.5f) };
        collisionShapeZ.Shape = new BoxShape3D { Size = new Vector3(1.5f, 1, bombToPlace.Range * 4 + 2) };

        return bombToPlace;
    }
}
