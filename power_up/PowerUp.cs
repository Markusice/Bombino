using Bombino.events;
using Bombino.player;
using Godot;

namespace Bombino.power_up;

/// <summary>
/// Represents a power-up in the game.
/// </summary>
internal partial class PowerUp : Area3D
{
    [Export(PropertyHint.File, ".tres")] private Material _hologramPink;

    [Export(PropertyHint.File, ".tres")] private Material _hologramBlue;

    private PowerUpType _type;
    private MeshInstance3D _meshInstance3D;

    /// <summary>
    /// Initializes a new instance of the <see cref="PowerUp"/> class.
    /// </summary>
    public override void _Ready()
    {
        _meshInstance3D = GetNode<MeshInstance3D>("%MeshInstance3D");
        _type = GetRandomPowerUpType();

        SetMaterialBasedOnType();
    }

    /// <summary>
    /// Gets a random a power-up type.
    /// </summary>
    /// <returns>A random power-up type.</returns>
    private static PowerUpType GetRandomPowerUpType()
    {
        var values = Enum.GetValues(typeof(PowerUpType)).Cast<PowerUpType>().ToArray();

        return values[new Random().Next(values.Length)];
    }

    /// <summary>
    /// Sets the material based on the power-up type.
    /// </summary>
    private void SetMaterialBasedOnType()
    {
        var material = _type == PowerUpType.IncreaseBombRange ? _hologramBlue : _hologramPink;

        _meshInstance3D.SetSurfaceOverrideMaterial(0, material);
    }

    /// <summary>
    /// Called when the body enters the area.
    /// Increases the player's bomb range or the maximum number of available bombs according to the given power-up.
    /// </summary>
    /// <param name="body">The body that entered the area.</param>
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