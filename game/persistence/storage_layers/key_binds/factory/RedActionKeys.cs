using Bombino.player;
using Bombino.ui.settings_menu;
using Godot.Collections;

namespace Bombino.game.persistence.storage_layers.key_binds.factory;

internal class RedActionKeys : IActionKeys
{
    static RedActionKeys()
    {
        PlayerActionKeysFactory.RegisterInstance(PlayerColor.Red, new RedActionKeys());
    }

    public Array<string> CreateActionKeys()
    {
        return ActionKeysContainer.ActionItems[5..10];
    }
}