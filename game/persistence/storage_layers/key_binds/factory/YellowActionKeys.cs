using Bombino.player;
using Bombino.ui.settings_menu;
using Godot.Collections;

namespace Bombino.game.persistence.storage_layers.key_binds.factory;

public class YellowActionKeys : IActionKeys
{
    static YellowActionKeys()
    {
        PlayerActionKeysFactory.RegisterInstance(PlayerColor.Yellow, new YellowActionKeys());
    }

    public Array<string> CreateActionKeys()
    {
        return ActionKeysContainer.ActionItems[10..15];
    }
}