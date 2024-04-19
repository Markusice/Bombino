using Bombino.game.persistence.state_storage;
using Bombino.player;
using Bombino.player.input_actions;
using GdUnit4;
using Godot;

namespace Bombino.test.player;

[TestSuite]
public class PlayerDataTest
{
    private Player player;

    [Before]
    public void SetUpPlayer()
    {
        player = new Player();
    }

    [TestCase]
    public void IsPlayerDataSet()
    {
        var playerData = new PlayerData(new Vector3(3, 2, 1), PlayerColor.Blue);

        player.PlayerData = playerData;

        player.Position = player.PlayerData.Position;
        player.Name = $"Player{playerData.Color.ToString()}";

        Assertions.AssertVector(player.Position).IsEqual(playerData.Position);
        Assertions.AssertString(player.Name).IsEqual($"Player{playerData.Color.ToString()}");
    }

    [TestCase]
    public void IsPlayerInputActionsSet()
    {
        Assertions.AssertArray(player.PlayerInputActions.Movements).IsEqual(new[]
        {
            Movement.MoveForward,
            Movement.MoveBackward, Movement.MoveLeft, Movement.MoveRight
        });
    }
}