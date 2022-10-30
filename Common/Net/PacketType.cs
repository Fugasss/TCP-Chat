namespace Common.Net;

public enum PacketType : int
{
    Welcome = 0,
    ClientMessage,
    ServerMessage,
    Command
}
