using Godot;

namespace Bombino.ui.play_menu;

internal partial class NewGameButton : Button
{
    #region Fields

    [Export(PropertyHint.File, "*.tscn")]
    private string NewGameScenePath { get; set; }

    #endregion

    #region MethodsForSignals

    private void OnPressed()
    {
        GetTree().ChangeSceneToFile(NewGameScenePath);
    }

    #endregion
}
