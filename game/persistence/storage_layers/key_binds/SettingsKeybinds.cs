using Bombino.player;
using Bombino.player.input_actions;

namespace Bombino.game.persistence.storage_layers.key_binds;

internal class SettingsKeyBinds : ISettingsKeyBinds
{
    #region Fields

    private readonly ISettingsDataAccessLayer<Dictionary<string, PlayerColor>> _settingsDataAccessLayer;

    public Dictionary<string, PlayerColor> InputActionsForPlayerColors { get; } = new();

    #endregion

    public SettingsKeyBinds(ISettingsDataAccessLayer<Dictionary<string, PlayerColor>> settingsDataAccessLayer)
    {
        _settingsDataAccessLayer = settingsDataAccessLayer;

        var playerInputActions = new PlayerInputActions();

        var playerInputActionsMovements = playerInputActions.Movements;
        var playerInputActionsPlaceBomb = playerInputActions.BombPlace;

        var playerColors = Enum.GetValues<PlayerColor>();

        AddInputActionsForPlayerColor(playerColors, playerInputActionsMovements, playerInputActionsPlaceBomb);
    }

    public bool SaveKeyBinds()
    {
        return _settingsDataAccessLayer.SaveData(InputActionsForPlayerColors);
    }

    public bool LoadKeyBinds()
    {
        return _settingsDataAccessLayer.LoadData(InputActionsForPlayerColors);
    }

    private void AddInputActionsForPlayerColor(IEnumerable<PlayerColor> playerColors,
        Movement[] playerInputActionsMovements,
        BombPlace playerInputActionsPlaceBomb)
    {
        foreach (var playerColor in playerColors)
        {
            var playerColorLowerCase = playerColor.ToString().ToLower();

            AddMovementInputActions(playerInputActionsMovements, playerColorLowerCase, playerColor);

            AddPlaceBombInputAction(playerInputActionsPlaceBomb, playerColorLowerCase, playerColor);
        }
    }

    private void AddMovementInputActions(IEnumerable<Movement> playerInputActionsMovements, string playerColorLowerCase,
        PlayerColor playerColor)
    {
        foreach (var movement in playerInputActionsMovements)
        {
            InputActionsForPlayerColors.Add($"{movement.Name}_{playerColorLowerCase}", playerColor);
        }
    }

    private void AddPlaceBombInputAction(BombPlace playerInputActionsPlaceBomb, string playerColorLowerCase,
        PlayerColor playerColor)
    {
        InputActionsForPlayerColors.Add($"{playerInputActionsPlaceBomb.Name}_{playerColorLowerCase}", playerColor);
    }
}