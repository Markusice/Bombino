using Bombino.events;
using Godot;

namespace Bombino.ui.main_ui.animation_scripts;

internal partial class PlayerBombNumberChangeAnimation : Node
{
    #region Fields

    private const float TweenDuration = 0.3f;

    private GridContainer PlayersBombData { get; set; }

    #endregion

    #region Overrides

    public override void _Ready()
    {
        PlayersBombData = GetNode<GridContainer>("%PlayersBombData");

        Events.Instance.PlayerBombNumberIncremented += PlayerBombNumberIncremented;
        Events.Instance.PlayerBombNumberDecreased += PlayerBombNumberDecreased;
    }

    protected override void Dispose(bool disposing)
    {
        Events.Instance.PlayerBombNumberIncremented -= PlayerBombNumberIncremented;
        Events.Instance.PlayerBombNumberDecreased -= PlayerBombNumberDecreased;

        base.Dispose(disposing);
    }

    #endregion

    #region MethodsForSignals

    private void PlayerBombNumberIncremented(string playerColor, int numberOfAvailableBombs)
    {
        const float tweenScale = 1.1f;

        var bombStatusContainer = GetNecessaryNodes(playerColor,
            out var bombPicture, out var bombNumberCircle);

        ChangeNumberLabelText(numberOfAvailableBombs, bombStatusContainer);

        PlayAnimationsForNumberIncrement(bombPicture, bombNumberCircle, tweenScale);
    }

    private void PlayerBombNumberDecreased(string playerColor, int numberOfAvailableBombs)
    {
        const float tweenScale = 0.85f;

        var bombStatusContainer = GetNecessaryNodes(playerColor,
            out var bombPicture, out var bombNumberCircle);

        ChangeNumberLabelText(numberOfAvailableBombs, bombStatusContainer);

        PlayAnimationForNumberDecrement(numberOfAvailableBombs, bombNumberCircle, tweenScale, bombPicture);
    }

    #endregion

    private PanelContainer GetNecessaryNodes(string playerColor, out TextureRect bombPicture,
        out Panel bombNumberCircle)
    {
        var bombStatusContainerName = $"BombStatusContainer_{playerColor}";
        var bombStatusContainer = PlayersBombData.GetNode<PanelContainer>(bombStatusContainerName);

        bombPicture = bombStatusContainer.GetNode<TextureRect>("BombPicture");
        bombNumberCircle = bombStatusContainer.GetNode<Panel>("%BombNumberCircle");

        return bombStatusContainer;
    }

    private static void ChangeNumberLabelText(int numberOfAvailableBombs, PanelContainer bombStatusContainer)
    {
        var bombNumberLabel = bombStatusContainer.GetNode<Label>("%BombNumberLabel");
        bombNumberLabel.Text = numberOfAvailableBombs.ToString();
    }

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