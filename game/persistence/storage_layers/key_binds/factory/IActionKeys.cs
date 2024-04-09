using Godot.Collections;

namespace Bombino.game.persistence.storage_layers.key_binds.factory;

internal interface IActionKeys
{
    Array<string> CreateActionKeys();
}