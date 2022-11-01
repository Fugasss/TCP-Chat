namespace Common.Chat
{
    public static class MyConsole
    {
        private class Message
        {
            public object Text;
            public ConsoleColor Color;
        }

        private static int s_MinY;
        private static int s_MaxY;

        private static readonly List<Message> s_Lines;

        static MyConsole()
        {
            s_Lines = new();
        }

        public static void WriteLine(object value, ConsoleColor color = ConsoleColor.Gray)
        {
            var str = value.ToString();
            var count = str.Length;
            var iterations = count / Console.WindowWidth + 1;

            for (int i = 0; i < iterations; i++)
            {
                var startIndex = i * Console.WindowWidth;
                var length = Math.Clamp((i + 1) * Console.WindowWidth, 0, str.Length - startIndex);
                var substr = str.Substring(startIndex, length);


                s_Lines.Add(new Message { Text = substr.PadRight(Console.WindowWidth), Color = color });
            }

            Render();
        }

        private static void Render()
        {
            s_MinY = Console.WindowTop;
            s_MaxY = s_MinY + Console.WindowHeight - 1;

            var previousColor = Console.ForegroundColor;

            var i = 1;
            foreach (var line in s_Lines)
            {
                i++;
                Console.SetCursorPosition(0, Math.Clamp(s_MaxY - s_Lines.Count - 2 + i, 0, s_MaxY - 1));
                Console.ForegroundColor = line.Color;
                Console.WriteLine(line.Text);
            }

            Console.ForegroundColor = previousColor;
        }

        public static void Write(object message)
        {
            var height = Console.WindowHeight;
            int postition = height - 1;

            Console.CursorTop = postition;

            Console.Write(message);
        }

        public static object? ReadLine(string readCharacter = "> ")
        {
            var height = Console.WindowHeight;
            int position = Console.WindowTop + height - 1;
            Console.CursorTop = position;

            Console.Write(readCharacter);
            var read = Console.ReadLine();

            WriteLine(readCharacter + read);

            return read;
        }
    }
}
