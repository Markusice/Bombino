namespace Bombino.scripts.factories;

using Godot.Collections;
using ui;

internal class BlueActionKeys : IActionKeys
{
    static BlueActionKeys() => PlayerActionKeysFactory.RegisterInstance(PlayerColor.Blue, new BlueActionKeys());

    public Array<string> CreateActionKeys() => ActionKeysContainer.ActionItems[..5];
}
