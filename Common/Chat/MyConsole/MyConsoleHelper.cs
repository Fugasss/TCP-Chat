using Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Chat
{
    public static class MyConsoleHelper
    {
        public static void WriteLineException(Exception e) 
        {
            MyConsole.WriteLine(new Formatter("-", "EXCEPTION", e.Message), ConsoleColor.Red);
        }
    }
}
