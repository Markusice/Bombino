namespace Bombino.scripts.ui;

using Godot;

internal partial class ExitToDesktopButton : Button, IUiButton
{
    public void OnPressed() => GetTree().Quit();
}