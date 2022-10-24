using ServerSide.Components;

Server server = new(7777, 2);
server.Start();

await Task.Delay(-1);