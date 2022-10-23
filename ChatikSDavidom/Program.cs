using ChatikSDavidom.Components.Packet;

Packet packet = new Packet();

packet.Write("AAA");
packet.Write(123);

var integer = packet.ReadFromEnd<int>();
var str = packet.ReadFromEnd<string>();

Console.WriteLine(str);
Console.WriteLine(integer);