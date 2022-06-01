namespace v0
{
    using System.Net;

    public static class Program
    {
        public static void Main(string[] args)
        {
            Prompt.Print("'the v0 chain'... in a stateful cli | version 0.0.0");

            var network = GetMeshNetwork();

            using (var processor = new CommandProcessor(network))
            {
                var keepLooping = processor.ShowHelp();
                while (keepLooping)
                {
                    var command = Prompt.Ask("Please enter a command:", "help");
                    keepLooping = processor.Process(command);
                }
            }
            Prompt.Print("Bye!");
        }

        static MeshNetwork GetMeshNetwork()
        {
            var address = Prompt.Ask("Please enter an IP to bind to.", IPAddress.Loopback, IPAddress.Parse);
            Prompt.Print($"using {address}");

            var port = Prompt.Ask("Please enter a port to bind to", 2888, s =>
            {
                if (int.TryParse(s, out var iPort)) return iPort;
                throw new Exception($"Error converting {s} to an int. using 2888");
            });
            Prompt.Print($"using {port}");
            return new MeshNetwork(address.ToString(), port);
        }
    }
}
