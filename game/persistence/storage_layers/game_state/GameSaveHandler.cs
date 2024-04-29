using Bombino.game.persistence.state_storage;
using Godot;

namespace Bombino.game.persistence.storage_layers.game_state;

internal class GameSaveHandler : IGameSaveHandler
{
    #region Fields

    private readonly IGameSaver<Godot.Collections.Dictionary<string, Variant>> _gameSaver;

    #endregion

    public GameSaveHandler(IGameSaver<Godot.Collections.Dictionary<string, Variant>> gameSaver)
    {
        _gameSaver = gameSaver;
    }

    #region InterfaceMethods

    /// <summary>
    /// Saves the game data.
    /// </summary>
    /// <remarks>
    /// This method creates a dictionary of game data and writes it to a save file using the <see cref="GameSaver.SaveData"/> method.
    /// </remarks>
    public void SaveGame()
    {
        var data = new Godot.Collections.Dictionary<string, Variant>();

        var playersDataObject = CreatePlayersDataObject();
        AddNewObjectToData(data, "PlayersData", playersDataObject);

        var enemiesDataObject = CreateEnemiesDataObject();
        AddNewObjectToData(data, "EnemiesData", enemiesDataObject);

        _gameSaver.SaveData(data);
    }

    #endregion

    /// <summary>
    /// Adds a new row to the data dictionary.
    /// </summary>
    /// <param name="data">The dictionary to add the row to.</param>
    /// <param name="objectName">The name of the row.</param>
    /// <param name="objectData">The dictionary representing the row.</param>
    private static void AddNewObjectToData(IDictionary<string, Variant> data,
        string objectName, Godot.Collections.Dictionary<string, Variant> objectData
    )
    {
        data.Add(objectName, objectData);
    }

    /// <summary>
    /// Creates a dictionary of players' data rows.
    /// </summary>
    /// <returns>A dictionary containing players' data rows.</returns>
    private static Godot.Collections.Dictionary<string, Variant> CreatePlayersDataObject()
    {
        var playersDataObject = new Godot.Collections.Dictionary<string, Variant>();

        foreach (var playerData in GameManager.PlayersData)
        {
            var playerDataObject = CreatePlayerDataObject(playerData);

            AddNewObjectToData(playersDataObject, playerData.Color.ToString(), playerDataObject);
        }

        return playersDataObject;
    }

    /// <summary>
    /// Creates a dictionary of player data to store.
    /// </summary>
    /// <param name="playerData">The player data to be stored.</param>
    /// <returns>A dictionary containing the player data.</returns>
    private static Godot.Collections.Dictionary<string, Variant> CreatePlayerDataObject(PlayerData playerData)
    {
        return new Godot.Collections.Dictionary<string, Variant>()
        {
            { "Position", playerData.Position },
            { "BombRange", playerData.BombRange },
            { "NumberOfPlacedBombs", playerData.NumberOfPlacedBombs },
            { "MaxNumberOfAvailableBombs", playerData.MaxNumberOfAvailableBombs },
            { "IsDead", playerData.IsDead },
            { "Wins", playerData.Wins },
        };
    }

    private static Godot.Collections.Dictionary<string, Variant> CreateEnemiesDataObject()
    {
        var enemiesDataObject = new Godot.Collections.Dictionary<string, Variant>();

        foreach (var enemyData in GameManager.EnemiesData)
        {
            var playerDataObject = CreateEnemyDataObject(enemyData);

            AddNewObjectToData(enemiesDataObject, enemyData.GetInstanceId().ToString(), playerDataObject);
        }

        return enemiesDataObject;
    }

    private static Godot.Collections.Dictionary<string, Variant> CreateEnemyDataObject(EnemyData enemyData)
    {
        return new Godot.Collections.Dictionary<string, Variant>()
        {
            { "Position", enemyData.Position },
        };
    }
}