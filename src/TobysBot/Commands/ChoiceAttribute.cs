namespace TobysBot.Commands;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public class ChoiceAttribute : Attribute
{
    public string Name { get; set; }
    public object Value { get; set; }

    public ChoiceAttribute(string name)
    {
        Name = name;
    }
}