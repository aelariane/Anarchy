using UnityEngine;

namespace Cache.Bases.Impl
{
    public abstract class HeroBase : CharacterBase
    {
        public Transform _chest;
        public Transform _foreArm_Left;
        public Transform _foreArm_Right;
        public Transform _hand_Left;
        public Transform _hand_Right;
        public Transform _head;
        public Transform _hip;
        public Transform _neck;
        public Transform _shoulder_Left;
        public Transform _shoulder_Right;
        public Transform _spine;
        public Transform _upperArm_Left;
        public Transform _upperArm_Right;

        protected override void Cache()
        {
            base.Cache();
            _head = BaseTransorm.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
            _neck = BaseTransorm.Find("Amarture/Controller_Body/hip/spine/chest/neck");
            _chest = BaseTransorm.Find("Amarture/Controller_Body/hip/spine/chest");
            _shoulder_Left = BaseTransorm.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L");
            _shoulder_Right = BaseTransorm.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R");
            _hip = BaseTransorm.Find("Amarture/Controller_Body/hip");
            _spine = BaseTransorm.Find("Amarture/Controller_Body/hip/spine");
            _upperArm_Left = BaseTransorm.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L");
            _upperArm_Right = BaseTransorm.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
            _foreArm_Left = BaseTransorm.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L");
            _foreArm_Right = BaseTransorm.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R");
            _hand_Left = BaseTransorm.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L");
            _hand_Right = BaseTransorm.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R");
        }

        public Transform Chest => _chest;

        public Transform ForeArmLeft => _foreArm_Left;

        public Transform ForeArmRight => _foreArm_Right;

        public Transform HandLeft => _hand_Left;

        public Transform HandRight => _hand_Right;

        public Transform Head => _head;

        public Transform Hip => _hip;

        public Transform Neck => _neck;

        public Transform ShoulderLeft => _shoulder_Left;

        public Transform ShoulderRight => _shoulder_Right;

        public Transform Spine => _spine;

        public Transform UpperArmLeft => _upperArm_Left;

        public Transform UperArmRight => _upperArm_Right;

    }
}