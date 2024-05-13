using Bombino.events;
using Godot;

namespace Bombino.ui.main_ui.animation_scripts;

/// <summary>
/// Represents the player bomb number change animation scene.
/// </summary>
internal partial class PlayerBombNumberChangeAnimation : Node
{
    #region Fields

    private const float TweenDuration = 0.3f;

    private GridContainer PlayersBombData { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        PlayersBombData = GetNode<GridContainer>("%PlayersBombData");

        Events.Instance.PlayerBombNumberIncremented += PlayerBombNumberIncremented;
        Events.Instance.PlayerBombNumberDecreased += PlayerBombNumberDecreased;
    }

    /// <summary>
    /// Called when the node is removed from the scene tree.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        Events.Instance.PlayerBombNumberIncremented -= PlayerBombNumberIncremented;
        Events.Instance.PlayerBombNumberDecreased -= PlayerBombNumberDecreased;

        base.Dispose(disposing);
    }

    #endregion

    #region MethodsForSignals

    /// <summary>
    /// Called when the player bomb number is incremented.
    /// </summary>
    /// <param name="playerColor">The color of the player.</param>
    /// <param name="numberOfAvailableBombs">The number of available bombs.</param>
    private void PlayerBombNumberIncremented(string playerColor, int numberOfAvailableBombs)
    {
        const float tweenScale = 1.1f;

        var bombStatusContainer = GetNecessaryNodes(playerColor,
            out var bombPicture, out var bombNumberCircle);

        ChangeNumberLabelText(numberOfAvailableBombs, bombStatusContainer);

        PlayAnimationsForNumberIncrement(bombPicture, bombNumberCircle, tweenScale);
    }

    /// <summary>
    /// Called when the player bomb number is decreased.
    /// </summary>
    /// <param name="playerColor">The color of the player.</param>
    /// <param name="numberOfAvailableBombs">The number of available bombs.</param>
    private void PlayerBombNumberDecreased(string playerColor, int numberOfAvailableBombs)
    {
        const float tweenScale = 0.85f;

        var bombStatusContainer = GetNecessaryNodes(playerColor,
            out var bombPicture, out var bombNumberCircle);

        ChangeNumberLabelText(numberOfAvailableBombs, bombStatusContainer);

        PlayAnimationForNumberDecrement(numberOfAvailableBombs, bombNumberCircle, tweenScale, bombPicture);
    }

    #endregion

    /// <summary>
    /// Gets the necessary nodes.
    /// </summary>
    /// <param name="playerColor">The color of the player.</param>
    /// <param name="bombPicture">The bomb's picture.</param>
    /// <param name="bombNumberCircle">The bomb number circle.</param>
    /// <returns>The bomb status container.</returns>
    private PanelContainer GetNecessaryNodes(string playerColor, out TextureRect bombPicture,
        out Panel bombNumberCircle)
    {
        var bombStatusContainerName = $"BombStatusContainer_{playerColor}";
        var bombStatusContainer = PlayersBombData.GetNode<PanelContainer>(bombStatusContainerName);

        bombPicture = bombStatusContainer.GetNode<TextureRect>("BombPicture");
        bombNumberCircle = bombStatusContainer.GetNode<Panel>("%BombNumberCircle");

        return bombStatusContainer;
    }

    /// <summary>
    /// Changes the number label text.
    /// </summary>
    /// <param name="numberOfAvailableBombs">The number of available bombs.</param>
    /// <param name="bombStatusContainer">The bomb status container.</param>
    private static void ChangeNumberLabelText(int numberOfAvailableBombs, PanelContainer bombStatusContainer)
    {
        var bombNumberLabel = bombStatusContainer.GetNode<Label>("%BombNumberLabel");
        bombNumberLabel.Text = numberOfAvailableBombs.ToString();
    }

    /// <summary>
    /// Plays animations for number increment.
    /// </summary>
    /// <param name="bombPicture">The bomb's picture.</param>
    /// <param name="bombNumberCircle">The bomb number circle.</param>
    /// <param name="tweenScale">The tween scale.</param>
    private void PlayAnimationsForNumberIncrement(TextureRect bombPicture, Panel bombNumberCircle, float tweenScale)
    {
        var tween = CreateTween()
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);

        if (Mathf.IsEqualApprox(bombPicture.SelfModulate.A, 0.6f))
        {
            tween.TweenProperty(bombPicture, "self_modulate",
                new Color(1, 1, 1, 1),
                TweenDuration);

            tween.Parallel().TweenProperty(bombNumberCircle, "scale",
                new Vector2(tweenScale, tweenScale),
                TweenDuration);
        }
        else
            tween.TweenProperty(bombNumberCircle, "scale",
                new Vector2(tweenScale, tweenScale),
                TweenDuration);

        tween.TweenProperty(bombNumberCircle, "scale",
            Vector2.One,
            TweenDuration);
    }

    /// <summary>
    /// Plays animations for number decrement.
    /// </summary>
    /// <param name="numberOfAvailableBombs">The number of available bombs.</param>
    /// <param name="bombNumberCircle">The bomb number circle.</param>
    /// <param name="tweenScale">The tween scale.</param>
    /// <param name="bombPicture">The bomb's picture.</param>
    private void PlayAnimationForNumberDecrement(int numberOfAvailableBombs, Panel bombNumberCircle, float tweenScale,
        TextureRect bombPicture)
    {
        var tween = CreateTween()
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);

        tween.TweenProperty(bombNumberCircle, "scale",
            new Vector2(tweenScale, tweenScale),
            TweenDuration);

        if (numberOfAvailableBombs == 0)
        {
            bombPicture.PivotOffset = bombPicture.Size / 8;

            tween.Parallel()
                .TweenProperty(bombPicture, "scale",
                    new Vector2(tweenScale, tweenScale),
                    0.15f)
                .SetTrans(Tween.TransitionType.Bounce);

            tween.TweenProperty(bombPicture, "scale",
                    Vector2.One,
                    0.1)
                .SetTrans(Tween.TransitionType.Spring);

            tween.Parallel().TweenProperty(bombNumberCircle, "scale",
                Vector2.One,
                TweenDuration);

            tween.Parallel().TweenProperty(bombPicture, "self_modulate",
                new Color(1, 1, 1, 0.6f),
                TweenDuration);
        }
        else
            tween.TweenProperty(bombNumberCircle, "scale",
                Vector2.One,
                TweenDuration);
    }
}