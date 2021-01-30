using System.Collections.Generic;

namespace Anarchy.Replays
{
    public class Replay
    {
        public List<AnimationPlayInformation> AnimationCalls { get; set; }
        public List<ReplayFrame> Frames { get; set; }
        public List<ObjectOperationInformation> Operations { get; set; }
        public ReplayWorld World { get; set; }
    }
}