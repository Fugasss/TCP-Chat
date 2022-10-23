using ChatikSDavidom.Components.Packet;

Packet packet = new Packet();

packet.Write("Test");
packet.Write(123);
packet.Write(523);

var packet2 = new Packet(packet.GetBytes());

var integer = packet2.ReadFromEnd<int>();
var integer2 = packet2.ReadFromEnd<int>();
var str = packet2.ReadFromEnd<string>();

Console.WriteLine(str);
Console.WriteLine(integer);
Console.WriteLine(integer2);
