namespace Anarchy
{
    public class Notification
    {
        public static void Notify(string head, string message, Type type = Type.Message)
        {
        }
    }

    public enum Type
    {
        Message,
        Warning,
        Error
    }
}