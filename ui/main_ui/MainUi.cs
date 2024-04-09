using System;
using Bombino.game;
using Bombino.game.persistence.state_storage;
using Godot;

namespace Bombino.ui.main_ui;

/// <summary>
/// Represents the main user interface for the game.
/// </summary>
internal partial class MainUi : CanvasLayer
{
    #region Exports

    [Export] private PackedScene _bombStatusContainerScene;

    [Export] private PackedScene _playerNameContainerScene;

    #endregion

    #region Signals

    [Signal]
    public delegate void PlayerBombNumberChangedEventHandler(PlayerData playerData);

    #endregion

    private Label TimerLabel { get; set; }
    private GridContainer PlayersBombsData { get; set; }

    public override void _Ready()
    {
        SetUiFields();

        SetTimerLabelText(60);
    }

    /// <summary>
    /// Sets up the UI fields by finding and assigning the necessary nodes.
    /// </summary>
    private void SetUiFields()
    {
        TimerLabel = GetNode<Label>("TimerPanelContainer/TimerPanel/TimerLabel");

        PlayersBombsData = GetNode<GridContainer>(
            "PlayerBombsContainer/MarginContainer/PlayersBombsData"
        );

        foreach (var playerData in GameManager.PlayersData) CreatePlayerBombsData(playerData);
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
    /// and adds them to the PlayersBombsData grid container.
    /// </summary>
    /// <param name="playerData">The player data.</param>
    private void CreatePlayerBombsData(PlayerData playerData)
    {
        var bombStatusContainer = SetUpBombStatusContainer(playerData);
        var playerNameContainer = SetUpPlayerNameContainer(playerData);

        PlayersBombsData.AddChild(bombStatusContainer);
        PlayersBombsData.AddChild(playerNameContainer);
    }

    /// <summary>
    /// Sets up the bomb status container for the given player data.
    /// </summary>
    /// <param name="playerData">The player data.</param>
    /// <returns>The bomb status container.</returns>
    private PanelContainer SetUpBombStatusContainer(PlayerData playerData)
    {
        var bombStatusContainer = _bombStatusContainerScene.Instantiate<PanelContainer>();
        var bombNumberLabel = bombStatusContainer.GetNode<Label>(
            "BombPicture/BombNumberCircle/BombNumberLabel"
        );

        bombNumberLabel.Text = playerData.NumberOfAvailableBombs.ToString();

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

        var playerNameLabel = playerNameContainer.GetNode<Label>("PlayerNameLabel");
        playerNameLabel.Text = playerData.Color.ToString();

        return playerNameContainer;
    }

    /// <summary>
    /// Changes the player's bomb number and opacity based on the player data.
    /// </summary>
    /// <param name="playerData">The player data.</param>
    private void ChangePlayerBombNumberAndOpacity(PlayerData playerData)
    {
        // TODO: Rewrite this method
        // if (playerData.NumberOfAvailableBombs == 0)
        // {
        //     _bombNumberLabel.Text = 0.ToString();
        //     _bombNumberLabel.SelfModulate = new Color(1, 1, 1, 0.6f);

        //     return;
        // }

        // _bombNumberLabel.SelfModulate = new Color(1, 1, 1, 1);
        // _bombNumberLabel.Text = playerData.NumberOfAvailableBombs.ToString();
    }

    /// <summary>
    /// Event handler for the PlayerBombNumberChanged signal.
    /// </summary>
    /// <param name="playerData">The player data.</param>
    private void OnPlayerBombNumberChanged(PlayerData playerData)
    {
        ChangePlayerBombNumberAndOpacity(playerData);
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

        if (timerLabelInTotalSeconds > 0)
        {
            timerLabelInTotalSeconds--;
            SetTimerLabelText((int)timerLabelInTotalSeconds);
        }
    }
}