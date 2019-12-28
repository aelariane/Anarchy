using UnityEngine;

namespace Optimization.Caching.Bases
{
    public abstract class TitanBase : CharacterBase
    {
        #region Transform paths

        public const string AABBPath = "AABB";
        public const string ChestPath = "Amarture/Core/Controller_Body/hip/spine/chest";
        public const string CorePath = "Amarture/Core";
        public const string FootAudioPath = "snd_titan_foot";
        public const string Hand_L_001Path = "Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001";
        public const string Hand_R_001Path = "Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001";
        public const string HeadPath = "Amarture/Core/Controller_Body/hip/spine/chest/neck/head";
        public const string HipPath = "Amarture/Core/Controller_Body/hip";
        public const string NeckPath = "Amarture/Core/Controller_Body/hip/spine/chest/neck";

        #endregion Transform paths

        protected Transform ap_front_ground;
        protected Transform chkAeLeft;
        protected Transform chkAeLLeft;
        protected Transform chkAeLRight;
        protected Transform chkAeRight;
        protected Transform chkBackLeft;
        protected Transform chkBackRight;
        protected Transform chkFront;
        protected Transform chkFrontLeft;
        protected Transform chkFrontRight;
        protected Transform chkOverHead;
        public Transform AABB;
        public Transform Chest;
        public Transform Core;
        public AudioSource FootAudio;
        public Transform Hand_L_001;
        public SphereCollider Hand_L_001Sphere;
        public Transform Hand_L_001SphereT;
        public Transform Hand_R_001;
        public SphereCollider Hand_R_001Sphere;
        public Transform Hand_R_001SphereT;
        public Transform Head;
        public Transform Hip;
        public Transform Neck;

        protected override void Cache()
        {
            base.Cache();
            if (baseT == null && (baseT = transform) == null)
            {
                return;
            }
            AABB = baseT.Find(AABBPath);
            Core = baseT.Find(CorePath);
            Chest = baseT.Find(ChestPath);
            Transform snd;
            if (snd = baseT.Find(FootAudioPath))
            {
                FootAudio = snd.GetComponent<AudioSource>();
            }
            if ((Hand_L_001 = baseT.Find(Hand_L_001Path)) && (Hand_L_001Sphere = Hand_L_001.GetComponent<SphereCollider>()))
            {
                Hand_L_001SphereT = Hand_L_001Sphere.transform;
            }
            if ((Hand_R_001 = baseT.Find(Hand_R_001Path)) && (Hand_R_001Sphere = Hand_R_001.GetComponent<SphereCollider>()))
            {
                Hand_R_001SphereT = Hand_R_001Sphere.transform;
            }
            Head = baseT.Find(HeadPath);
            Hip = baseT.Find(HipPath);
            Neck = baseT.Find(NeckPath);
            chkFrontRight = baseT.Find("chkFrontRight");
            chkFrontLeft = baseT.Find("chkFrontLeft");
            chkBackRight = baseT.Find("chkBackRight");
            chkBackLeft = baseT.Find("chkBackLeft");
            chkOverHead = baseT.Find("chkOverHead");
            chkFront = baseT.Find("chkFront");
            chkAeLeft = baseT.Find("chkAeLeft");
            chkAeLLeft = baseT.Find("chkAeLLeft");
            chkAeRight = baseT.Find("chkAeRight");
            chkAeLRight = baseT.Find("chkAeLRight");
            ap_front_ground = baseT.Find("ap_front_ground");
        }
    }
}