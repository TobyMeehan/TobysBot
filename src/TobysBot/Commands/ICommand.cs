﻿using Discord.Commands;

namespace TobysBot.Commands;

public interface ICommand
{
    string? Name { get; }
    string? Description { get; }
    ICommandDictionary<ICommand> SubCommands { get; }
    IReadOnlyCollection<ICommandOption> Options { get; }
}