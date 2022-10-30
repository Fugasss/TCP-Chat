using Common.Messages;

namespace Common.Chat
{
    public class ConsoleChat : IChat
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


        public void SendMessage(string message, ConsoleColor color = ConsoleColor.White)
        {
            var previousColor = MessageColor;

            MessageColor = color;
            Console.WriteLine(message);
            MessageColor = previousColor;

            for (int i = 0; i < SpacesBetweenMessages; i++)
                Console.WriteLine("\n");
        }
        public void SendMessage(Formatter formatter, ConsoleColor color = ConsoleColor.White)
        {
            SendMessage(formatter.ToString(), color);
        }
        public void SendException(Exception e)
        {
            SendMessage(new Formatter(DateTime.Now.ToString("HH:mm"), "EXCEPTION", $"Type: {e.GetType().Name}\t\t\t" + $"Location: {e.Source}\t\t\t" + $"{e.Message}"), ConsoleColor.Red);
        }

        public object ReadMessage()
        {
            Console.Write(SendCharacter + " ");
            return Console.ReadLine();
        }
    }
}
