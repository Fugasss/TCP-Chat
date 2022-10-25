using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSide.Components
{
    internal static class Chat
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
        public static string SendCharacter { get; set; } = ">";


        public static void SendMessage(string message)
        {
            Console.WriteLine(message);

            for (int i = 0; i < SpacesBetweenMessages; i++)
                Console.WriteLine("\n");
        }

        public static object ReadMessage()
        {
            Console.Write(SendCharacter + " ");
            return Console.ReadLine();
        }


    }
}
