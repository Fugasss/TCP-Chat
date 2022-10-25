using ChatikSDavidom.Components.Net;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide.Components
{
    internal class Client
    {
        public Client(TcpClient tcp, int id, IServer server)
        {
            Tcp = tcp;
            Id = id;
            m_Server = server;
            m_Stream = tcp.GetStream();

            BeginRead();
        }

        public string Name { get; set; } = "";

        public TcpClient Tcp { get; }
        public int Id { get; }

        private readonly NetworkStream m_Stream;
        private readonly IServer m_Server;
        private byte[] m_SendBuffer = new byte[Settings.MaxBufferSize];
        private byte[] m_ReceiveBuffer = new byte[Settings.MaxBufferSize];

        public void Send(Packet packet)
        {
            var bytes = packet.GetBytes();

            Send(bytes);
        }

        public void Send(byte[] bytes)
        {
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

            m_Server.HandlePacket(this, received);
            BeginRead();
        }


    }
}
