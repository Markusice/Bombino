using System;
using Godot;

namespace Bombino.player.input_actions;

internal class Movement
{
    public string Name { get; }

    public Func<Vector3, Vector3> Action { get; }

    public static Movement MoveForward => new("move_forward", (direction) =>
    {
        direction.Z -= 1.0f;

        return direction;
    });

    public static Movement MoveBackward => new("move_back", (direction) =>
    {
        direction.Z += 1.0f;

        return direction;
    });

    public static Movement MoveLeft => new("move_left", (direction) =>
    {
        direction.X -= 1.0f;

        return direction;
    });

    public static Movement MoveRight => new("move_right", (direction) =>
    {
        direction.X += 1.0f;

        return direction;
    });

    private Movement(string name, Func<Vector3, Vector3> action)
    {
        Name = name;
        Action = action;
    }
}