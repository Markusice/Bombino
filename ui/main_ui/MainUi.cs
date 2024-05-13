using Bombino.game;
using Godot;
using PlayerData = Bombino.game.persistence.state_resources.PlayerData;

namespace Bombino.ui.main_ui;

/// <summary>
/// Represents the main user interface for the game.
/// </summary>
internal partial class MainUi : CanvasLayer
{
    #region Exports

    [Export(PropertyHint.File, "*.tscn")] private PackedScene _bombStatusContainerScene;

    [Export(PropertyHint.File, "*.tscn")] private PackedScene _playerNameContainerScene;

    #endregion

    #region Fields

    private Label TimerLabel { get; set; }
    private GridContainer PlayersBombData { get; set; }

    #endregion

    #region Overrides

    public override void _Ready()
    {
        SetUiFields();

        SetTimerLabelText(60);
    }

    #endregion

    /// <summary>
    /// Sets up the UI fields by finding and assigning the necessary nodes.
    /// </summary>
    private void SetUiFields()
    {
        TimerLabel = GetNode<Label>("TimerPanelContainer/TimerPanel/TimerLabel");
        PlayersBombData = GetNode<GridContainer>("%PlayersBombData");

        foreach (var playerData in GameManager.PlayersData)
        {
            CreatePlayerBombData(playerData);
        }
    }

    /// <summary>
    /// Sets the text of the timer label based on the given time in seconds.
    /// </summary>
    /// <param name="timeInSeconds">The time in seconds.</param>
    private void SetTimerLabelText(int timeInSeconds)
    {
        TimerLabel.Text = TimeSpan.FromSeconds(timeInSeconds).ToString(@"m\:ss");
    }

    /// <summary>
    /// Creates the bomb status container and player name container for the given player data,
    /// and adds them to the PlayersBombData grid container.
    /// </summary>
    /// <param name="playerData">The player data.</param>
    private void CreatePlayerBombData(PlayerData playerData)
    {
        var bombStatusContainer = SetUpBombStatusContainer(playerData);
        var playerNameContainer = SetUpPlayerNameContainer(playerData);

        PlayersBombData.AddChild(bombStatusContainer);
        PlayersBombData.AddChild(playerNameContainer);
    }

    /// <summary>
    /// Sets up the bomb status container for the given player data.
    /// </summary>
    /// <param name="playerData">The player data.</param>
    /// <returns>The bomb status container.</returns>
    private PanelContainer SetUpBombStatusContainer(PlayerData playerData)
    {
        var bombStatusContainer = _bombStatusContainerScene.Instantiate<PanelContainer>();
        bombStatusContainer.Name = $"BombStatusContainer_{playerData.Color.ToString()}";

        var bombNumberLabel = bombStatusContainer.GetNode<Label>(
            "BombPicture/BombNumberCircle/BombNumberLabel"
        );

        bombNumberLabel.Text = playerData.MaxNumberOfAvailableBombs.ToString();

        return bombStatusContainer;
    }

    /// <summary>
    /// Sets up the player name container for the given player data.
    /// </summary>
    /// <param name="playerData">The player data.</param>
    /// <returns>The player name container.</returns>
    private MarginContainer SetUpPlayerNameContainer(PlayerData playerData)
    {
        var playerNameContainer = _playerNameContainerScene.Instantiate<MarginContainer>();
        playerNameContainer.Name = $"PlayerNameContainer_{playerData.Color.ToString()}";

        var playerNameLabel = playerNameContainer.GetNode<Label>("PlayerNameLabel");
        playerNameLabel.Text = playerData.Color.ToString();

        return playerNameContainer;
    }

    /// <summary>
    /// Event handler for the TimerLabelChanger timeout.
    /// Decreases the timer label value by 1 second.
    /// </summary>
    private void OnTimerLabelChangerTimeout()
    {
        var timerLabelInTotalSeconds = TimeSpan
            .ParseExact(TimerLabel.Text, @"m\:ss", null)
            .TotalSeconds;

        if (!(timerLabelInTotalSeconds > 0)) return;

        timerLabelInTotalSeconds--;
        SetTimerLabelText((int)timerLabelInTotalSeconds);
    }
}