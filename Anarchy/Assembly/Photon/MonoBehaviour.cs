using UnityEngine;

namespace Photon
{
    public class MonoBehaviour : UnityEngine.MonoBehaviour
    {
        private PhotonView pv;
        internal PhotonView BasePV => pv ?? (pv = PhotonView.Get(this));

        //private void OnDestroy()
        //{
        //    CacheTransform.RemoveParent(transform);
        //}
    }
}