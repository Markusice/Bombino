namespace Bombino.scripts.ui;

using System;
using Godot;
using persistence;

internal partial class MainUI : CanvasLayer
{
    #region Exports

    [Export]
    private PackedScene _bombStatusContainerScene;

    [Export]
    private PackedScene _playerNameContainerScene;

    #endregion

    #region Signals

    [Signal]
    public delegate void PlayerBombNumberChangedEventHandler(PlayerData playerData);

    #endregion

    public Label TimerLabel { get; private set; }
    public GridContainer PlayersBombsData { get; private set; }

    private PanelContainer _bombStatusContainer;
    private Label _bombNumberLabel;

    public override void _Ready()
    {
        SetUiFields();

        SetTimerLabelText(60);
    }

    private void SetUiFields()
    {
        PlayersBombsData = GetNode<GridContainer>(
            "PlayerBombsContainer/MarginContainer/PlayersBombsData"
        );
        TimerLabel = GetNode<Label>("TimerPanelContainer/TimerPanel/TimerLabel");
        _bombStatusContainer = _bombStatusContainerScene.Instantiate<PanelContainer>();

        foreach (var playerData in GameManager.PlayersData)
        {
            CreatePlayerBombsData(playerData);
        }
    }

    private void SetTimerLabelText(int timeInSeconds)
    {
        TimerLabel.Text = TimeSpan.FromSeconds(timeInSeconds).ToString(@"m\:ss");
    }

    private void CreatePlayerBombsData(PlayerData playerData)
    {
        SetUpBombStatusContainer(playerData);
        var playerNameContainer = SetUpPlayerNameContainer(playerData);

        PlayersBombsData.AddChild(_bombStatusContainer);
        PlayersBombsData.AddChild(playerNameContainer);
    }

    private void SetUpBombStatusContainer(PlayerData playerData)
    {
        _bombNumberLabel = _bombStatusContainer.GetNode<Label>(
            "BombPicture/BombNumberCircle/BombNumberLabel"
        );

        _bombNumberLabel.Text = playerData.NumberOfAvailableBombs.ToString();
    }

    private MarginContainer SetUpPlayerNameContainer(PlayerData playerData)
    {
        var playerNameContainer = _playerNameContainerScene.Instantiate<MarginContainer>();

        var playerNameLabel = playerNameContainer.GetNode<Label>("PlayerNameLabel");
        playerNameLabel.Text = playerData.Color.ToString();

        return playerNameContainer;
    }

    private void ChangePlayerBombNumberAndOpacity(PlayerData playerData)
    {
        if (playerData.NumberOfAvailableBombs == 0)
        {
            _bombNumberLabel.Text = 0.ToString();
            _bombNumberLabel.SelfModulate = new Color(1, 1, 1, 0.6f);

            return;
        }

        _bombNumberLabel.SelfModulate = new Color(1, 1, 1, 1);
        _bombNumberLabel.Text = playerData.NumberOfAvailableBombs.ToString();
    }

    private void OnPlayerBombNumberChanged(PlayerData playerData)
    {
        ChangePlayerBombNumberAndOpacity(playerData);
    }

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
