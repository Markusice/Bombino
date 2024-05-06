using Bombino.file_system_helpers.directory;
using Bombino.game.persistence.storage_layers.game_state;
using Godot;

namespace Bombino.ui.load_game;

internal partial class LoadSaves : GridContainer
{
    #region Fields

    private readonly IDirAccessManager _dirAccessManager = new DirAccessManager();

    #endregion

    #region Overrides

    public override void _Ready()
    {
        CreateRowsForSaves();
    }

    #endregion

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

    private static void SortFileNames(string[] fileNames)
    {
        Array.Reverse(fileNames);
    }

    private void AddSaveRows(IEnumerable<string> fileNames)
    {
        foreach (var fileName in fileNames)
        {
            var saveRow = new SaveRow(fileName);
            AddChild(saveRow);
        }
    }
}
