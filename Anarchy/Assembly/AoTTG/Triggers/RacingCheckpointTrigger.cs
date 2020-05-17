using Anarchy;
using RC;
using UnityEngine;

public class RacingCheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject gameObj = other.gameObject;
        if (gameObj.layer == 8)
        {
            gameObj = gameObj.transform.root.gameObject;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && gameObj.GetPhotonView() != null && gameObj.GetPhotonView().IsMine && gameObj.GetComponent<HERO>() != null)
            {
                if (CustomLevel.RacingCP.Count == 0 || CustomLevel.RacingCP.Peek() != this)
                {
                    CustomLevel.RacingCP.Push(this);
                    RCManager.racingSpawnPointRotation = gameObj.transform.rotation;
                    Anarchy.UI.Chat.Add($"<color=#{User.MainColor}>Checkpoint[<color=#{User.SubColor}>" + CustomLevel.RacingCP.Count + "</color>] set</color>");
                }
                gameObj.GetComponent<HERO>().FillGas();
                RCManager.racingSpawnPoint = base.gameObject.transform.position;
                RCManager.racingSpawnPointSet = true;
            }
        }
    }
}
