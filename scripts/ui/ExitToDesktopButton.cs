using Godot;

public partial class ExitToDesktopButton : Button
{
    private void OnPressed()
    {
        GetTree().Quit();
    }
}
