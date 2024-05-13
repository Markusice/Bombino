using Bombino.game.persistence.storage_layers.key_binds;
using Bombino.player;
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
    /// <param name="inputActionsData"></param>
    public static void CreateActionKeys(
        ActionKeysContainer actionKeysContainer,
        Dictionary<string, PlayerColor> inputActionsData
    )
    {
        foreach (var (inputAction, _) in inputActionsData)
        {
            var remapButton = CreateRemapButton(inputAction);
            var settingKeyLabel = CreateSettingKey(inputAction);

            actionKeysContainer.AddChild(settingKeyLabel);
            actionKeysContainer.AddChild(remapButton);
        }
    }

    /// <summary>
    /// Represents a button used for remapping actions.
    /// </summary>
    private static RemapButton CreateRemapButton(string actionKey)
    {
        var remapButton = new RemapButton();

        remapButton.SetActionKey(actionKey);
        remapButton.SizeFlagsHorizontal = Control.SizeFlags.Expand;

        return remapButton;
    }

    /// <summary>
    /// Represents a control that displays a single line of read-only text.
    /// </summary>
    private static Label CreateSettingKey(string actionKey)
    {
        var settingKeyLabel = new Label
        {
            Text = actionKey.Capitalize(),
            ThemeTypeVariation = "SettingLabel",
            HorizontalAlignment = HorizontalAlignment.Center
        };

        return settingKeyLabel;
    }
}
