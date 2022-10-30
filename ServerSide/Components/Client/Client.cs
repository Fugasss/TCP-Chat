using Common;
using Common.Chat;
using Common.Net;
using System.Net.Sockets;

namespace ServerSide.Components
{
    internal class Client
    {
        public Client(TcpClient tcp, int id, IServer server, IChat chat)
        {
            Tcp = tcp;
            Id = id;
            m_Chat = chat;
            m_Server = server;
            m_Stream = tcp.GetStream();
        }

        public string Name { get; set; } = "";

        public TcpClient Tcp { get; }
        public int Id { get; }

        private readonly NetworkStream m_Stream;
        private readonly IServer m_Server;
        private readonly byte[] m_SendBuffer = new byte[ProjectSettings.MaxBufferSize];
        private readonly byte[] m_ReceiveBuffer = new byte[ProjectSettings.MaxBufferSize];

        private readonly IChat m_Chat;

        public void Start()
        {
            BeginRead();
        }

        public void Send(Packet packet)
        {
            var bytes = packet.GetBytes();

            Send(bytes);
        }

        public void Send(byte[] bytes)
        {
            if (!Tcp.Connected) return;

            Array.Copy(bytes, m_SendBuffer, bytes.Length);

            m_Stream.BeginWrite(m_SendBuffer, 0, bytes.Length, null, null);
        }

        private void BeginRead()
        {
            if (!Tcp.Connected)
            {
                Tcp.Close();
                return;
            }
            m_Stream.BeginRead(m_ReceiveBuffer, 0, ProjectSettings.MaxBufferSize, ReadAsync, null);
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

                m_Server.HandlePacket(this, received);
            }
            catch (Exception e)
            {
                m_Chat.SendException(e);

                if (e is IOException)
                {
                    Tcp.Close();
                }
            }
            finally
            {
                BeginRead();
            }

        }

        public void Stop()
        {
            Tcp?.Close();
        }

    }
}
