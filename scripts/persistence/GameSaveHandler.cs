namespace Bombino.scripts.persistence;

using Godot;
using Godot.Collections;

internal static class GameSaveHandler
{
    /// <summary>
    /// Saves the game data.
    /// </summary>
    /// <remarks>
    /// This method creates a dictionary of game data and writes it to a save file using the <see cref="GameSave.WriteSave"/> method.
    /// </remarks>
    public static void SaveGame()
    {
        var data = new Dictionary<string, Variant>();

        var playersDataRows = CreatePlayersDataRows();

        AddNewRowToData(data, "PlayersData", playersDataRows);

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

    private static void AddNewRowToData(
        Dictionary<string, Variant> data,
        string rowName,
        Dictionary<string, Variant> row
    )
    {
        data.Add(rowName, row);
    }

    private static Dictionary<string, Variant> CreatePlayersDataRows()
    {
        var playersDataRows = new Dictionary<string, Variant>();

        foreach (var playerData in GameManager.PlayersData)
        {
            var playerDataToStore = CreatePlayerDataToStore(playerData);

            AddNewRowToData(playersDataRows, playerData.Color.ToString(), playerDataToStore);
        }

        return playersDataRows;
    }

    private static Dictionary<string, Variant> CreatePlayerDataToStore(PlayerData playerData)
    {
        return new Dictionary<string, Variant>
        {
            { "BombRange", playerData.BombRange },
            { "ActionKeys", playerData.ActionKeys }
        };
    }
}
