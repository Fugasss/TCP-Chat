using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using ChatikSDavidom.Components.Net;
using Common.Net.ConcretePackets;

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
            var tcp = m_Listener.EndAcceptTcpClient(result);

            if (tcp == null)
                return;

            m_ConnectedClients.Add(new Client(tcp, m_CurrentClientId++, this));

            if (m_CurrentClientId == MaxClients) return;

            BeginReceiveConnection();
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
                    SendExclude(sender, packet);
                    break;
                case PacketType.Message:
                    UserMessage message = new(bytes);
                    Log(message.ToString());
                    SendExclude(sender, packet);
                    break;
                case PacketType.Command:
                    break;
            }
        }

        private void SendExclude(Client sender, Packet packet)
        {
            foreach (var client in m_ConnectedClients.Where(x => x.Id != sender.Id))
                client.Send(packet);
        }

        public static void Log(string format, params object[] args) 
        {
            Console.WriteLine(string.Format(format, args));
        }
    }
}