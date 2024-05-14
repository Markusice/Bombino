using Godot;

namespace Bombino.ui.players_menu;

internal abstract partial class PlayerButton : Button
{
    #region Exports

    [Export(PropertyHint.File, "*.tscn")]
    private string MapsMenuPath { get; set; }

    #endregion

    #region MethodsForSignals

    /// <summary>
    /// Event handler for the button press event.
    /// </summary>
    protected void _OnPressed()
    {
        GetTree().ChangeSceneToFile(MapsMenuPath);
    }

    #endregion
}
