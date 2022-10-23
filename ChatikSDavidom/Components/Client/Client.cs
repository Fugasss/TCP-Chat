using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ChatikSDavidom.Components.Client
{
    public class Client
    {
        public Client(IPAddress address, int port)
        {
            m_Address = address;
            m_Port = port;
            m_Client = new TcpClient(address.ToString(), port);

            BeginConnection();
        }

        private TcpClient m_Client;
        private NetworkStream m_Stream;
        private readonly IPAddress m_Address;
        private readonly int m_Port;

        private byte[] m_Buffer = new byte[4096];
        private byte[] m_Received = new byte[4096];

        private void BeginConnection()
        {
            m_Stream = m_Client.GetStream();
            m_Stream.BeginRead(m_Buffer, 0, 0, ReadAsync, null);
        }


        private void ReadAsync(IAsyncResult result)
        {
            var count = m_Stream.EndRead(result);

            if (count <= 0)
                return;

            Array.Copy(m_Buffer, m_Received, count);


        }
    }
}
