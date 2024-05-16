using Bombino.events;
using Bombino.player;
using Godot;

namespace Bombino.power_up;

/// <summary>
/// Represents a power-up in the game.
/// </summary>
internal partial class PowerUp : Area3D
{
    #region Exports

    [Export(PropertyHint.File, ".tres")]
    private Material _hologramPink;

    #endregion

    #region Fields

    private PowerUpType _type;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PowerUp"/> class.
    /// </summary>
    public override void _Ready()
    {
        _type = GetRandomPowerUpType();
        ShowMaterialBasedOnType();
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
    /// Shows the material based on the type of power-up.
    /// </summary>
    private void ShowMaterialBasedOnType()
    {
        var color = _type == PowerUpType.IncreaseMaxBombs ? "Pink" : "Blue";
        ShowMeshInstance3D(color);

        Show(); // make the parent visible too
    }

    /// <summary>
    /// Shows the MeshInstance3D with the specified color.
    /// </summary>
    /// <param name="color">The color of the MeshInstance3D to show.</param>
    private void ShowMeshInstance3D(string color)
    {
        GetNode<MeshInstance3D>($"%MeshInstance3D{color}").Show();
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
