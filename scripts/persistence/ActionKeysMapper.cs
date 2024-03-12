using Godot;

public class ActionKeysMapper
{
    public static void CreateActionKeys(ActionKeysContainer actionKeysContainer)
    {
        foreach (var actionItem in actionKeysContainer.ActionItems)
        {
            var remapButton = new RemapButton();
            remapButton.SetActionKey(actionItem);
            remapButton.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;

            var settingKey = new Label();
            settingKey.Text = actionItem;
            settingKey.ThemeTypeVariation = "SettingLabel";
            settingKey.HorizontalAlignment = HorizontalAlignment.Center;

            actionKeysContainer.AddChild(settingKey);
            actionKeysContainer.AddChild(remapButton);
        }
    }
}