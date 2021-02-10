using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Notifications
{
    public enum NotificationEvent : int
    {
        LowGasLevel = 0b1,
        LowBladesLevel = 0b10,
        PersonalMessage = 0b100
    }
}
