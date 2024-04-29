using Bombino.game;
using Godot;

namespace Bombino.ui.game_loading_screen;

internal partial class GameLoadingScene : CanvasLayer
{
    #region Fields

    private GameManager GameManager { get; set; }

    #endregion

    #region Overrides

    public override void _Ready()
    {
        var mainNode = GetParent().GetNode<Node>("Main");
        GameManager = mainNode.GetNode<GameManager>("WorldEnvironment");

        AttachMethodsForSignals();

        GameManager.CreateNewGame();
    }

    protected override void Dispose(bool disposing)
    {
        DetachMethodsForSignals();

        base.Dispose(disposing);
    }

    #endregion

    #region MethodsForSignals

    private void OnSceneLoad(double progress)
    {
        var progressLabel = GetNode<Label>("PanelContainer/ProgressLabel");
        progressLabel.Text = $"{Mathf.Floor(progress * 100)}%";
    }

    private void OnEverythingLoaded()
    {
        QueueFree();
    }

    #endregion

    private void AttachMethodsForSignals()
    {
        GameManager.SceneLoad += OnSceneLoad;
        GameManager.EverythingLoaded += OnEverythingLoaded;
    }

    private void DetachMethodsForSignals()
    {
        GameManager.SceneLoad -= OnSceneLoad;
        GameManager.EverythingLoaded -= OnEverythingLoaded;
    }
}