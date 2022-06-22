﻿namespace TobysBot.Commands;

public interface ICommandOption
{
    string? Name { get; }
    string? Description { get; }
    Type? Type { get; }
    bool Required { get; }
    object? DefaultValue { get; }
}