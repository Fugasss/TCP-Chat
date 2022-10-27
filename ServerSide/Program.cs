using ServerSide.Components;
using Common.Messages;
using Common.Chat;
using Common.Net.ConcretePackets;
using Common.DI;

internal class Program
{
    public static ServiceContainer? Container { get; private set; }

    private static async Task Main(string[] args)
    {
        InitDI();
        IChat chat = Container.GetService<ConsoleChat>();

        InitServer(chat, out var server);
        InitProgramEndPoint(server);

        await Task.Delay(-1);
    }

    private static void InitDI()
    {
        Container = new(new ConsoleChat());
    }
    private static void InitServer(IChat chat, out Server server)
    {
        int port;
        int maxConnections;

        do
            chat.SendMessage("Enter listening port");
        while (!int.TryParse((string)chat.ReadMessage(), out port));

        do
            chat.SendMessage("Enter max connections");
        while (!int.TryParse((string)chat.ReadMessage(), out maxConnections));

        server = new(port, maxConnections, chat);
        server.Start();

        chat.SendMessage(new Formatter(DateTime.Now.ToString("HH:mm"), "Server", "Server started"));
    }
    private static void InitProgramEndPoint(Server server)
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => server.Stop();
    }
}