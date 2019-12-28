using UnityEngine;

namespace Optimization
{
    public class TitanTrigger : MonoBehaviour
    {
        public bool IsCollide;

        private void OnTriggerEnter(Collider other)
        {
            if (IsCollide) return;
            GameObject obj = other.transform.root.gameObject;
            if (obj.layer == 8)
            {
                GameObject myPlayer = IN_GAME_MAIN_CAMERA.MainObject;
                if (myPlayer != null && myPlayer == obj)
                {
                    this.IsCollide = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsCollide) return;
            GameObject obj = other.transform.root.gameObject;
            if (obj.layer == 8)
            {
                GameObject myPlayer = IN_GAME_MAIN_CAMERA.MainObject;
                if (myPlayer != null && myPlayer == obj)
                {
                    this.IsCollide = false;
                }
            }
        }
    }
}