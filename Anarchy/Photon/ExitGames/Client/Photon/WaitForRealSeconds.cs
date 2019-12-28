using UnityEngine;

namespace ExitGames.Client.Photon
{
    public sealed class WaitForRealSeconds : CustomYieldInstruction
    {
        private readonly float endTime;

        public override bool keepWaiting
        {
            get { return this.endTime > Time.realtimeSinceStartup; }
        }

        public WaitForRealSeconds(float seconds)
        {
            this.endTime = Time.realtimeSinceStartup + seconds;
        }
    }
}