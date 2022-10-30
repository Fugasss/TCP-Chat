using Common.Messages;

namespace Common.Net.ConcretePackets
{
    public class Welcome : Packet
    {
        public readonly string Name;
        public readonly string Time;

        public Welcome(string name) : base(PacketType.Welcome)
        {
            Name = name;
            Time = DateTime.Now.ToString(Settings.DateFormat);

            Write(Name);
            Write(Time);
        }

        public Welcome(byte[] bytes) : base(bytes)
        {
            Name = ReadFromStart<string>();
            Time = ReadFromStart<string>();
        }

        public override string ToString()
        {
            return ToFormatter().ToString();
        }

        public override Formatter ToFormatter()
        {
            return new Formatter(Time, "Welcome", Name + " was connected!");
        }
    }
}
