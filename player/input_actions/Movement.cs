using Godot;

namespace Bombino.player.input_actions;

/// <summary>
/// Represents a movement action in the game.
/// </summary>
internal class Movement
{
    /// <summary>
    /// Gets the name of the movement action.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the action to perform for the movement.
    /// </summary>
    public Func<Vector3, Vector3> Action { get; }

    /// <summary>
    /// Represents the movement action to move forward.
    /// </summary>
    /// <returns>The movement action to move forward.</returns>
    public static Movement MoveForward => new("move_forward", (direction) =>
    {
        direction.Z -= 1.0f;

        return direction;
    });

    /// <summary>
    /// Represents the movement action to move backward.
    /// </summary>
    /// <returns>The movement action to move backward.</returns>
    public static Movement MoveBackward => new("move_back", (direction) =>
    {
        direction.Z += 1.0f;

        return direction;
    });

    /// <summary>
    /// Represents the movement action to move left.
    /// </summary>
    /// <returns>The movement action to move left.</returns>
    public static Movement MoveLeft => new("move_left", (direction) =>
    {
        direction.X -= 1.0f;

        return direction;
    });

    /// <summary>
    /// Represents the movement action to move right.
    /// </summary>
    /// <returns>The movement action to move right.</returns>
    public static Movement MoveRight => new("move_right", (direction) =>
    {
        direction.X += 1.0f;

        return direction;
    });

    /// <summary>
    /// Initializes a new instance of the <see cref="Movement"/> class with the specified name and action.
    /// </summary>
    /// <param name="name">The name of the movement action.</param>
    /// <param name="action">The action to perform for the movement.</param>
    /// <returns>The new instance of the <see cref="Movement"/> class.</returns>
    private Movement(string name, Func<Vector3, Vector3> action)
    {
        Name = name;
        Action = action;
    }
}