using Bombino.player;
using Bombino.ui.settings_menu;
using Godot.Collections;

namespace Bombino.game.persistence.storage_layers.key_binds.factory;

internal class BlueActionKeys : IActionKeys
{
    static BlueActionKeys()
    {
        PlayerActionKeysFactory.RegisterInstance(PlayerColor.Blue, new BlueActionKeys());
    }

    public Array<string> CreateActionKeys()
    {
        return ActionKeysContainer.ActionItems[..5];
    }
}