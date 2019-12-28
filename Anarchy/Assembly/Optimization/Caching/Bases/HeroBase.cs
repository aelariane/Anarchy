using UnityEngine;

namespace Optimization.Caching.Bases
{
    public abstract class HeroBase : CharacterBase
    {
        public Transform Chest;
        public Transform Forearm_L;
        public Transform Forearm_R;
        public Transform Hand_L;
        public Transform Hand_R;
        public Transform Head;
        public Transform Hip;
        public Transform Neck;
        public Transform Shoulder_L;
        public Transform Shoulder_R;
        public Transform Spine;
        public Transform Upper_Arm_L;
        public Transform Upper_Arm_R;

        protected override void Cache()
        {
            base.Cache();
            Head = baseT.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
            Neck = baseT.Find("Amarture/Controller_Body/hip/spine/chest/neck");
            Chest = baseT.Find("Amarture/Controller_Body/hip/spine/chest");
            Shoulder_L = baseT.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L");
            Shoulder_R = baseT.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R");
            Hip = baseT.Find("Amarture/Controller_Body/hip");
            Spine = baseT.Find("Amarture/Controller_Body/hip/spine");
            Upper_Arm_L = baseT.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L");
            Upper_Arm_R = baseT.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
            Forearm_L = baseT.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L");
            Forearm_R = baseT.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R");
            Hand_L = baseT.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
            Hand_R = baseT.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
        }
    }
}