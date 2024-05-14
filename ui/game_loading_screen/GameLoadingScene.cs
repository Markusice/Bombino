using Bombino.game;
using Godot;

namespace Bombino.ui.game_loading_screen;

/// <summary>
/// Represents the game loading scene.
/// </summary>
internal partial class GameLoadingScene : CanvasLayer
{
    #region Fields

    private GameManager GameManager { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        var mainNode = GetParent().GetNode<Node>("Main");
        GameManager = mainNode.GetNode<GameManager>("WorldEnvironment");

        AttachMethodsForSignals();

        GameManager.CreateNewGame();
    }

    /// <summary>
    /// Called when the node is removed from the scene tree.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        DetachMethodsForSignals();

        base.Dispose(disposing);
    }

    #endregion

    #region MethodsForSignals

    /// <summary>
    /// Called when the scene is loading.
    /// </summary>
    /// <param name="progress">The progress of the scene loading.</param>
    private void OnSceneLoad(double progress)
    {
        var progressLabel = GetNode<Label>("PanelContainer/ProgressLabel");
        progressLabel.Text = $"{Mathf.Floor(progress * 100)}%";
    }

    /// <summary>
    /// Called when everything is loaded.
    /// </summary>
    private void OnEverythingLoaded()
    {
        QueueFree();
    }

    #endregion

    /// <summary>
    /// Attaches methods for signals.
    /// </summary>
    private void AttachMethodsForSignals()
    {
        GameManager.SceneLoad += OnSceneLoad;
        GameManager.EverythingLoaded += OnEverythingLoaded;
    }

    /// <summary>
    /// Detaches methods for signals.
    /// </summary>
    private void DetachMethodsForSignals()
    {
        GameManager.SceneLoad -= OnSceneLoad;
        GameManager.EverythingLoaded -= OnEverythingLoaded;
    }
}
