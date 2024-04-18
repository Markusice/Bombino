using System;
using System.Linq;
using Bombino.bomb;
using Bombino.events;
using Bombino.game;
using Godot;

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

            var collisionObject = player as CollisionObject3D;
            collisionObject.SetCollisionMaskValue(5, false);

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
    /// <param name="node">The player node.</param>
    /// <param name="bombToPlacePosition">The position where the bomb is to be placed.</param>
    /// <returns>True if the player is unable to place a bomb, false otherwise.</returns>
    private static bool IsUnableToPlaceBomb(Node node, Vector3 bombToPlacePosition)
    {
        var placedBombs = node.GetTree().GetNodesInGroup("bombs");

        return placedBombs
            .Cast<Area3D>()
            .Any(bombArea3D => bombArea3D.Position == bombToPlacePosition);
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

        return bombToPlace;
    }
}
