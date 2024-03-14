using Godot;

public class ActionKeysMapper
{
    public static void CreateActionKeys(ActionKeysContainer actionKeysContainer)
    {
        foreach (var actionItem in actionKeysContainer.ActionItems)
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
        var settingKeyLabel = new Label();

        settingKeyLabel.Text = actionItem.Capitalize();
        settingKeyLabel.ThemeTypeVariation = "SettingLabel";
        settingKeyLabel.HorizontalAlignment = HorizontalAlignment.Center;

        return settingKeyLabel;
    }
}