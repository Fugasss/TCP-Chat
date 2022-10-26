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
        public Server(int port, int maxClients)
        {
            m_Listener = new TcpListener(IPAddress.Any, port);
            Port = port;
            MaxClients = maxClients;
        }

        private TcpListener m_Listener;
        private List<Client> m_ConnectedClients;

        public int Port { get; }
        public int MaxClients { get; }

        private int m_CurrentClientId;

        public void Start()
        {
            m_ConnectedClients = new List<Client>(MaxClients);
            m_Listener.Start();
            BeginReceiveConnection();
        }

        private void BeginReceiveConnection()
        {
            m_Listener.BeginAcceptTcpClient(OnReceivedConnection, null);
        }

        private void OnReceivedConnection(IAsyncResult result)
        {
            try
            {
                var tcp = m_Listener.EndAcceptTcpClient(result);

                if (tcp == null)
                    return;

                var client = new Client(tcp, m_CurrentClientId, this);

                if (m_CurrentClientId == MaxClients)
                {
                    client.Send(new Command(Commands.ClientDisconnect));
                    client.Tcp.Close();
                    return;
                }

                m_CurrentClientId++;
                m_ConnectedClients.Add(client);

            }
            catch (Exception e)
            {
                Chat.SendException(e);
            }
            finally
            {
                BeginReceiveConnection();
            }
        }

        public void Stop()
        {
            m_Listener.Stop();
        }

        public void HandlePacket(Client sender, byte[] bytes)
        {
            Packet packet = new(bytes);

            switch (packet.Type)
            {
                case PacketType.Welcome:
                    Welcome welcome = new(bytes);
                    sender.Name = welcome.Name;
                    Log(welcome.ToString());
                    SendExclude(sender, bytes);
                    break;
                case PacketType.Message:
                    UserMessage message = new(bytes);
                    Log(message.ToString());
                    SendExclude(sender, bytes);
                    break;
                case PacketType.Command:
                    Command command = new(bytes);
                    if (command.CommandType == Commands.ClientDisconnect)
                    {
                        sender.Stop();
                        Chat.SendMessage(new Formatter(DateTime.Now.ToString("HH:mm"), "Disconnected", "Bye bye " + sender.Name), ConsoleColor.Green);
                    }
                    break;
            }
        }

        private void SendExclude(Client sender, byte[] packet)
        {
            foreach (var client in m_ConnectedClients.Where(x => x.Id != sender.Id))
                client.Send(packet);
        }

        public static void Log(string format, params object[] args)
        {
            Chat.SendMessage(string.Format(format, args));
        }
        public static void Log(Formatter formatter)
        {
            Chat.SendMessage(formatter);
        }
    }
}