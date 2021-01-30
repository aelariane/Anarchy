using UnityEngine;

public class LevelTriggerGas : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                other.gameObject.GetComponent<HERO>().FillGas();
                UnityEngine.Object.Destroy(base.gameObject);
            }
            else if (other.gameObject.GetComponent<HERO>().BasePV.IsMine)
            {
                other.gameObject.GetComponent<HERO>().FillGas();
                UnityEngine.Object.Destroy(base.gameObject);
            }
            else
            {
                other.gameObject.GetComponent<HERO>().gasUsageTrack = 0f;
            }
        }
    }

    private void Start()
    {
    }
}