using UnityEngine;

namespace Optimization.Caching.Bases
{
    public abstract class CharacterBase : Photon.MonoBehaviour
    {
        public Animation baseA;
        public GameObject baseG;
        public Transform baseGT;
        public Rigidbody baseR;
        public Transform baseT;

        public bool IsLocal { get; private set; }

        protected virtual void Cache()
        {
            baseA = base.animation;
            baseG = base.gameObject;
            baseGT = baseG.transform;
            baseR = base.rigidbody;
            baseT = base.transform;
            IsLocal = IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine;
        }
    }
}