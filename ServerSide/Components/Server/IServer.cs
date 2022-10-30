using Common.Messages;

namespace ServerSide.Components
{
    internal interface IServer
    {
        event Action<Formatter> Warn;
        event Action<Exception> Error;
        event Action<Formatter> ClientConnect;
        event Action<Formatter> ClientDisconnect;
        event Action<Formatter> ClientMessage;
        event Action<Formatter> ServerStart;
        event Action<Formatter> ServerStop;

        void Start();
        void Stop();

        void HandlePacket(Client sender, byte[] bytes);
    }
}