using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Bombino.player;

namespace Bombino.game.persistence.storage_layers.key_binds.factory;

internal static class PlayerActionKeysFactory
{
    static PlayerActionKeysFactory()
    {
        LoadIActionKeysClasses();
    }

    private static readonly Dictionary<PlayerColor, IActionKeys> Instances = new();

    public static void RegisterInstance(PlayerColor color, IActionKeys instance)
    {
        Instances.Add(color, instance);
    }

    public static IActionKeys GetInstance(PlayerColor color)
    {
        return Instances[color];
    }

    private static void LoadIActionKeysClasses()
    {
        // load assembly
        var assembly = Assembly.GetExecutingAssembly();

        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsClass)
                continue;

            if (type.Namespace is null)
                continue;

            if (!type.Namespace.Equals("Bombino.game.persistence.storage_layers.key_binds.factory"))
                continue;

            if (!typeof(IActionKeys).IsAssignableFrom(type))
                continue;

            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
    }
}