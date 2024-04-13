using System;
using System.Linq;
using Bombino.game.persistence.state_storage;
using Bombino.player;
using Godot;

namespace Bombino.power_up;

internal partial class PowerUp : Area3D
{
    private PowerUpType _type;

    public override void _Ready()
    {
        _type = GetRandomPowerUpType();
    }

    private PowerUpType GetRandomPowerUpType()
    {
        var values = Enum.GetValues(typeof(PowerUpType)).Cast<PowerUpType>().ToArray();

        return values[new Random().Next(values.Length)];
    }

    private void OnBodyEntered(Node3D body)
    {
        if (!body.IsInGroup("players"))
            return;

        var player = body as Player;

        if (_type == PowerUpType.IncreaseMaxBombs)
            player.PlayerData.MaxNumberOfAvailableBombs++;
        else
            player.PlayerData.BombRange++;

        QueueFree();
    }
}
