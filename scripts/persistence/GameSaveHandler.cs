namespace Bombino.scripts.persistence;

using Godot;
using Godot.Collections;

internal static class GameSaveHandler
{
    public static void SaveGame()
    {
        var data = new Dictionary<string, Variant>();

        foreach (var playerData in GameManager.PlayersData)
        {
            var playerDataToStore = new Dictionary<string, Variant>
            {
                { "BombRange", playerData.BombRange },
                { "ActionKeys", playerData.ActionKeys }
            };

            data.Add(playerData.Color.ToString(), playerDataToStore);
        }

        GameSave.WriteSave(data);
    }

    public static bool IsThereSavedData(out Dictionary<string, Variant> outputData)
    {
        if (GameSave.IsSaveExits())
        {
            GameSave.LoadSave();
            outputData = GameSave.Data;

            return true;
        }

        outputData = GameSave.Data;

        return false;
    }
}
