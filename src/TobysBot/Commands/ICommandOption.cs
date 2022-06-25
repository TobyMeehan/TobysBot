namespace TobysBot.Commands;

/// <summary>
/// Represents a command option/parameter.
/// </summary>
public interface ICommandOption
{
    /// <summary>
    /// Name of the option.
    /// </summary>
    string? Name { get; }
    
    /// <summary>
    /// Summary of the option.
    /// </summary>
    string? Description { get; }
    
    /// <summary>
    /// Datatype of the option.
    /// </summary>
    Type? Type { get; }
    
    /// <summary>
    /// Whether the option is required.
    /// </summary>
    bool Required { get; }
    
    /// <summary>
    /// Default value of the option.
    /// </summary>
    object? DefaultValue { get; }
}