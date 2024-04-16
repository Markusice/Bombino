using System;
using System.Collections.Generic;
using Bombino.player;
using Bombino.player.input_actions;

namespace Bombino.game.persistence.storage_layers.key_binds;

internal class SettingsKeyBinds
{
    public const string KeyBindsPath = "user://keybinds.cfg";

    private readonly IDataAccessLayer _dataAccessLayer;

    public Dictionary<string, PlayerColor> InputActionsForPlayerColors { get; } = new();

    public SettingsKeyBinds(IDataAccessLayer dataAccessLayer)
    {
        _dataAccessLayer = dataAccessLayer;

        var playerInputActions = new PlayerInputActions();

        var playerInputActionsMovements = playerInputActions.Movements;
        var playerInputActionsPlaceBomb = playerInputActions.BombPlace;

        var playerColors = Enum.GetValues<PlayerColor>();

        foreach (var playerColor in playerColors)
        {
            var playerColorLowerCase = playerColor.ToString().ToLower();

            foreach (var movement in playerInputActionsMovements)
            {
                InputActionsForPlayerColors.Add($"{movement.Name}_{playerColorLowerCase}", playerColor);
            }

            InputActionsForPlayerColors.Add($"{playerInputActionsPlaceBomb.Name}_{playerColorLowerCase}", playerColor);
        }
    }

    public bool SaveKeyBinds()
    {
        return _dataAccessLayer.SaveData(this);
    }

    public bool LoadKeyBinds()
    {
        return _dataAccessLayer.LoadData(this);
    }
}