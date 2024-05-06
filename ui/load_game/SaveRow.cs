using Bombino.file_system_helpers.directory;
using Bombino.game.persistence.storage_layers.game_state;
using Godot;

namespace Bombino.ui.load_game;

internal partial class SaveRow : HBoxContainer
{
    public SaveRow(string fileName)
    {
        Set("theme_override_constants/separation", 26);

        var baseFileName = fileName.GetBaseName();
        var saveNameLabel = new Label { Text = baseFileName, ThemeTypeVariation = "SaveLabel" };
        SetLabelProperties(saveNameLabel);

        var selectButton = new Button { Text = "Select", ThemeTypeVariation = "PrimaryButton" };
        SetButtonThemeOverrides(selectButton);
        // TODO: selectButton.Pressed += LoadSave

        var deleteButton = new Button { Text = "Delete", ThemeTypeVariation = "SecondaryButton" };
        SetButtonThemeOverrides(deleteButton);
        deleteButton.Pressed += () => DeleteSave(fileName);

        AddChild(saveNameLabel);
        AddChild(selectButton);
        AddChild(deleteButton);
    }

    private static void SetLabelProperties(Label label)
    {
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.ClipText = true;
        label.CustomMinimumSize = new Vector2(600, 0);
    }

    private static void SetButtonThemeOverrides(Button button)
    {
        button.Set("theme_override_constants/outline_size", 0);
        button.Set("theme_override_font_sizes/font_size", 28);
    }

    private void DeleteSave(string saveFileName)
    {
        var error = new DirAccessManager().RemoveFileAbsolute(GetFileAbsolutePath(saveFileName));
        if (error != Error.Ok)
        {
            return;
        }

        QueueFree();
    }

    private static string GetFileAbsolutePath(string saveFileName)
    {
        return $"{OS.GetUserDataDir()}/{SaveDirectory.Path}/{saveFileName}";
    }
}
