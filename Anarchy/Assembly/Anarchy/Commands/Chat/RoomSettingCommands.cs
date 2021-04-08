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
            if(args.Length < 2)
            {
                chatMessage = Lang.Format("errRoom", args[0].ToLower());
                return false;
            }
            switch (args[0].ToLower())
            {

               case "hide":
                   {
                       PhotonNetwork.room.Visible = false;
                   }
                   break;

                   
               case "show":
                   {
                       PhotonNetwork.room.Visible = true;
                   }
                   break;

                   
               case "close":
                   {
                       PhotonNetwork.room.Open = false;
                   }
                   break;

                   
               case "open":
                   {
                       PhotonNetwork.room.Open = true;
                   }
                   break;

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
                    int time;
                    if(args[1].ToLower() == "set")
                    {
                        if(args.Length < 3)
                        {
                            chatMessage = Lang.Format("errRoom", args[0].ToLower());
                            return false;
                        }
                        if (!int.TryParse(args[2], out time))
                        {
                            chatMessage = Lang.Format("errArg", CommandName + " time");
                            return false;
                        }
                        FengGameManagerMKII.FGM.logic.ServerTime = time;
                        FengGameManagerMKII.FGM.logic.OnRequireStatus();
                        chatMessage = Lang.Format("roomTimeSet", time.ToString());
                        SendLocalizedText("roomTimeSet", new string[] { time.ToString() });
                        break;
                    }
                    if (!int.TryParse(args[1], out time))
                    {
                        chatMessage = Lang.Format("errArg", CommandName + " time");
                        return false;
                    }
                    FengGameManagerMKII.FGM.logic.ServerTime += time;
                    FengGameManagerMKII.FGM.logic.OnRequireStatus();
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
