using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Replays.Recorders
{
    public class ReplayAnimationRecorder
    {
        private ReplayWorld replayWorld;
        private int nextAnimId = 1;
        private Dictionary<string, int> animationsDictionary = new Dictionary<string, int>();
        private List<AnimationPlayInformation> playedAnimations = new List<AnimationPlayInformation>();
        public IDictionary<string, int> AnimationsDictionary => animationsDictionary;

        public ReplayAnimationRecorder(ReplayWorld world)
        {
            replayWorld = world;
        }

        public void RecordAnimationCall(ReplayGameObject go, AnimationType type, string animation, float time = 0f)
        {
            int animationId;
            if(!animationsDictionary.TryGetValue(animation, out animationId))
            {
                animationId = nextAnimId;
                animationsDictionary.Add(animation, nextAnimId++);
            }

            playedAnimations.Add(new AnimationPlayInformation(
                animationId,
                type,
                go.Id,
                FengGameManagerMKII.FGM.logic.RoundTime,
                time));
        }

        //This animations header presents something like shortcuts, list of used animations.
        public byte[] SerializeAnimationsHeader()
        {
            List<byte> bytes = new List<byte>();

            var bytesLength = new byte[2];
            int index = 0;

            ExitGames.Client.Photon.Protocol.Serialize((short)animationsDictionary.Count, bytesLength, ref index);

            foreach (var pair in animationsDictionary)
            {
                var stringBytes = Encoding.UTF8.GetBytes(pair.Key);
                bytes.Add((byte)stringBytes.Length);
                bytes.AddRange(stringBytes);
            }

            return bytes.ToArray();
        }

        //Serializes all happened animation calls
        public byte[] SerializeAnimationCalls()
        {
            List<byte> bytes = new List<byte>();

            bytes.Add((byte)animationsDictionary.Count);
            foreach(var pair in animationsDictionary)
            {
                var stringBytes = Encoding.UTF8.GetBytes(pair.Key);
                bytes.Add((byte)stringBytes.Length);
                bytes.AddRange(stringBytes);
            }

            var animsCountLength = new byte[4];
            int index = 0;
            ExitGames.Client.Photon.Protocol.Serialize(playedAnimations.Count, animsCountLength, ref index);
            bytes.AddRange(animsCountLength);

            foreach (var info in playedAnimations)
            {
                var animBytes = info.Serialize();
                bytes.AddRange(animBytes);
            }

            return bytes.ToArray();
        }
    }
}
