using Bombino.game.persistence.state_storage;

namespace Bombino.game.persistence.storage_layers.game_state;

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

    /// <summary>
    /// Checks if there is saved data and retrieves it.
    /// </summary>
    /// <param name="outputData">The dictionary containing the saved game data.</param>
    /// <returns><c>true</c> if saved data exists, <c>false</c> otherwise.</returns>
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

    /// <summary>
    /// Adds a new row to the data dictionary.
    /// </summary>
    /// <param name="data">The dictionary to add the row to.</param>
    /// <param name="rowName">The name of the row.</param>
    /// <param name="row">The dictionary representing the row.</param>
    private static void AddNewRowToData(
        Dictionary<string, Variant> data,
        string rowName,
        Dictionary<string, Variant> row
    )
    {
        data.Add(rowName, row);
    }

    /// <summary>
    /// Creates a dictionary of players' data rows.
    /// </summary>
    /// <returns>A dictionary containing players' data rows.</returns>
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

    /// <summary>
    /// Creates a dictionary of player data to store.
    /// </summary>
    /// <param name="playerData">The player data to be stored.</param>
    /// <returns>A dictionary containing the player data.</returns>
    private static Dictionary<string, Variant> CreatePlayerDataToStore(PlayerData playerData)
    {
        return new Dictionary<string, Variant>
        {
            { "BombRange", playerData.BombRange },
        };
    }
}