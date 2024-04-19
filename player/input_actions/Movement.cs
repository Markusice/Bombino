using System;
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
    public static Movement MoveForward => new("move_forward", (direction) =>
    {
        direction.Z -= 1.0f;

        return direction;
    });

    /// <summary>
    /// Represents the movement action to move backward.
    /// </summary>
    public static Movement MoveBackward => new("move_back", (direction) =>
    {
        direction.Z += 1.0f;

        return direction;
    });

    /// <summary>
    /// Represents the movement action to move left.
    /// </summary>
    public static Movement MoveLeft => new("move_left", (direction) =>
    {
        direction.X -= 1.0f;

        return direction;
    });

    /// <summary>
    /// Represents the movement action to move right.
    /// </summary>
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
    private Movement(string name, Func<Vector3, Vector3> action)
    {
        Name = name;
        Action = action;
    }

    protected bool Equals(Movement other)
    {
        return Name == other.Name && Equals(Action, other.Action);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Movement)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Action);
    }
}