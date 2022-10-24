namespace ServerSide.Components
{
    internal interface IServer
    {
        void Start();
        void Stop();

        void HandlePacket(Client sender, byte[] bytes);
    }
}