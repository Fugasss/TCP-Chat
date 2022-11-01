using Common.Chat;
using Common.DI;
using ServerSide.Components;

internal class Program
{
    public static ServiceContainer? Container { get; private set; }

    private static async Task Main(string[] args)
    {
        InitServer(out var server);
        InitProgramEndPoint(server);

        await Task.Delay(-1);
    }

    private static void InitServer(out Server server)
    {
        int port;
        int maxConnections;

        do
            MyConsole.WriteLine("Enter listening port", ConsoleColor.DarkGreen);
        while (!int.TryParse((string)MyConsole.ReadLine(), out port));

        do
            MyConsole.WriteLine("Enter max connections", ConsoleColor.DarkGreen);
        while (!int.TryParse((string)MyConsole.ReadLine(), out maxConnections));

        server = new(port, maxConnections);

        server.Warn += (warn) => MyConsole.WriteLine(warn, ConsoleColor.Yellow);
        server.Error += MyConsoleHelper.WriteLineException;
        server.ClientConnect += (formatter) => MyConsole.WriteLine(formatter, ConsoleColor.Green);
        server.ClientDisconnect += (formatter) => MyConsole.WriteLine(formatter, ConsoleColor.DarkYellow);
        server.ClientMessage += (formatter) => MyConsole.WriteLine(formatter);
        server.ServerStop += (formatter) => MyConsole.WriteLine(formatter, ConsoleColor.Red);
        server.ServerStart += (formatter) => MyConsole.WriteLine(formatter, ConsoleColor.Cyan);

        server.Start();
    }

    private static void InitProgramEndPoint(Server server)
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) => server.Stop();
    }
}