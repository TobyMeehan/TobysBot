namespace TobysBot.Commands;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public class ChoiceAttribute : Attribute
{
    public string Name { get; }
    public object Value { get; }

    public ChoiceAttribute(string name)
    {
        Name = name;
    }

    public ChoiceAttribute(string name, string value)
    {
        Name = name;
        Value = value;
    }
}