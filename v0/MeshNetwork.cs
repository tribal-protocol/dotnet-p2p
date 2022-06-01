using System.Text;
using WatsonMesh;

namespace v0
{
    public class MeshNetwork
    {
        private readonly MeshNode _mesh;

        public MeshNetwork(string ipAddress = "0.0.0.0", int port = 2888)
        {
            _mesh = new MeshNode(ipAddress, port);
            _mesh.PeerConnected += PeerConnected;
            _mesh.PeerDisconnected += PeerDisconnected;
            _mesh.MessageReceived += MessageReceived;
            _mesh.SyncMessageReceived = SyncMessageReceived;
        }
        ~MeshNetwork()
        {
            _mesh.PeerConnected -= PeerConnected;
            _mesh.PeerDisconnected -= PeerDisconnected;
            _mesh.MessageReceived -= MessageReceived;
            _mesh.SyncMessageReceived = null;
            Started = false;
        }

        public bool Started { get; private set; }

        public void StartNetwork()
        {
            _mesh.Start();
            Started = true;
        }

        public void StopNetwork()
        {
            Started = false;
            // TODO STOP ALL CLIENT CONNECTIONS!
        }

        public void AddPeer(string ipPort)
        {
            try
            {
                Prompt.Print($"Attempting to add peer {ipPort}...");
                _mesh.Add(new MeshPeer(ipPort));
            }
            catch (Exception ex)
            {
                Prompt.Print($"An error occurred while attempting to add a new peer! {ex}");
            }
        }

        public List<MeshPeer> ListPeers()
        {
            try
            {
                return _mesh.GetPeers();
            }
            catch (Exception ex)
            {
                Prompt.Print($"An error occurred while attempting to list peers! {ex}");
                return null;
            }
        }

        public void BroadcastMessage(string message)
        {
            try
            {
                Console.WriteLine("Attempting to broadcast message....");
                var broadcastSuccess = _mesh.Broadcast(message);
                Console.WriteLine(broadcastSuccess == true
                    ? "Success!"
                    : "There was an issue broadcasting your message.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while attempting to broadcast message. {ex}");
            }
        }

        public void SendMessage(string ipPort, string message)
        {
            try
            {
                Console.WriteLine($"Attempting to send message to {ipPort}");
                var response = _mesh.SendAndWait(ipPort, 5000, message);
                if (response != null) Console.WriteLine($"response received: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while attempting to send a message to {ipPort}. {ex}");
            }
        }

        private static void PeerConnected(object sender, ServerConnectionEventArgs args) =>
            Prompt.Print($"Peer {args.PeerNode} has connected.");

        private static void PeerDisconnected(object sender, ServerConnectionEventArgs args) =>
            Prompt.Print($"Peer {args.PeerNode} has disconnected.");

        private static void MessageReceived(object sender, MessageReceivedEventArgs args) =>
            Prompt.Print($"Message from {args.SourceIpPort} -- {Encoding.UTF8.GetChars(args.Data)}");

        private static SyncResponse SyncMessageReceived(MessageReceivedEventArgs args)
        {
            Prompt.Print($"Sync Message from {args.SourceIpPort} -- {Encoding.UTF8.GetChars(args.Data)}");
            return new SyncResponse(SyncResponseStatus.Success, "Hello back!");
        }
    }
}
