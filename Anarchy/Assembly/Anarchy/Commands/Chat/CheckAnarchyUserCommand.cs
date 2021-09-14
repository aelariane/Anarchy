using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Anarchy.Network;

namespace Anarchy.Commands.Chat
{
    public class CheckAnarchyUserCommand : ChatCommand
    {
        public CheckAnarchyUserCommand() : base("check", false)
        {
        }

        public override bool Execute(string[] args)
        {
            if (args.Length < 1)
            {
                chatMessage = Lang["errArg"];
                return false;
            }

            int anarchyInt = 0;
            int abuseInt = 0;

            if (args[0] != "desc")
            {
                if (int.TryParse(args[0], out int playerId) == false)
                {
                    chatMessage = Lang["errArg"];
                    return false;
                }

                PhotonPlayer player = PhotonPlayer.Find(playerId);

                if (player.Properties.ContainsKey(PhotonPlayerProperty.anarchyFlags) == false && player.Properties.ContainsKey(PhotonPlayerProperty.anarchyAbuseFlags) == false)
                {
                    chatMessage = Lang["noAnarchyFeaturesFound"];
                    return true;
                }
                anarchyInt = (int)player.Properties[PhotonPlayerProperty.anarchyFlags];
                abuseInt = (int)player.Properties[PhotonPlayerProperty.anarchyAbuseFlags];

                if (anarchyInt == 0 && abuseInt == 0)
                {
                    chatMessage = Lang["noAnarchyFeaturesFound"];
                    return true;
                }
            }
            else
            {
                if (args[1] == "abuse")
                {
                    abuseInt = int.Parse(args[2]);
                }
                else
                {
                    anarchyInt = int.Parse(args[1]);
                }

            }

            if(anarchyInt > 0)
            {
                List<string> anarchyFeatures = new List<string>();
                if ((anarchyInt & (int)AnarchyFlags.DisableBodyLean) == (int)AnarchyFlags.DisableBodyLean)
                {
                    anarchyFeatures.Add(Lang["bodyLeanAbuse"]);
                }
                if ((anarchyInt & (int)AnarchyFlags.LegacyBurst) == (int)AnarchyFlags.LegacyBurst)
                {
                    anarchyFeatures.Add(Lang["legacyBurstAbuse"]);
                }
                if ((anarchyInt & (int)AnarchyFlags.NewTPSCamera) == (int)AnarchyFlags.NewTPSCamera)
                {
                    anarchyFeatures.Add(Lang["newTpsAbuse"]);
                }
                if((anarchyInt & (int)AnarchyFlags.NewTPSCamera) == (int)AnarchyFlags.NewTPSCamera)
                {
                    anarchyFeatures.Add(Lang["disableBurstCD"]);
                }
                chatMessage += Lang.Format("anarchyFeatures") + " " + string.Join(", ", anarchyFeatures.ToArray());
            }

            if (abuseInt > 0)
            {
                List<string> anarchyAbusiveFeatures = new List<string>();
                if((abuseInt & (int)AbuseFlags.InfiniteGasInPvp) == (int)AbuseFlags.InfiniteGasInPvp)
                {
                    anarchyAbusiveFeatures.Add(Lang["infGasInPvp"]);
                }
                if(chatMessage.Length > 0)
                {
                    chatMessage += "\n";
                }
                chatMessage += Lang.Format("anarchyAbusiveFeatures") + " " + string.Join(", ", anarchyAbusiveFeatures.ToArray());
            }

            return true;
        }
    }
}
