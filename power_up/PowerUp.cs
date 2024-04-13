using System;
using System.Linq;
using System.Runtime.CompilerServices;
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
    }
}
