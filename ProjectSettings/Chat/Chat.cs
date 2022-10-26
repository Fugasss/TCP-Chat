using Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Chat
{
    public static class Chat
    {
        public static ConsoleColor MessageColor
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }
        public static ConsoleColor BackgroundColor
        {
            get => Console.BackgroundColor;
            set => Console.BackgroundColor = value;
        }
        public static int SpacesBetweenMessages { get; private set; }
        public static string SendCharacter { get; set; } = "> ";


        public static void SendMessage(string message, ConsoleColor color = ConsoleColor.White)
        {
            var previousColor = MessageColor;

            MessageColor = color;
            Console.WriteLine(message);
            MessageColor = previousColor;

            for (int i = 0; i < SpacesBetweenMessages; i++)
                Console.WriteLine("\n");
        }
        public static void SendMessage(Formatter formatter, ConsoleColor color = ConsoleColor.White)
        {
            SendMessage(formatter.ToString(), color);
        }

        public static object ReadMessage()
        {
            Console.Write(SendCharacter + " ");
            return Console.ReadLine();
        }

        public static void SendException(Exception e)
        {
            SendMessage($"~~~~~~~~~EXCEPTION~~~~~~~~~\t\t\t" + $"\tType: {e.GetType().Name}\t\t\t" + $"Location: {e.Source}\t\t\t" + $"{e.Message}", ConsoleColor.Red);
        }
    }
}
