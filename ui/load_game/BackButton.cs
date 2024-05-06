using Godot;

internal partial class BackButton : Button
{
    #region Fields

    [Export(PropertyHint.File, "*.tscn")]
    private string PlayMenuScenePath { get; set; }

    #endregion

    #region MethodsForSignals

    private void OnPressed()
    {
        GetTree().ChangeSceneToFile(PlayMenuScenePath);
    }

    #endregion
}
