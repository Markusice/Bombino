using Godot;

namespace Bombino.ui.components.back_button;

internal partial class BackButton : Button
{
    #region Fields

    [Export(PropertyHint.File, "*.tscn")]
    private string PreviousScenePath { get; set; }

    #endregion

    #region MethodsForSignals

    private void OnPressed()
    {
        GetTree().ChangeSceneToFile(PreviousScenePath);
    }

    #endregion
}
