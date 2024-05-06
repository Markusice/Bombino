using Godot;

internal partial class LoadGameButton : Button
{
    #region Fields

    [Export(PropertyHint.File, "*.tscn")]
    private string LoadGameScenePath { get; set; }

    #endregion

    #region MethodsForSignals

    private void OnPressed()
    {
        GetTree().ChangeSceneToFile(LoadGameScenePath);
    }

    #endregion
}
