﻿using ChatikSDavidom.Components.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Net.ConcretePackets
{
    public enum Commands : int
    {
        ClientDisconnect = 0,
        ServerStop,
    }

    public class Command : Packet
    {
        public readonly string Time;
        public readonly int CommandType;

        public Command(int command) : base(PacketType.Command)
        {
            CommandType = command;
            Time = DateTime.Now.ToString(Settings.DateFormat);

            Write(CommandType);
            Write(Time);
        }

        public Command(byte[] bytes) : base(bytes)
        {
            CommandType = ReadFromStart<int>();
            Time = ReadFromStart<string>();
        }

        public override string ToString()
        {
            return string.Format("{0}\t\t ~ {1} ~", Time, (Commands)CommandType);
        }
    }
}