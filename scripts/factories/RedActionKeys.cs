namespace Bombino.scripts.factories;

using Godot.Collections;
using ui;

internal class RedActionKeys : IActionKeys
{
    static RedActionKeys() => PlayerActionKeysFactory.RegisterInstance(PlayerColor.Red, new RedActionKeys());

    public Array<string> CreateActionKeys() => ActionKeysContainer.ActionItems[5..10];
}
