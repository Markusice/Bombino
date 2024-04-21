using Godot;
using System.Linq;
using Bombino.game;

namespace Bombino.ui.rounds_menu
{
    internal partial class RoundStats : CanvasLayer
    {
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


        public override void _Ready()
        {
            _roundLabel = GetNode<Label>("TitleContainer/Title");

            _playerXPosition = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXPosition");
            _playerXName = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXName");
            _playerXWon = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXWon");

            _playerXPosition2 = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXPosition2");
            _playerXName2 = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXName2");
            _playerXWon2 = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXWon2");

            _playerXPosition3 = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXPosition3");
            _playerXName3 = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXName3");
            _playerXWon3 = GetNode<Label>("PanelContainer/MarginContainer/GridContainer/PlayerXWon3");

            UpdatePlayerStats();
        }

        private void UpdatePlayerStats()
        {

            _roundLabel.Text = $"Round {GameManager.CurrentRound}";

            var playerData = GameManager.PlayersData.OrderByDescending(p => p.Wins).ToList();

            _playerXPosition.Text = "1st";
            _playerXName.Text = playerData[0].Color.ToString();
            _playerXWon.Text = playerData[0].Wins.ToString();

            _playerXPosition2.Text = "2nd";
            _playerXName2.Text = playerData[1].Color.ToString();
            _playerXWon2.Text = playerData[1].Wins.ToString();

            if (playerData.Count < 3)
            {
                _playerXPosition3.Visible = false;
                _playerXName3.Visible = false;
                _playerXWon3.Visible = false;
                return;
            }

            _playerXPosition3.Text = "3rd";
            _playerXName3.Text = playerData[2].Color.ToString();
            _playerXWon3.Text = playerData[2].Wins.ToString();
        }

    }
}