using Common.Chat;
using Common.DI;
using ServerSide.Components;

internal class Program
{
    public static ServiceContainer? Container { get; private set; }

    private static async Task Main(string[] args)
    {
        InitDI();
        InitServer(out var server);
        InitProgramEndPoint(server);

        await Task.Delay(-1);
    }

    private static void InitDI()
    {
        Container = new(new ConsoleChat());
    }
    private static void InitServer(out Server server)
    {
        IChat chat = Container.GetService<ConsoleChat>();

        int port;
        int maxConnections;

        do
            chat.SendMessage("Enter listening port", ConsoleColor.DarkGreen);
        while (!int.TryParse((string)chat.ReadMessage(), out port));

        do
            chat.SendMessage("Enter max connections", ConsoleColor.DarkGreen);
        while (!int.TryParse((string)chat.ReadMessage(), out maxConnections));

        server = new(port, maxConnections, chat);

        server.Warn += (warn) => chat.SendMessage(warn, ConsoleColor.Yellow);
        server.Error += chat.SendException;
        server.ClientConnect += (formatter) => chat.SendMessage(formatter, ConsoleColor.Green);
        server.ClientDisconnect += (formatter) => chat.SendMessage(formatter, ConsoleColor.DarkYellow);
        server.ClientMessage += (formatter) => chat.SendMessage(formatter);
        server.ServerStop += (formatter) => chat.SendMessage(formatter, ConsoleColor.Red);
        server.ServerStart += (formatter) => chat.SendMessage(formatter, ConsoleColor.Cyan);

        server.Start();
    }

    private static void InitProgramEndPoint(Server server)
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => server.Stop();
    }
}