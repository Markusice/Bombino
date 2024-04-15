using System;
using System.Linq;
using Bombino.bomb;
using Bombino.game;
using Godot;

namespace Bombino.player.input_actions;

internal class BombPlace
{
    public string Name => "place_bomb";

    public Action<Player> Action { get; } = (player) =>
    {
        if (player.PlayerData.NumberOfPlacedBombs >= player.PlayerData.MaxNumberOfAvailableBombs)
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

        var bombToPlace = CreateBomb(player, bombToPlacePosition);
        GameManager.WorldEnvironment.AddChild(bombToPlace);
    };

    /// <summary>
    /// Checks if the player is unable to place a bomb.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="bombToPlacePosition"></param>
    /// <returns></returns>
    private static bool IsUnableToPlaceBomb(Node node, Vector3 bombToPlacePosition)
    {
        var placedBombs = node.GetTree().GetNodesInGroup("bombs");

        return placedBombs
            .Cast<Area3D>()
            .Any(bombArea3D => bombArea3D.Position == bombToPlacePosition);
    }

    /// <summary>
    /// Creates a bomb.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="bombToPlacePosition"></param>
    /// <returns></returns>
    private static Bomb CreateBomb(Player player, Vector3 bombToPlacePosition)
    {
        var bombToPlace = player.BombScene.Instantiate<Bomb>();

        bombToPlace.Position = bombToPlacePosition;
        bombToPlace.Range = player.PlayerData.BombRange;
        bombToPlace.Player = player;

        return bombToPlace;
    }
}