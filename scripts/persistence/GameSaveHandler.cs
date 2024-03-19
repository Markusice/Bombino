namespace Bombino.scripts.persistence;

using Godot;
using Godot.Collections;

// responsible for saving and loading the game using GameSave.cs
internal static class GameSaveHandler
{
    public static Dictionary<string, Variant> GetDataFromGameSave()
    {
        return GameSave.Data;
    }

    // foreach (var keyValuePair in data)
    // {
    //     GD.Print(keyValuePair.Key);
    //     GD.Print(Enum.Parse<PlayerColor>(keyValuePair.Key));
    // }
}