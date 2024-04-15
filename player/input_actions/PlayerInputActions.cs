namespace Bombino.player.input_actions;

internal class PlayerInputActions
{
    public readonly Movement[] Movements =
        { Movement.MoveForward, Movement.MoveBackward, Movement.MoveLeft, Movement.MoveRight };

    public readonly BombPlace BombPlace = new();
}