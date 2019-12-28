using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Commands.Chat
{
    internal class DaylightChangeCommand : ChatCommand
    {

        public DaylightChangeCommand() : base("light", false)
        {
        }

        public override bool Execute(string[] args)
        {
            int daylight = ((int)IN_GAME_MAIN_CAMERA.DayLight) + 1;
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "1":
                    case "day":
                        daylight = 0;
                        break;

                    case "2":
                    case "dawn":
                        daylight = 1;
                        break;

                    case "3":
                    case "night":
                        daylight = 2;
                        break;

                    default:
                        chatMessage = Lang.Format("errArg", CommandName);
                        return false;
                }
            }
            if (daylight > 2)
                daylight = 0;
            DayLight light = (DayLight)daylight;
            chatMessage = Lang.Format("daylightChange", light.ToString());
            IN_GAME_MAIN_CAMERA.MainCamera.setDayLight(light);
            return true;
        }
    }
}
