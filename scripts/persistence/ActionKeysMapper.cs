namespace Bombino.scripts.persistence;

using Godot;
using ui;

internal static class ActionKeysMapper
{
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

    private static RemapButton CreateRemapButton(string actionItem)
    {
        var remapButton = new RemapButton();

        remapButton.SetActionKey(actionItem);
        remapButton.SizeFlagsHorizontal = Control.SizeFlags.Expand;

        return remapButton;
    }

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
