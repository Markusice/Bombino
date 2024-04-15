namespace Bombino.player.input_actions;

/// <summary>
/// Represents the input actions for the player.
/// </summary>
internal class PlayerInputActions
{
    public readonly Movement[] Movements =
        { Movement.MoveForward, Movement.MoveBackward, Movement.MoveLeft, Movement.MoveRight };

    public readonly BombPlace BombPlace = new();
}