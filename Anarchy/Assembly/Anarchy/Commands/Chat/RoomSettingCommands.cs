using System.Collections.Generic;

namespace Anarchy.Commands.Chat
{
    internal class RoomSettingCommands : ChatCommand
    {
        public RoomSettingCommands() : base("room", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            string sendKey = string.Empty;
            switch (args[0].ToLower())
            {
                case "max":
                    if(!int.TryParse(args[1], out int max))
                    {
                        chatMessage = Lang.Format("errArg", CommandName + " max");
                        return false;
                    }
                    PhotonNetwork.room.MaxPlayers = max;
                    chatMessage = Lang.Format("roomMax", max.ToString());
                    SendLocalizedText("roomMax", new string[] { max.ToString() });
                    break;

                case "time":
                    if (!int.TryParse(args[1], out int time))
                    {
                        chatMessage = Lang.Format("errArg", CommandName + " time");
                        return false;
                    }
                    FengGameManagerMKII.FGM.Logic.ServerTimeBase += time;
                    FengGameManagerMKII.FGM.Logic.ServerTime += time;
                    chatMessage = Lang.Format("roomTime", time.ToString());
                    SendLocalizedText("roomTime", new string[] { time.ToString() });
                    break;

                default:
                    chatMessage = Lang.Format("errRoom", args[0].ToLower());    
                    return false;
            }
            return true;
        }
    }
}
