using Bombino.player;
using Bombino.player.input_actions;

namespace Bombino.game.persistence.storage_layers.key_binds;

/// <summary>
/// Represents a key binds settings data access layer.
/// </summary>
internal class SettingsKeyBinds : ISettingsKeyBinds
{
    #region Fields

    private readonly ISettingsDataAccessLayer<
        Dictionary<string, PlayerColor>
    > _settingsDataAccessLayer;

    public Dictionary<string, PlayerColor> InputActionsForPlayerColors { get; } = new();

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsKeyBinds"/> class.
    /// </summary>
    /// <param name="settingsDataAccessLayer">The settings data access layer to use for saving and loading key binds.</param>
    public SettingsKeyBinds(
        ISettingsDataAccessLayer<Dictionary<string, PlayerColor>> settingsDataAccessLayer
    )
    {
        _settingsDataAccessLayer = settingsDataAccessLayer;

        var playerInputActions = new PlayerInputActions();

        var playerInputActionsMovements = playerInputActions.Movements;
        var playerInputActionsPlaceBomb = playerInputActions.BombPlace;

        var playerColors = Enum.GetValues<PlayerColor>();

        AddInputActionsForPlayerColor(
            playerColors,
            playerInputActionsMovements,
            playerInputActionsPlaceBomb
        );
    }

    #region InterfaceMethods

    public bool SaveKeyBinds()
    {
        return _settingsDataAccessLayer.SaveData(InputActionsForPlayerColors);
    }

    public bool LoadKeyBinds()
    {
        return _settingsDataAccessLayer.LoadData(InputActionsForPlayerColors);
    }

    public bool IsLoaded()
    {
        return InputActionsForPlayerColors.Count != 0;
    }

    #endregion

    /// <summary>
    /// Adds input actions for the player color.
    /// </summary>
    /// <param name="playerColors">The player colors to add input actions for.</param>
    /// <param name="playerInputActionsMovements">The player input actions movements to add.</param>
    /// <param name="playerInputActionsPlaceBomb">The player input actions place bomb to add.</param>
    private void AddInputActionsForPlayerColor(
        IEnumerable<PlayerColor> playerColors,
        Movement[] playerInputActionsMovements,
        BombPlace playerInputActionsPlaceBomb
    )
    {
        foreach (var playerColor in playerColors)
        {
            AddMovementInputActions(playerInputActionsMovements, playerColor);
            AddPlaceBombInputAction(playerInputActionsPlaceBomb, playerColor);
        }
    }

    /// <summary>
    /// Adds movement input actions for the player color.
    /// </summary>
    /// <param name="playerInputActionsMovements">The player input actions movements to add.</param>
    /// <param name="playerColor">The player color.</param>
    private void AddMovementInputActions(
        IEnumerable<Movement> playerInputActionsMovements,
        PlayerColor playerColor
    )
    {
        foreach (var movement in playerInputActionsMovements)
        {
            InputActionsForPlayerColors.Add(
                $"{movement.Name}_{playerColor.ToString().ToLower()}",
                playerColor
            );
        }
    }

    /// <summary>
    /// Adds place bomb input actions for the player color.
    /// </summary>
    /// <param name="playerInputActionsPlaceBomb">The player input actions place bomb to add.</param>
    /// <param name="playerColor">The player color.</param>
    private void AddPlaceBombInputAction(
        BombPlace playerInputActionsPlaceBomb,
        PlayerColor playerColor
    )
    {
        InputActionsForPlayerColors.Add(
            $"{playerInputActionsPlaceBomb.Name}_{playerColor.ToString().ToLower()}",
            playerColor
        );
    }
}
