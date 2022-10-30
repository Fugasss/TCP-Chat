using Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Net.ConcretePackets
{
    public class ServerMessage : Packet
    {
        public readonly string Time;
        public readonly string Message;

        public ServerMessage(string message) : base(PacketType.ServerMessage)
        {
            Message = message;
            Time = DateTime.Now.ToString(Settings.DateFormat);

            Write(Time);
            Write(Message);
        }

        public ServerMessage(byte[] bytes) : base(bytes)
        {
            Time = ReadFromStart<string>();
            Message = ReadFromStart<string>();
        }

        public override string ToString()
        {
            return ToFormatter().ToString();
        }
        public override Formatter ToFormatter()
        {
            return new Formatter(Time, "SERVER", Message);
        }
    }
}
