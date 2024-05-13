using Godot;

namespace Bombino.ui.components.back_button;

/// <summary>
/// Represents the back button component.
/// </summary>
internal partial class BackButton : Button
{
    #region Fields

    [Export(PropertyHint.File, "*.tscn")]
    private string PreviousScenePath { get; set; }

    #endregion

    #region MethodsForSignals

    /// <summary>
    /// Called when the button is pressed.
    /// </summary>
    private void OnPressed()
    {
        GetTree().ChangeSceneToFile(PreviousScenePath);
    }

    #endregion
}
