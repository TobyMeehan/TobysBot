namespace TobysBot.Commands;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public class ChoiceAttribute : Attribute
{
    /// <summary>
    /// Name of the choice.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Value of the choice.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Initialises a new <see cref="ChoiceAttribute"/> with the specified name.
    /// </summary>
    /// <param name="name"></param>
    public ChoiceAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Initialises a new <see cref="ChoiceAttribute"/> with the specified name and value.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public ChoiceAttribute(string name, string value)
    {
        Name = name;
        Value = value;
    }
}