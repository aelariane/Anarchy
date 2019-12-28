using RC;
using UnityEngine;

public class LevelTriggerCheckPoint : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                RCManager.racingSpawnPointSet = true;
                RCManager.racingSpawnPoint = transform.position;
                RCManager.racingSpawnPointRotation = transform.rotation;
            }
            else if (other.gameObject.GetComponent<HERO>().BasePV.IsMine)
            {
                RCManager.racingSpawnPointSet = true;
                RCManager.racingSpawnPoint = transform.position;
                RCManager.racingSpawnPointRotation = transform.rotation;
            }
        }
    }
}