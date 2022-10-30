using Common.Messages;

namespace ServerSide.Components
{
    internal interface IServer
    {
        event EventHandler<Formatter> Log;
        event EventHandler<EventArgs> Warn;
        event EventHandler<EventArgs> Error;
        event EventHandler<EventArgs> ClientConnected;
        event EventHandler<EventArgs> ClientDisconnected;
        event EventHandler<EventArgs> ClientPacketReceived;
        event EventHandler<EventArgs> ServerStart;
        event EventHandler<EventArgs> ServerStop;

        void Start();
        void Stop();

        void HandlePacket(Client sender, byte[] bytes);
    }
}