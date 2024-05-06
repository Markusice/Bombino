using Bombino.file_system_helpers.directory;
using Godot;

namespace Bombino.ui.load_game;

internal partial class LoadSaves : GridContainer
{
    #region Fields

    private readonly IDirAccessManager _dirAccessManager = new DirAccessManager();

    #endregion

    public override void _Ready()
    {
        CreateRowsForSaves();
    }

    private void CreateRowsForSaves()
    {
        var fileNames = _dirAccessManager.GetFileNames("user://saves");
        var error = fileNames.Item1;
        if (error != Error.Ok)
        {
            return;
        }

        var saveFileNames = fileNames.Item2;
        SortFileNames(saveFileNames);

        AddSaveRows(saveFileNames);
    }

    private static void SortFileNames(string[] fileNames)
    {
        Array.Reverse(fileNames);
    }

    private void AddSaveRows(string[] saveFileNames)
    {
        foreach (var saveFileName in saveFileNames)
        {
            var saveRow = CreateSaveRow(saveFileName);
            AddChild(saveRow);
        }
    }

    private static SaveRow CreateSaveRow(string saveFileName)
    {
        var baseFileName = saveFileName.GetBaseName();
        var saveRow = new SaveRow(baseFileName);

        return saveRow;
    }
}
