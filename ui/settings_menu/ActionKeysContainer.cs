using Bombino.game.persistence.storage_layers.key_binds;
using Godot;

namespace Bombino.ui.settings_menu;

/// <summary>
/// Represents a container for action keys in the user interface.
/// </summary>
internal partial class ActionKeysContainer : GridContainer
{   
    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        var settingsDataAccessLayer = new SettingsDataAccessLayer();
        var settingsKeyBinds = new SettingsKeyBinds(settingsDataAccessLayer);

        ActionKeysMapper.CreateActionKeys(this, settingsKeyBinds.InputActionsForPlayerColors);
    }
}
