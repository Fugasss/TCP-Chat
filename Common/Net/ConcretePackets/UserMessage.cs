﻿using Common.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Net.ConcretePackets
{
    public class UserMessage : Packet
    {
        public readonly string Name;
        public readonly string Time;
        public readonly string Message;

        public UserMessage(string name, string message) : base(PacketType.ClientMessage)
        {
            Name = name;
            Message = message;
            Time = DateTime.Now.ToString(Settings.DateFormat);

            Write(Name);
            Write(Time);
            Write(Message);
        }

        public UserMessage(byte[] bytes) : base(bytes)
        {
            Name = ReadFromStart<string>();
            Time = ReadFromStart<string>();
            Message = ReadFromStart<string>();
        }

        public override string ToString()
        {
            return ToFormatter().ToString();
        }
        public override Formatter ToFormatter()
        {
            return new Formatter(Time, "< " + Name + " >", Message);
        }
    }
}