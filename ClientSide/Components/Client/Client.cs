using Common;
using Common.Chat;
using Common.Net;
using Common.Net.ConcretePackets;
using System.Net;
using System.Net.Sockets;

namespace ChatikSDavidom.Components.Client
{
    public class Client
    {
        public Client(IPAddress address, int port, string name, IChat chat)
        {
            m_Address = address;
            m_Port = port;
            Name = name;
            m_Chat = chat;
            m_Tcp = new TcpClient(address.ToString(), port);
            m_Stream = m_Tcp.GetStream();

            BeginRead();
        }

        public bool Connected => m_Tcp.Connected;
        public string Name { get; }

        private readonly TcpClient m_Tcp;
        private readonly NetworkStream m_Stream;
        private readonly IPAddress m_Address;
        private readonly int m_Port;

        private readonly byte[] m_ReceiveBuffer = new byte[ProjectSettings.MaxBufferSize];
        private readonly byte[] m_SendBuffer = new byte[ProjectSettings.MaxBufferSize];

        private Action m_OnCompleteSendAction = null;
        private readonly IChat m_Chat;

        public void Send(Packet packet, Action onSendCompleted = null)
        {
            if (!m_Tcp.Connected) return;
            if (!m_Stream.CanWrite) return;

            m_OnCompleteSendAction = onSendCompleted;

            var bytes = packet.GetBytes();
            Array.Copy(bytes, m_SendBuffer, bytes.Length);

            m_Stream.BeginWrite(m_SendBuffer, 0, bytes.Length, EndWrite, null);

        }
        private void EndWrite(IAsyncResult result)
        {
            try
            {
                m_Stream.EndWrite(result);

                m_OnCompleteSendAction?.Invoke();
                m_OnCompleteSendAction = null;

            }
            catch (Exception e)
            {
                m_Chat.SendException(e);
            }
        }


        private void BeginRead()
        {
            if (!m_Tcp.Connected) return;

            m_Stream.BeginRead(m_ReceiveBuffer, 0, ProjectSettings.MaxBufferSize, ReadAsync, null);
        }

        private void ReadAsync(IAsyncResult result)
        {
            try
            {
                if (!m_Tcp.Connected)
                    return;

                var count = m_Stream.EndRead(result);

                if (count <= 0)
                    return;

                var received = new byte[count];

                Array.Copy(m_ReceiveBuffer, received, count);

                HandleReceived(received);
            }
            catch (Exception e)
            {
                m_Chat.SendException(e);
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
                    m_Chat.SendMessage(welcome.ToString());
                    break;
                case PacketType.ClientMessage:
                    var message = new UserMessage(bytes);
                    m_Chat.SendMessage(message.ToString());
                    break;
                case PacketType.ServerMessage:
                    var serverMessage = new ServerMessage(bytes);
                    m_Chat.SendMessage(serverMessage.ToString(), ConsoleColor.DarkYellow);
                    break;
                case PacketType.Command:
                    var command = new Command(bytes);

                    switch (command.CommandType)
                    {
                        case Commands.ClientDisconnect:
                        case Commands.ConnectDeny:
                        case Commands.ServerStop:
                            m_Chat.SendMessage(new Common.Messages.Formatter(label: "DISCONNECT", message: command.CommandType.ToString()), ConsoleColor.Red);

                            if (m_Tcp.Connected)
                                m_Tcp?.Close();
                            break;
                    }

                    break;

            }
        }

        public void Stop()
        {
            m_Tcp.Close();
        }
    }
}
