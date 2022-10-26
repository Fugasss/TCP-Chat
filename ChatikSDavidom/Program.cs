using ChatikSDavidom.Components.Client;
using Common.Chat;
using Common.Net.ConcretePackets;
using System.Net;

string ip = "";
int port = 7777;
string name = "";

do
{
    Chat.SendMessage("Enter IP: ");
    ip = (string)Chat.ReadMessage();
}
while (string.IsNullOrEmpty(ip));

do
    Chat.SendMessage("Enter Port: ");
while (!int.TryParse((string)Chat.ReadMessage(), out port));

do
{
    Chat.SendMessage("Enter Your Name: ");
    name = (string)Chat.ReadMessage();
}
while (string.IsNullOrEmpty(name));

Client client = new(IPAddress.Parse(ip), port, name);

AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

client.Send(new Welcome(client.Name));

while (client.Connected)
{
    var message = (string)Chat.ReadMessage();
    if (string.IsNullOrEmpty(message)) continue;
    client.Send(new UserMessage(client.Name, message));
}

Chat.SendMessage("\t\tServer was closed!\t\t", ConsoleColor.Red);

void CurrentDomain_ProcessExit(object? sender, EventArgs e)
{
    client.Send(new Command(Commands.ClientDisconnect), () => client.Stop());
}