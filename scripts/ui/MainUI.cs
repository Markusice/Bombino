namespace Bombino.scripts.ui;

using Godot;

internal partial class MainUI : CanvasLayer
{
    #region Exports

    [Export]
    private PackedScene _bombStatusContainerScene;

    [Export]
    private PackedScene _playerNameContainerScene;

    #endregion

    internal Label TimerLabel { get; private set; }
    internal GridContainer PlayersBombsData { get; private set; }

    public override void _Ready()
    {
        SetUiFields();
    }

    private void SetUiFields()
    {
        PlayersBombsData = GetNode<GridContainer>("PlayerBombsContainer/MarginContainer/PlayersBombsData");
        TimerLabel = GetNode<Label>("TimerPanelContainer/TimerPanel/TimerLabel");

        CreatePlayerBombsData("Player 1", 3);
    }

    private void CreatePlayerBombsData(string playerName, int bombCount)
    {
        var bombStatusContainer = _bombStatusContainerScene.Instantiate<PanelContainer>();
        var playerNameContainer = _playerNameContainerScene.Instantiate<MarginContainer>();

        var bombNumberLabel = bombStatusContainer.GetNode<Label>("BombPicture/BombNumberCircle/BombNumberLabel");
        bombNumberLabel.Text = bombCount.ToString();

        var playerNameLabel = playerNameContainer.GetNode<Label>("PlayerNameLabel");
        playerNameLabel.Text = playerName;

        PlayersBombsData.AddChild(bombStatusContainer);
        PlayersBombsData.AddChild(playerNameContainer);
    }
}