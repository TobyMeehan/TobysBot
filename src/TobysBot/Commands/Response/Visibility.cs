namespace TobysBot.Commands.Response;

/// <summary>
/// The visibility of a response.
/// </summary>
public enum Visibility
{
    /// <summary>
    /// The response can be seen by everyone.
    /// </summary>
    Public,
    
    /// <summary>
    /// The response is ephemeral if possible, otherwise public.
    /// </summary>
    Ephemeral,
    
    /// <summary>
    /// The response is ephemeral if possible, otherwise not shown.
    /// </summary>
    Hidden,
    
    /// <summary>
    /// The response is ephemeral if possible, otherwise sent in DMs.
    /// </summary>
    Private
}