using Bombino.events;
using Godot;

namespace Bombino.ui.main_ui.animation_scripts;

/// <summary>
/// Represents the player kill animation scene.
/// </summary>
internal partial class PlayerKillAnimation : Node
{
    #region Fields

    private const float TweenDuration = 0.2f;

    private GridContainer PlayersBombData { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        PlayersBombData = GetNode<GridContainer>("%PlayersBombData");

        Events.Instance.PlayerDied += OnPlayerDied;
    }

    /// <summary>
    /// Called when the node is removed from the scene tree.
    /// </summary>
    /// <param name="disposing">If the node is being disposed.</param>
    protected override void Dispose(bool disposing)
    {
        Events.Instance.PlayerDied -= OnPlayerDied;

        base.Dispose(disposing);
    }

    #endregion

    #region MethodsForSignals

    /// <summary>
    /// Called when the player died.
    /// </summary>
    /// <param name="playerColor">The color of the player.</param>
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

    /// <summary>
    /// Plays the animation for the player kill.
    /// </summary>
    /// <param name="playerNameLabel">The player name label.</param>
    /// <param name="bombPicture">The bomb's picture.</param>
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

    /// <summary>
    /// Sets the font color.
    /// </summary>
    /// <param name="value">The color value to set.</param>
    /// <param name="label">The label to set the value for.</param>
    private static void SetFontColor(Color value, Label label) =>
        label.Set("theme_override_colors/font_color", value);

    /// <summary>
    /// Sets the font outline size.
    /// </summary>
    /// <param name="value">The outline size value to set.</param>
    /// <param name="label">The label to set the value for.</param>
    private static void SetFontOutlineSize(int value, Label label) =>
        label.Set("theme_override_constants/outline_size", value);
}