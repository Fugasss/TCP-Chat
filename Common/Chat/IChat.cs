using Common.Messages;

namespace Common.Chat
{
    public interface IChat
    {
        object ReadMessage();
        void SendException(Exception e);
        void SendMessage(Formatter formatter, ConsoleColor color = ConsoleColor.White);
        void SendMessage(string message, ConsoleColor color = ConsoleColor.White);
    }
}