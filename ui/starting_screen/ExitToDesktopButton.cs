using Bombino.ui.scripts;
using Godot;

namespace Bombino.ui.starting_screen;

/// <summary>
/// Represents a button that allows the user to exit the application and return to the desktop.
/// </summary>
internal partial class ExitToDesktopButton : Button, IUiButton
{
    /// <summary>
    /// Called when the exit to desktop button is pressed.
    /// </summary>
    public void OnPressed()
    {
        GetTree().Quit();
    }
}