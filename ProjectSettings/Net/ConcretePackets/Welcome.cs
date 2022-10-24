using ChatikSDavidom.Components.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return string.Format("{0}\t\t {1} was connected!", Time, Name);
        }
    }
}
