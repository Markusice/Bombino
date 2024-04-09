using Bombino.game;
using Godot;

namespace Bombino.ui.game_loading_screen;

internal partial class GameLoadingScene : CanvasLayer
{
    private void OnMainChildEnteredTree(Node node)
    {
        if (node is not WorldEnvironment worldEnvironment)
            return;

        var gameManager = (GameManager)worldEnvironment;

        gameManager.SceneLoad += OnSceneLoad;
        gameManager.EverythingLoaded += OnEverythingLoaded;

        gameManager.CreateNewGame();
    }

    private void OnSceneLoad(double progress)
    {
        var progressLabel = GetNode<Label>("PanelContainer/ProgressLabel");
        progressLabel.Text = $"{Mathf.Floor(progress * 100)}%";
    }

    private void OnEverythingLoaded()
    {
        QueueFree();
    }
}