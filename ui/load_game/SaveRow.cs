using Godot;

namespace Bombino.ui.load_game;

internal partial class SaveRow : HBoxContainer
{
    public SaveRow(string saveFileName)
    {
        Set("theme_override_constants/separation", 26);

        var saveNameLabel = new Label { Text = saveFileName, ThemeTypeVariation = "SaveLabel" };
        SetLabelProperties(saveNameLabel);

        var selectButton = new Button { Text = "Select", ThemeTypeVariation = "PrimaryButton" };
        SetButtonThemeOverrides(selectButton);

        var deleteButton = new Button { Text = "Delete", ThemeTypeVariation = "SecondaryButton" };
        SetButtonThemeOverrides(deleteButton);

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
}
