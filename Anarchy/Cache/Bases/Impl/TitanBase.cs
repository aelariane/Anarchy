using UnityEngine;

namespace Cache.Bases.Impl
{
    public abstract class TitanBase : CharacterBase
    {
        private Transform _apFrontGround;
        private Transform _chkAeLeft;
        private Transform _chkAeLLeft;
        private Transform _chkAeLRight;
        private Transform _chkAeRight;
        private Transform _chkBackLeft;
        private Transform _chkBackRight;
        private Transform _chkFront;
        private Transform _chkFrontLeft;
        private Transform _chkFrontRight;
        private Transform _chkOverHead;

        private Transform _aabb;
        private Transform _chest;
        private Transform _core;
        private AudioSource _footAudio;
        private Transform _hand_Left_001;
        private SphereCollider _hand_Left_001_Sphere;
        private Transform _hand_Left_001_SphereTransform;
        private Transform _hand_Right_001;
        private SphereCollider _hand_Right_001_Sphere;
        private Transform _hand_Right_001_SphereTransform;
        private Transform _head;
        private Transform _hip;
        private Transform _neck;

        protected override void Cache()
        {
            base.Cache();
            if (BaseTransorm == null && (BaseTransorm = transform) == null)
            {
                return;
            }
            _aabb = BaseTransorm.Find(TransformPaths.AABB);
            _core = BaseTransorm.Find(TransformPaths.CORE);
            _chest = BaseTransorm.Find(TransformPaths.CHEST);
            Transform snd;
            if (snd = BaseTransorm.Find(TransformPaths.FOOT_AUDIO))
            {
                _footAudio = snd.GetComponent<AudioSource>();
            }
            if ((_hand_Left_001 = BaseTransorm.Find(TransformPaths.HAND_LEFT_001)) && (_hand_Left_001_Sphere = _hand_Left_001.GetComponent<SphereCollider>()))
            {
                _hand_Left_001_SphereTransform = _hand_Left_001_Sphere.transform;
            }
            if ((_hand_Right_001 = BaseTransorm.Find(TransformPaths.HAND_RIGHT_001)) && (_hand_Right_001_Sphere = _hand_Right_001.GetComponent<SphereCollider>()))
            {
                _hand_Right_001_SphereTransform = _hand_Right_001_Sphere.transform;
            }
            _head = BaseTransorm.Find(TransformPaths.HEAD);
            _hip = BaseTransorm.Find(TransformPaths.HIP);
            _neck = BaseTransorm.Find(TransformPaths.NECK);
            _chkFrontRight = BaseTransorm.Find("chkFrontRight");
            _chkFrontLeft = BaseTransorm.Find("chkFrontLeft");
            _chkBackRight = BaseTransorm.Find("chkBackRight");
            _chkBackLeft = BaseTransorm.Find("chkBackLeft");
            _chkOverHead = BaseTransorm.Find("chkOverHead");
            _chkFront = BaseTransorm.Find("chkFront");
            _chkAeLeft = BaseTransorm.Find("chkAeLeft");
            _chkAeLLeft = BaseTransorm.Find("chkAeLLeft");
            _chkAeRight = BaseTransorm.Find("chkAeRight");
            _chkAeLRight = BaseTransorm.Find("chkAeLRight");
            _apFrontGround = BaseTransorm.Find("ap_front_ground");
        }
    }
}