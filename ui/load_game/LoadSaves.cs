using Bombino.file_system_helpers.directory;
using Bombino.game.persistence.storage_layers.game_state;
using Godot;

namespace Bombino.ui.load_game;

/// <summary>
/// Represents the load saves scene.
/// </summary>
internal partial class LoadSaves : GridContainer
{
    #region Fields

    private readonly IDirAccessManager _dirAccessManager = new DirAccessManager();

    #endregion

    #region Overrides

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        CreateRowsForSaves();
    }

    #endregion

    /// <summary>
    /// Creates rows for the saves.
    /// </summary>
    private void CreateRowsForSaves()
    {
        var (error, fileNames) = _dirAccessManager.GetFileNames($"user://{SaveDirectory.Path}");
        if (error != Error.Ok)
        {
            return;
        }

        SortFileNames(fileNames);

        AddSaveRows(fileNames);
    }

    /// <summary>
    /// Sorts the file names in reverse order.
    /// </summary>
    private static void SortFileNames(string[] fileNames)
    {
        Array.Reverse(fileNames);
    }

    /// <summary>
    /// Adds save rows to the grid container.
    /// </summary>
    private void AddSaveRows(IEnumerable<string> fileNames)
    {
        foreach (var fileName in fileNames)
        {
            var saveRow = new SaveRow(fileName);
            AddChild(saveRow);
        }
    }
}
