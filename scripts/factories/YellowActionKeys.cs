namespace Bombino.scripts.factories;

using Godot.Collections;
using ui;

public class YellowActionKeys : IActionKeys
{
    static YellowActionKeys() => PlayerActionKeysFactory.RegisterInstance(PlayerColor.Yellow, new YellowActionKeys());

    public Array<string> CreateActionKeys() => ActionKeysContainer.ActionItems[10..15];
}
