using Common.Messages;

namespace Common.Net.ConcretePackets
{
    public enum Commands : int
    {
        ClientDisconnect = 0,
        ConnectDeny,
        ServerStop,
    }

    public class Command : Packet
    {
        public readonly string Time;
        public readonly Commands CommandType;

        public Command(Commands command) : base(PacketType.Command)
        {
            CommandType = command;
            Time = DateTime.Now.ToString(Settings.DateFormat);

            Write((int)CommandType);
            Write(Time);
        }

        public Command(byte[] bytes) : base(bytes)
        {
            CommandType = (Commands)ReadFromStart<int>();
            Time = ReadFromStart<string>();
        }

        public override string ToString()
        {
            return ToFormatter().ToString();
        }

        public override Formatter ToFormatter()
        {
            return new Formatter(Time, CommandType.ToString(), "");
        }
    }
}
