using Godot;

namespace Bombino.ui.settings_menu;

/// <summary>
/// Provides methods to create action keys for an <see cref="ActionKeysContainer"/>.
/// </summary>
internal static class ActionKeysMapper
{
    /// <summary>
    /// Creates action keys for the specified <paramref name="actionKeysContainer"/>.
    /// </summary>
    /// <param name="actionKeysContainer">The action keys container to create action keys for.</param>
    public static void CreateActionKeys(ActionKeysContainer actionKeysContainer)
    {
        foreach (var actionItem in ActionKeysContainer.ActionItems)
        {
            var remapButton = CreateRemapButton(actionItem);
            var settingKeyLabel = CreateSettingKey(actionItem);

            actionKeysContainer.AddChild(settingKeyLabel);
            actionKeysContainer.AddChild(remapButton);
        }
    }

    /// <summary>
    /// Represents a button used for remapping actions.
    /// </summary>
    private static RemapButton CreateRemapButton(string actionItem)
    {
        var remapButton = new RemapButton();

        remapButton.SetActionKey(actionItem);
        remapButton.SizeFlagsHorizontal = Control.SizeFlags.Expand;

        return remapButton;
    }

    /// <summary>
    /// Represents a control that displays a single line of read-only text.
    /// </summary>
    private static Label CreateSettingKey(string actionItem)
    {
        var settingKeyLabel = new Label
        {
            Text = actionItem.Capitalize(),
            ThemeTypeVariation = "SettingLabel",
            HorizontalAlignment = HorizontalAlignment.Center
        };

        return settingKeyLabel;
    }
}