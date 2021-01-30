using Anarchy.NameAnimation;
using System.Collections.Generic;
using UnityEngine;

namespace Anarchy.Commands.Chat
{
    internal class AnimateNameCommand : ChatCommand
    {
        public AnimateNameCommand() : base("animate", false, false, true)
        {
        }

        public override bool Execute(string[] args)
        {
            Lerp lerp = new Lerp(PhotonNetwork.player.UIName)
            {
                Colors = new List<Color> { Color.magenta, Color.blue, Color.cyan, Color.blue, Color.magenta },
                Time = 5f
            };
            if (!lerp.Active)
            {
                lerp.Active = true;
                FengGameManagerMKII.FGM.StartCoroutine(lerp.Animate());
            }
            else
            {
                lerp.Active = false;
                FengGameManagerMKII.FGM.StopCoroutine(lerp.Animate());
            }
            return true;
        }
    }
}