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
        InitDI();
        IChat chat = Container.GetService<ConsoleChat>();

        InitClient(chat, out var client);
        InitProgramEndPoint(client);

        while (client.Connected)
        {
            var message = (string)chat.ReadMessage();
            if (string.IsNullOrEmpty(message)) continue;
            client.Send(new UserMessage(client.Name, message));
        }

        Console.ReadKey();
    }

    private static void InitDI()
    {
        Container = new(new ConsoleChat());
    }
    private static void InitClient(IChat chat, out Client client)
    {
        string ip;
        int port;
        string name;

        do
        {
            chat.SendMessage("Enter IP: ", ConsoleColor.DarkGreen);
            ip = (string)chat.ReadMessage();
        }
        while (string.IsNullOrEmpty(ip));

        do
            chat.SendMessage("Enter Port: ", ConsoleColor.DarkGreen);
        while (!int.TryParse((string)chat.ReadMessage(), out port));

        do
        {
            chat.SendMessage("Enter Your Name: ", ConsoleColor.DarkGreen);
            name = (string)chat.ReadMessage();
        }
        while (string.IsNullOrEmpty(name));

        client = new(IPAddress.Parse(ip), port, name, chat);

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

