using Bombino.events;
using Godot;

namespace Bombino.ui.main_ui.animation_scripts;

internal partial class PlayerKillAnimation : Node
{
    #region Fields

    private const float TweenDuration = 0.2f;

    private GridContainer PlayersBombData { get; set; }

    #endregion

    #region Overrides

    public override void _Ready()
    {
        PlayersBombData = GetNode<GridContainer>("%PlayersBombData");

        Events.Instance.PlayerDied += OnPlayerDied;
    }

    protected override void Dispose(bool disposing)
    {
        Events.Instance.PlayerDied -= OnPlayerDied;

        base.Dispose(disposing);
    }

    #endregion

    #region MethodsForSignals

    private void OnPlayerDied(string playerColor)
    {
        var playerNameContainerName = $"PlayerNameContainer_{playerColor}";
        var playerNameContainer = PlayersBombData.GetNode<MarginContainer>(playerNameContainerName);

        var playerNameLabel = playerNameContainer.GetNode<Label>("PlayerNameLabel");

        var bombStatusContainerName = $"BombStatusContainer_{playerColor}";
        var bombStatusContainer = PlayersBombData.GetNode<PanelContainer>(bombStatusContainerName);

        var bombPicture = bombStatusContainer.GetNode<TextureRect>("BombPicture");
        var bombNumberLabel = bombStatusContainer.GetNode<Label>("%BombNumberLabel");

        PlayAnimationForPlayerKill(playerNameLabel, bombPicture);

        bombNumberLabel.Text = 0.ToString();
    }

    #endregion

    private void PlayAnimationForPlayerKill(Label playerNameLabel, TextureRect bombPicture)
    {
        var tween = CreateTween()
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.In);

        tween.TweenMethod(Callable.From<Color>(value => SetFontColor(value, playerNameLabel)),
            Colors.White, Colors.DarkRed,
            TweenDuration);

        tween.Parallel().TweenMethod(Callable.From<int>(value => SetFontOutlineSize(value, playerNameLabel)),
            0, 2,
            TweenDuration);

        tween.Parallel().TweenProperty(bombPicture, "modulate",
            new Color(1, 1, 1, 0.4f),
            TweenDuration);
    }

    private static void SetFontColor(Color value, Label label) =>
        label.Set("theme_override_colors/font_color", value);

    private static void SetFontOutlineSize(int value, Label label) =>
        label.Set("theme_override_constants/outline_size", value);
}