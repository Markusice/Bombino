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
    /// <param name="player">The player node.</param>
    /// <param name="bombToPlacePosition">The position where the bomb is to be placed.</param>
    /// <returns>True if the player is unable to place a bomb, false otherwise.</returns>
    private static async Task<bool> IsUnableToPlaceBombAsync(Node player, Vector3 bombToPlacePosition)
    {
        var bombArea3D = new Area3D
        {
            Position = bombToPlacePosition
        };
        
        var bombBox = new BoxShape3D { Size = new Vector3(0.4f, 0.4f, 0.4f) };
        bombArea3D.AddChild(new CollisionShape3D { Shape = bombBox });

        bombArea3D.SetCollisionLayerValue(6, true);
        bombArea3D.SetCollisionLayerValue(1, false);

        bombArea3D.SetCollisionMaskValue(2, true);
        bombArea3D.SetCollisionMaskValue(1, false);

        GameManager.WorldEnvironment.AddChild(bombArea3D);

        GD.Print(bombArea3D.Position);

        var isl4131x91391i491i = false;

        bombArea3D.BodyEntered += (Node3D body) => {
            GD.Print(body != player);

            isl4131x91391i491i = true;
        };

        await bombArea3D.GetTree().ToSignal(bombArea3D, "body_entered");
        
        bombArea3D.QueueFree();

        if (isl4131x91391i491i) return true;

        var placedBombs = player.GetTree().GetNodesInGroup("bombs");

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