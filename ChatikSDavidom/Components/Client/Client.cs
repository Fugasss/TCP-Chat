using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using ChatikSDavidom.Components;
using ChatikSDavidom.Components.Net;
using Common;
using ClientSide.Components;
using Common.Net.ConcretePackets;

namespace ChatikSDavidom.Components.Client
{
    public class Client
    {
        public Client(IPAddress address, int port, string name)
        {
            m_Address = address;
            m_Port = port;
            Name = name;
            m_Client = new TcpClient(address.ToString(), port);
            m_Stream = m_Client.GetStream();
            Connected = true;

            BeginRead();
        }

        public bool Connected { get; private set; }
        public string Name { get; }

        private TcpClient m_Client;
        private NetworkStream m_Stream;
        private readonly IPAddress m_Address;
        private readonly int m_Port;

        private byte[] m_ReceiveBuffer = new byte[Settings.MaxBufferSize];
        private byte[] m_SendBuffer = new byte[Settings.MaxBufferSize];

        public void Send(Packet packet)
        {
            //if(packet is )
            //Chat.SendMessage(packet.ToString());

            var bytes = packet.GetBytes();
            Array.Copy(bytes, m_SendBuffer, bytes.Length);

            m_Stream.BeginWrite(m_SendBuffer, 0, bytes.Length, null, null);
        }

        private void BeginRead()
        {
            m_Stream.BeginRead(m_ReceiveBuffer, 0, Settings.MaxBufferSize, ReadAsync, null);
        }

        private void ReadAsync(IAsyncResult result)
        {
            var count = m_Stream.EndRead(result);

            if (count <= 0)
                return;

            var received = new byte[count];

            Array.Copy(m_ReceiveBuffer, received, count);

            HandleReceived(received);
            BeginRead();
        }

        private void HandleReceived(byte[] bytes)
        {
            Packet packet = new(bytes);

            switch (packet.Type)
            {
                case PacketType.Welcome:
                    var welcome = new Welcome(bytes);
                    Chat.SendMessage(welcome.ToString());
                    break;
                case PacketType.Message:
                    var message = new UserMessage(bytes);
                    Chat.SendMessage(message.ToString());
                    break;
                case PacketType.Command:
                    var command = new Command(bytes);
                    if (command.CommandType is Commands.ClientDisconnect or Commands.ServerStop)
                    {
                        Connected = false;
                        m_Client.Close();
                    }
                    break;
            }
        }
    }
}
