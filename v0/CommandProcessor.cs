namespace v0;

public class CommandProcessor : IDisposable
{
    private readonly MeshNetwork _network;
    public CommandProcessor(MeshNetwork network) => _network = network;

    private readonly List<Invokable> _commands = new List<Invokable>()
    {
        new("help", "displays this message", cp => cp.ShowHelp()),
        new("quit", "closes connections gracefully, and kills this app", cp => false),
        new("network start", "starts the network", cp => cp.NetworkStart()),
        new("network stop", "stop the network", cp => cp.NetworkStop()),
        new("network list", "list connected peers", cp => cp.NetworkListPeers()),
        new("network connect", "peer connect wizard", cp => cp.NetworkConnect()),
        new("network send", "send message wizard", cp => cp.NetworkSend()),
        new("network broadcast", "broadcast message wizard", cp => cp.NetworkBroadcast())
    };

    public bool Process(string command)
    {
        if (_network == null) return false;

        var clean = (command ?? "").ToLower();
        var invokable = this._commands.SingleOrDefault(c => c.Command.Equals(clean));
        if (invokable == null)
            return ShowHelp("Unknown Command, please try again.");

        return invokable.Action(this);
    }

    public bool ShowHelp(string optional = "")
    {
        if (!string.IsNullOrEmpty(optional)) Prompt.Print(optional);

        Prompt.Print("    Available Commands are:");
        foreach (var invokable in this._commands)
            Prompt.Print(invokable.ToString());
        return true;
    }

    private bool NetworkBroadcast() => SafeExecute(() =>
        _network.BroadcastMessage(Prompt.Ask($"Please enter the message you'd like to broadcast below.{Environment.NewLine}")));

    private bool NetworkConnect() =>
        SafeExecute(() => _network.AddPeer(Prompt.Ask("please enter an IP:PORT to connect to. ")));

    private bool NetworkListPeers() => SafeExecute(() =>
    {
        var peers = _network.ListPeers();

        if (peers == null || !peers.Any())
            Prompt.Print("No peers currently connected");
        else
            peers.ForEach(p => Console.WriteLine(p.IpPort));
    });

    private bool NetworkSend() => SafeExecute(() =>
    {
        var ipPort = Prompt.Ask("please enter an ip:port to send a message to. ");
        var message = Prompt.Ask("Please enter the message you'd like to send below." + Environment.NewLine);
        Prompt.Print("Attempting to send message...");
        _network.SendMessage(ipPort, message);
    });

    private bool NetworkStart()
    {
        if (_network.Started)
            Prompt.Print("network is already started.");
        else
        {
            Prompt.Print("Starting Network.");
            _network.StartNetwork();
            Prompt.Print("Network Started!");
        }
        return true;
    }

    private bool NetworkStop() => SafeExecute(() =>
    {
        Prompt.Print("Stopping Network...");
        _network.StopNetwork();
    }, "network is already stopped.");

    private bool SafeExecute(Action networkActive, string message="Please start the mesh network first.")
    {
        if (_network.Started)
            networkActive();
        else
            Prompt.Print(message);
        return true;
    }

    public void Dispose()
    {
        if (_network.Started)
        {
            // TODO gracefully kill off connections. Gonna want to also look for other exit signals.
        }
    }
}
