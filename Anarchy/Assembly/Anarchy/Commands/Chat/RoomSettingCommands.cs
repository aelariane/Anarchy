namespace Anarchy.Commands.Chat
{
    internal class RoomSettingCommands : ChatCommand
    {
        public RoomSettingCommands() : base("room", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            string sendMessage = string.Empty;
            switch (args[0].ToLower())
            {
                case "max":
                    if(!int.TryParse(args[1], out int max))
                    {
                        chatMessage = Lang.Format("errArg", CommandName + " max");
                        return false;
                    }
                    PhotonNetwork.room.maxPlayers = max;
                    chatMessage = Lang.Format("roomMax", max.ToString());
                    sendMessage = English.Format("roomMax", max.ToString());
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
                    sendMessage = English.Format("roomTime", time.ToString());
                    break;

                default:
                    chatMessage = Lang.Format("errRoom", args[0].ToLower());    
                    return false;
            }
            FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.Others, new object[] { sendMessage, string.Empty });
            return true;
        }
    }
}
