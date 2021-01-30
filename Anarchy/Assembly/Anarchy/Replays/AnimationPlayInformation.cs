using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ExitGames.Client.Photon;

namespace Anarchy.Replays
{
    public struct AnimationPlayInformation
    {
        public AnimationType Type { get; }
        public int Id { get; }
        public int AnimationId { get; }
        public float Time { get; }
        public float TimeArgument { get; }

        public AnimationPlayInformation(int animId, AnimationType type, int id, float time, float timeArg)
        {
            AnimationId = animId;
            Id = id;
            Time = time;
            Type = type;
            TimeArgument = timeArg;
        }

        public byte[] Serialize()
        {
            var result = new byte[Type > AnimationType.Play ? 17 : 13];

            int index = 0;
            Protocol.Serialize(Id, result, ref index);
            result[index++] = (byte)Type;
            Protocol.Serialize(AnimationId, result, ref index);
            Protocol.Serialize(Time, result, ref index);
            if (Type > AnimationType.Play)
            {
                Protocol.Serialize(TimeArgument, result, ref index);
            }

            return result;
        }
    }
}
