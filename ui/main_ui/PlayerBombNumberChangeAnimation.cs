using Bombino.events;
using Godot;

namespace Bombino.ui.main_ui;

internal partial class PlayerBombNumberChangeAnimation : Node
{
    private const float TweenDuration = 0.3f;

    private GridContainer _playersBombData;

    public override void _Ready()
    {
        Events.Instance.PlayerBombNumberIncremented += PlayerBombNumberIncremented;
        Events.Instance.PlayerBombNumberDecreased += PlayerBombNumberDecreased;

        _playersBombData = GetNode<GridContainer>("%PlayersBombData");
    }

    private void PlayerBombNumberIncremented(string playerColor, int numberOfAvailableBombs)
    {
        const float tweenScale = 1.1f;

        var bombStatusContainerName = $"BombStatusContainer_{playerColor}";
        var bombStatusContainer = _playersBombData.GetNode<PanelContainer>(bombStatusContainerName);

        var bombPicture = bombStatusContainer.GetNode<TextureRect>("BombPicture");
        var bombNumberCircle = bombStatusContainer.GetNode<Panel>("%BombNumberCircle");
        var bombNumberLabel = bombStatusContainer.GetNode<Label>("%BombNumberLabel");

        var tween = GetTree().CreateTween().SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.Out);

        bombNumberLabel.Text = numberOfAvailableBombs.ToString();

        if (Mathf.IsEqualApprox(bombPicture.SelfModulate.A, 0.6f))
        {
            tween.TweenProperty(bombPicture, new NodePath(CanvasItem.PropertyName.SelfModulate),
                new Color(1, 1, 1, 1),
                TweenDuration);
            tween.Parallel().TweenProperty(bombNumberCircle, new NodePath(Control.PropertyName.Scale),
                new Vector2(tweenScale, tweenScale), TweenDuration);
        }
        else
            tween.TweenProperty(bombNumberCircle, new NodePath(Control.PropertyName.Scale),
                new Vector2(tweenScale, tweenScale), TweenDuration);

        tween.TweenProperty(bombNumberCircle, new NodePath(Control.PropertyName.Scale),
            Vector2.One, TweenDuration);
    }

    private void PlayerBombNumberDecreased(string playerColor, int numberOfAvailableBombs)
    {
        const float tweenScale = 0.85f;

        var bombStatusContainerName = $"BombStatusContainer_{playerColor}";
        var bombStatusContainer = _playersBombData.GetNode<PanelContainer>(bombStatusContainerName);

        var bombPicture = bombStatusContainer.GetNode<TextureRect>("BombPicture");
        var bombNumberCircle = bombStatusContainer.GetNode<Panel>("%BombNumberCircle");
        var bombNumberLabel = bombStatusContainer.GetNode<Label>("%BombNumberLabel");

        var tween = GetTree().CreateTween().SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.Out);

        tween.TweenProperty(bombNumberCircle, new NodePath(Control.PropertyName.Scale),
            new Vector2(tweenScale, tweenScale), TweenDuration);

        if (numberOfAvailableBombs == 0)
        {
            bombPicture.PivotOffset = bombPicture.Size / 8;

            tween.Parallel()
                .TweenProperty(bombPicture, new NodePath(Control.PropertyName.Scale),
                    new Vector2(tweenScale, tweenScale), 0.15f)
                .SetTrans(Tween.TransitionType.Bounce);

            tween.TweenProperty(bombPicture, new NodePath(Control.PropertyName.Scale),
                    Vector2.One, 0.1)
                .SetTrans(Tween.TransitionType.Spring);
            tween.Parallel().TweenProperty(bombNumberCircle, new NodePath(Control.PropertyName.Scale),
                Vector2.One,
                TweenDuration);
            tween.Parallel().TweenProperty(bombPicture, new NodePath(CanvasItem.PropertyName.SelfModulate),
                new Color(1, 1, 1, 0.6f), TweenDuration);
        }
        else
            tween.TweenProperty(bombNumberCircle, new NodePath(Control.PropertyName.Scale),
                Vector2.One, TweenDuration);
    }
}