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
        public bool Connected { get; private set; }

        private int m_CurrentClientId;

        private IChat m_Chat;

        public event EventHandler<Formatter> Log;
        public event EventHandler<EventArgs> Warn;
        public event EventHandler<EventArgs> Error;
        public event EventHandler<EventArgs> ClientConnected;
        public event EventHandler<EventArgs> ClientDisconnected;
        public event EventHandler<EventArgs> ClientPacketReceived;
        public event EventHandler<EventArgs> ServerStart;
        public event EventHandler<EventArgs> ServerStop;

        public void Start()
        {
            m_Listener.Start();
            Connected = true;

            Log?.Invoke(this, new Formatter(label: "LOG", message: $"Server Listening on port {Port}"));
            Log?.Invoke(this,
                new Formatter(label: "LOG",
                message: $"Your internal IP: {Dns.GetHostEntry(Dns.GetHostName())
                                                  .AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork)}"));
            Log?.Invoke(this, new Formatter(label: "LOG",
                                            message: $"Your external IP: {new HttpClient().GetStringAsync("https://ipv4.icanhazip.com/").Result}"));

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
                m_Chat.SendException(e);
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
            Connected = false;
        }

        public void HandlePacket(Client sender, byte[] bytes)
        {
            Packet packet = new(bytes);

            switch (packet.Type)
            {
                case PacketType.Welcome:
                    Welcome welcome = new(bytes);
                    sender.Name = welcome.Name;
                    m_Chat.SendMessage(welcome.ToString(), ConsoleColor.Green);
                    SendExclude(sender, bytes);
                    break;
                case PacketType.Message:
                    UserMessage message = new(bytes);
                    m_Chat.SendMessage(message.ToString());
                    SendExclude(sender, bytes);
                    break;
                case PacketType.Command:
                    Command command = new(bytes);
                    if (command.CommandType == Commands.ClientDisconnect)
                    {
                        sender.Stop();
                        m_Chat.SendMessage(new Formatter(DateTime.Now.ToString("HH:mm"), "Disconnected", "Bye bye " + sender.Name), ConsoleColor.Cyan);
                        m_ConnectedClients.Remove(sender);
                    }
                    break;
            }
        }

        private void SendExclude(Client sender, byte[] packet)
        {
            foreach (var client in m_ConnectedClients.Where(x => x.Id != sender.Id))
                client.Send(packet);
        }
        private void SendAll(byte[] packet)
        {
            foreach (var client in m_ConnectedClients)
                client.Send(packet);
        }
    }
}