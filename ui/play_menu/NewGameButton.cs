using Godot;

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
