using Anarchy.Configuration;

namespace Anarchy.Notifications
{
    public class Notification
    {
        public static BoolSetting NotificationsEnabled = new BoolSetting(nameof(NotificationsEnabled), true);
        public static IntSetting NotificationSettings = new IntSetting(nameof(NotificationSettings), int.MaxValue);

    }
}