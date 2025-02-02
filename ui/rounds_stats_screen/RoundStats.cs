using Bombino.game;
using Bombino.ui.scripts;
using Godot;

namespace Bombino.ui.rounds_stats_screen;

/// <summary>
/// Represents a user interface component for displaying the stats of a round.
/// </summary>
internal partial class RoundStats : CanvasLayer
{
    #region Exports

    [Export(PropertyHint.File, "*.tscn")]
    private string StartingScreenPath { get; set; }

    #endregion

    #region Fields

    private GameManager _gameManager;

    private Label _roundLabel;

    private Label _playerXPosition;
    private Label _playerXName;
    private Label _playerXWon;

    private Label _playerXPosition2;
    private Label _playerXName2;
    private Label _playerXWon2;

    private Label _playerXPosition3;
    private Label _playerXName3;
    private Label _playerXWon3;

    #endregion

    #region Overrides

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        _gameManager = GetParent().GetNode<GameManager>("WorldEnvironment");

        _roundLabel = GetNode<Label>("TitleContainer/Title");

        _playerXPosition = GetNode<Label>(
            "PanelContainer/MarginContainer/GridContainer/PlayerXPosition"
        );
        _playerXName = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXName");
        _playerXWon = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXWon");

        _playerXPosition2 = GetNode<Label>(
            "PanelContainer/MarginContainer/GridContainer/PlayerXPosition2"
        );
        _playerXName2 = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXName2");
        _playerXWon2 = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXWon2");

        _playerXPosition3 = GetNode<Label>(
            "PanelContainer/MarginContainer/GridContainer/PlayerXPosition3"
        );
        _playerXName3 = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXName3");
        _playerXWon3 = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXWon3");

        UpdatePlayerStats();
    }

    /// <summary>
    /// Called when the node receives an input event.
    /// </summary>
    /// <param name="event">The input event received.</param>
    public override void _Input(InputEvent @event)
    {
        if (!InputEventChecker.IsEnterKeyPressed(@event))
            return;

        OnContinuePressed();
    }

    #endregion

    /// <summary>
    /// Updates the player stats on the screen.
    /// </summary>
    private void UpdatePlayerStats()
    {
        UpdateRoundLabel();

        UpdatePlayerLabels();
    }

    /// <summary>
    /// Updates the round label with the current round and winner.
    /// </summary>
    private void UpdateRoundLabel()
    {
        if (GameManager.CurrentRound < GameManager.NumberOfRounds)
        {
            if (GameManager.CurrentWinner == null)
            {
                _roundLabel.Text = $"Round {GameManager.CurrentRound} - Draw!";
                return;
            }
            _roundLabel.Text =
                $"Round {GameManager.CurrentRound} - {GameManager.CurrentWinner} won";
            return;
        }
        var maxWins = GameManager.PlayersData.Max(p => p.Wins);
        var winners = GameManager.PlayersData.Where(p => p.Wins == maxWins).ToList();

        if (winners.Count > 1)
        {
            _roundLabel.Text = "Game Over! - Draw!";
            return;
        }
        _roundLabel.Text = $"Game Over! - {winners[0].Color} won!";
    }

    /// <summary>
    /// Updates the player labels with the current player stats.
    /// </summary>
    private void UpdatePlayerLabels()
    {
        var playersData = GameManager.PlayersData.OrderByDescending(p => p.Wins).ToList();

        _playerXPosition.Text = "1st";
        _playerXName.Text = playersData[0].Color.ToString();
        _playerXWon.Text = playersData[0].Wins.ToString();

        _playerXPosition2.Text = "2nd";
        _playerXName2.Text = playersData[1].Color.ToString();
        _playerXWon2.Text = playersData[1].Wins.ToString();

        if (playersData.Count < 3)
        {
            _playerXPosition3.Visible = false;
            _playerXName3.Visible = false;
            _playerXWon3.Visible = false;
            return;
        }

        _playerXPosition3.Text = "3rd";
        _playerXName3.Text = playersData[2].Color.ToString();
        _playerXWon3.Text = playersData[2].Wins.ToString();
    }

    /// <summary>
    /// Called when the continue button is pressed.
    /// </summary>
    private void OnContinuePressed()
    {
        QueueFree();
        if (GameManager.CurrentRound == GameManager.NumberOfRounds)
        {
            _gameManager.GameOver();
            GetTree().ChangeSceneToFile(StartingScreenPath);
            return;
        }

        _gameManager.StartNextRound();
    }
}
