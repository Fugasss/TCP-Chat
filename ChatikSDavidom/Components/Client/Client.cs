using System.Net.Sockets;
using System.Net;
using Common;
using Common.Net.ConcretePackets;
using Common.Net;
using Common.Chat;

namespace ChatikSDavidom.Components.Client
{
    public class Client
    {
        public Client(IPAddress address, int port, string name)
        {
            m_Address = address;
            m_Port = port;
            Name = name;
            m_Tcp = new TcpClient(address.ToString(), port);
            m_Stream = m_Tcp.GetStream();

            BeginRead();
        }

        public bool Connected => m_Tcp.Connected;
        public string Name { get; }

        private TcpClient m_Tcp;
        private NetworkStream m_Stream;
        private readonly IPAddress m_Address;
        private readonly int m_Port;

        private byte[] m_ReceiveBuffer = new byte[Settings.MaxBufferSize];
        private byte[] m_SendBuffer = new byte[Settings.MaxBufferSize];

        public void Send(Packet packet)
        {
            var bytes = packet.GetBytes();
            Array.Copy(bytes, m_SendBuffer, bytes.Length);

            m_Stream.BeginWrite(m_SendBuffer, 0, bytes.Length, null, null);
        }

        private void BeginRead()
        {
            if (!m_Tcp.Connected) return;

            m_Stream.BeginRead(m_ReceiveBuffer, 0, Settings.MaxBufferSize, ReadAsync, null);
        }

        private void ReadAsync(IAsyncResult result)
        {
            try
            {
                var count = m_Stream.EndRead(result);

                if (count <= 0)
                    return;

                var received = new byte[count];

                Array.Copy(m_ReceiveBuffer, received, count);

                HandleReceived(received);
            }
            catch (Exception e)
            {
                Chat.SendException(e);
                m_Tcp.Close();
            }
            finally
            {
                BeginRead();
            }
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
                        m_Tcp.Close();
                    }
                    break;
            }
        }
    }
}
