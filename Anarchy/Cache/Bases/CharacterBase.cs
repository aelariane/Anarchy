using UnityEngine;

namespace Cache.Bases
{
    public abstract class CharacterBase : MonoBehaviour
    {
        private Animation _baseA;
        private GameObject _baseG;
        private Transform _baseGOTransform;
        private Rigidbody _baseRigidBody;
        private Transform _baseTransform;

        protected virtual void Cache()
        {
            _baseA = base.animation;
            _baseG = base.gameObject;
            _baseGOTransform = _baseG.transform;
            _baseRigidBody = base.rigidbody;
            _baseTransform = base.transform;
        }

        public Animation BaseAnimation => _baseA;

        public GameObject BaseGameObject => _baseG;

        public Transform BaseGOTransform => _baseGOTransform;

        public Rigidbody BaseRigidBody => _baseRigidBody;

        public Transform BaseTransorm
        { 
            get => BaseTransorm;
            set => _baseTransform = value; 
        }
    }
}