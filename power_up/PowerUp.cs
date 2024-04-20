using Bombino.events;
using Bombino.player;
using Godot;

namespace Bombino.power_up;

internal partial class PowerUp : Area3D
{
    [Export(PropertyHint.File, ".tres")] private Material _hologramPink;

    [Export(PropertyHint.File, ".tres")] private Material _hologramBlue;

    private PowerUpType _type;
    private MeshInstance3D _meshInstance3D;

    public override void _Ready()
    {
        _meshInstance3D = GetNode<MeshInstance3D>("%MeshInstance3D");
        _type = GetRandomPowerUpType();

        SetMaterialBasedOnType();
    }

    private static PowerUpType GetRandomPowerUpType()
    {
        var values = Enum.GetValues(typeof(PowerUpType)).Cast<PowerUpType>().ToArray();

        return values[new Random().Next(values.Length)];
    }

    private void SetMaterialBasedOnType()
    {
        var material = _type == PowerUpType.IncreaseBombRange ? _hologramBlue : _hologramPink;

        _meshInstance3D.SetSurfaceOverrideMaterial(0, material);
    }

    private void OnBodyEntered(Node3D body)
    {
        if (!body.IsInGroup("players"))
            return;

        var player = (Player)body;

        if (_type == PowerUpType.IncreaseMaxBombs)
        {
            player.PlayerData.MaxNumberOfAvailableBombs++;

            Events.Instance.EmitSignal(
                Events.SignalName.PlayerBombNumberIncremented,
                player.PlayerData.Color.ToString(),
                player.PlayerData.MaxNumberOfAvailableBombs - player.PlayerData.NumberOfPlacedBombs
            );
        }
        else
            player.PlayerData.BombRange++;

        QueueFree();
    }
}