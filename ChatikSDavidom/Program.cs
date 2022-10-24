using ChatikSDavidom.Components.Client;
using ChatikSDavidom.Components.Net;
using Common.Net.ConcretePackets;
using System.Net;

string ip = "127.0.0.1";
string port = "7777";


Client client = new(IPAddress.Parse(ip), int.Parse(port), $"Test {new Random().Next(0, 1000)}");
client.Send(new Welcome(client.Name));


int messageCount = 0;

while (client.Connected)
{
    await Task.Delay(5000);
    client.Send(new UserMessage(client.Name, $"{++messageCount}"));
}