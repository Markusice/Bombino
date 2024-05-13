using Bombino.file_system_helpers.directory;
using Bombino.game.persistence.storage_layers.game_state;
using Godot;

namespace Bombino.ui.load_game;

/// <summary>
/// Represents the save row scene.
/// </summary>
internal partial class SaveRow : HBoxContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveRow"/> class.
    /// </summary>
    /// <param name="fileName">The name of the save file.</param>
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

    /// <summary>
    /// Sets the properties of the label.
    /// </summary>
    /// <param name="label">The label to set the properties for.</param>
    private static void SetLabelProperties(Label label)
    {
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.ClipText = true;
        label.CustomMinimumSize = new Vector2(600, 0);
    }

    /// <summary>
    /// Sets the theme overrides for the button.
    /// </summary>
    /// <param name="button">The button to set the theme overrides for.</param>
    private static void SetButtonThemeOverrides(Button button)
    {
        button.Set("theme_override_constants/outline_size", 0);
        button.Set("theme_override_font_sizes/font_size", 28);
    }

    /// <summary>
    /// Deletes the save file.
    /// </summary>
    private void DeleteSave(string saveFileName)
    {
        var error = new DirAccessManager().RemoveFileAbsolute(GetFileAbsolutePath(saveFileName));
        if (error != Error.Ok)
        {
            return;
        }

        QueueFree();
    }

    /// <summary>
    /// Gets the absolute path of the save file.
    /// </summary>
    /// <param name="saveFileName">The name of the save file.</param>
    /// <returns>The absolute path of the save file.</returns>
    private static string GetFileAbsolutePath(string saveFileName)
    {
        return $"{OS.GetUserDataDir()}/{SaveDirectory.Path}/{saveFileName}";
    }
}
