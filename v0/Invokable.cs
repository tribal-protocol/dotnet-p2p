namespace v0;

public class Invokable
{
    public Invokable(string command, string description, Func<CommandProcessor, bool> action)
    {
        this.Command = command;
        this.Description = description;
        this.Action = action;
    }

    public Func<CommandProcessor, bool> Action { get;  }
    public string Command { get; }
    public string Description { get; }

    public override string ToString() => $"    {this.Command.PadRight(20)[..20]}{this.Description}";
}
