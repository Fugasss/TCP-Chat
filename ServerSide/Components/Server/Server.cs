using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Common.Net.ConcretePackets;
using Common.Net;
using Common.Chat;
using Common.Messages;
using System.Reflection.Emit;

namespace ServerSide.Components
{
    internal class Server : IServer
    {
        public Server(int port, int maxClients, IChat chat)
        {
            m_Chat = chat;
            m_Listener = new TcpListener(IPAddress.Any, port);
            m_ConnectedClients = new(maxClients);
            Port = port;
            MaxClients = maxClients;
        }

        private TcpListener m_Listener;
        private List<Client> m_ConnectedClients;

        public int Port { get; }
        public int MaxClients { get; }

        private int m_CurrentClientId;

        private readonly IChat m_Chat;

        public event Action<Formatter> Log;
        public event Action<Formatter> Warn;
        public event Action<Exception> Error;
        public event Action<Formatter> ClientConnect;
        public event Action<Formatter> ClientDisconnect;
        public event Action<Formatter> ClientMessage;
        public event Action<Formatter> ServerStart;
        public event Action<Formatter> ServerStop;

        public void Start()
        {
            m_Listener.Start();

            ServerStart?.Invoke(new Formatter(label: "START", message: $"Listening on port {Port}"));
            ServerStart?.Invoke(new Formatter(time: "-", message: $"Internal IP: {Dns.GetHostEntry(Dns.GetHostName())
                                                  .AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork)}"));
            ServerStart?.Invoke(new Formatter(time: "-", message: $"External IP: {new HttpClient().GetStringAsync("https://ipv4.icanhazip.com/").Result}"));

            BeginReceiveConnection();
        }

        private void BeginReceiveConnection()
        {
            m_Listener.BeginAcceptTcpClient(OnConnectionReceived, null);
        }

        private void OnConnectionReceived(IAsyncResult result)
        {
            try
            {
                var tcp = m_Listener.EndAcceptTcpClient(result);

                if (tcp == null)
                    return;

                var client = new Client(tcp, m_CurrentClientId, this, m_Chat);

                if (m_ConnectedClients.Count == MaxClients)
                {
                    Warn?.Invoke(new Formatter(label: "WARN", message: $"Max count of clients reached. Connection for {client.Tcp.Client.RemoteEndPoint} denied"));

                    if (client.Tcp.Connected)
                    {
                        client?.Send(new Command(Commands.ConnectDeny));
                        client = null;
                    }
                    return;
                }

                m_CurrentClientId++;
                m_ConnectedClients.Add(client);
                client.Start();
            }
            catch (Exception e)
            {
                Error?.Invoke(e);
            }
            finally
            {
                BeginReceiveConnection();
            }
        }

        public void Stop()
        {
            m_Listener.Stop();
            SendAll(new Command(Commands.ServerStop).GetBytes());
            ServerStop?.Invoke(new Formatter(label: "STOP", message: "Server was stopped"));
        }

        public void HandlePacket(Client sender, byte[] bytes)
        {
            Packet packet = new(bytes);

            switch (packet.Type)
            {
                case PacketType.Welcome:
                    Welcome welcome = new(bytes);
                    sender.Name = welcome.Name;
                    ClientConnect?.Invoke(welcome.ToFormatter());
                    SendAllExclude(bytes, sender);
                    break;
                case PacketType.ClientMessage:
                    UserMessage message = new(bytes);
                    ClientMessage?.Invoke(message.ToFormatter());
                    SendAllExclude(bytes, sender);
                    break;
                case PacketType.Command:
                    Command command = new(bytes);
                    if (command.CommandType == Commands.ClientDisconnect)
                    {
                        sender.Stop();
                        m_ConnectedClients.Remove(sender);

                        ClientDisconnect?.Invoke(new Formatter(label: "Disconnected", message: "Bye-bye " + sender.Name));
                        SendAllExclude(new ServerMessage("Bye-bye " + sender.Name).GetBytes(), sender);
                    }
                    break;
            }
        }

        private void SendAllExclude(byte[] packet, params Client[] clients)
        {
            foreach (var client in m_ConnectedClients.Except(clients))
                client.Send(packet);
        }
        private void SendAll(byte[] packet)
        {
            foreach (var client in m_ConnectedClients)
                client.Send(packet);
        }
    }
}