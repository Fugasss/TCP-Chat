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
        public bool Connected {get; private set;}

        private int m_CurrentClientId;

        private IChat m_Chat;

        public void Start()
        {
            m_Listener.Start();
            Connected = true;
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
                        client.Send(new Command(Commands.ConnectDeny));
                        client.Tcp.Close();
                    }
                    return;
                }

                m_CurrentClientId++;
                m_ConnectedClients.Add(client);

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
                    m_Chat.SendMessage(welcome.ToString());
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
                        m_Chat.SendMessage(new Formatter(DateTime.Now.ToString("HH:mm"), "Disconnected", "Bye bye " + sender.Name), ConsoleColor.Green);
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
            foreach(var client in m_ConnectedClients)
                client.Send(packet);
        }
    }
}