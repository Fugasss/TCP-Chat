using ChatikSDavidom.Components.Client;
using Common.Chat;
using Common.DI;
using Common.Net.ConcretePackets;
using System.Net;

internal class Program
{
    public static ServiceContainer? Container { get; private set; }

    private static void Main(string[] args)
    {
        InitClient(out var client);
        InitProgramEndPoint(client);

        while (client.Connected)
        {
            var message = (string)MyConsole.ReadLine();
            if (string.IsNullOrWhiteSpace(message)) continue;
            client.Send(new UserMessage(client.Name, message));
        }

        Console.ReadKey();
    }

    private static void InitClient(out Client client)
    {
        string ip;
        int port;
        string name;

        IPAddress address;

        do
        {
            MyConsole.WriteLine("Enter IP: ", ConsoleColor.DarkGreen);
            ip = (string)MyConsole.ReadLine();

        }
        while (!IPAddress.TryParse(ip, out address));

        do
            MyConsole.WriteLine("Enter Port: ", ConsoleColor.DarkGreen);
        while (!int.TryParse((string)MyConsole.ReadLine(), out port));

        do
        {
            MyConsole.WriteLine("Enter Your Name: ", ConsoleColor.DarkGreen);
            name = (string)MyConsole.ReadLine();
        }
        while (string.IsNullOrWhiteSpace(name));

        client = new(address, port, name);

        client.Send(new Welcome(client.Name));

    }
    private static void InitProgramEndPoint(Client client)
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            client.Send(new Command(Commands.ClientDisconnect), () => client.Stop());
        };

    }
}

