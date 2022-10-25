using ServerSide.Components;
using Common.Messages;
using Common.Chat;

int port = 80;
int maxConnections = 10;

do
    Chat.SendMessage("Enter listening port");
while (!int.TryParse((string)Chat.ReadMessage(), out port));

do
    Chat.SendMessage("Enter max connections");
while (!int.TryParse((string)Chat.ReadMessage(), out maxConnections));

Server server = new(port, maxConnections);
server.Start();

Server.Log(new Formatter(DateTime.Now.ToString("HH:mm"), "Server", "Server started"));

await Task.Delay(-1);