using ChatikSDavidom.Components.Client;
using ChatikSDavidom.Components.Net;
using ClientSide.Components;
using Common.Net.ConcretePackets;
using System.Net;

string ip = "127.0.0.1";
int port = 7777;
string name = "ABOBA";

Chat.SendMessage("Enter IP: ");
ip = (string)Chat.ReadMessage();

Chat.SendMessage("Enter Port: ");
port = int.Parse((string)Chat.ReadMessage());

Chat.SendMessage("Enter Your Name: ");
name = (string)Chat.ReadMessage();

Client client = new(IPAddress.Parse(ip), port, name);
client.Send(new Welcome(client.Name));

while (client.Connected)
{
    var message = (string)Chat.ReadMessage();
    if (string.IsNullOrEmpty(message)) continue;
    client.Send(new UserMessage(client.Name, message));
}