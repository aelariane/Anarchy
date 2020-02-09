using Anarchy;
using RC;
using UnityEngine;

public class RacingCheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject gameObject = other.gameObject;
        if (gameObject.layer == 8)
        {
            gameObject = gameObject.transform.root.gameObject;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && gameObject.GetPhotonView() != null && gameObject.GetPhotonView().IsMine && gameObject.GetComponent<HERO>() != null)
            {
                if (CustomLevel.RacingCP.Count == 0 || CustomLevel.RacingCP.Peek() != this)
                {
                    CustomLevel.RacingCP.Push(this);
                    RCManager.racingSpawnPointRotation = gameObject.transform.rotation;
                    Anarchy.UI.Chat.Add($"<color=#{User.MainColor}>Checkpoint[<color=#{User.SubColor}>" + CustomLevel.RacingCP.Count + "</color>] set</color>");
                }
                gameObject.GetComponent<HERO>().fillGas();
                RCManager.racingSpawnPoint = base.gameObject.transform.position;
                RCManager.racingSpawnPointSet = true;
            }
        }
    }
}
