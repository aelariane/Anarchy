using Anarchy;
using Optimization.Caching;
using RC;
using UnityEngine;

public class RacingCheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject gameObj = other.gameObject;
        var hero = gameObj.transform.root.gameObject.GetComponent<HERO>();
        if (hero == null)
        {
            return;
        }
        switch (gameObj.layer)
        {
            case Layers.PlayersN:
                if (CustomLevel.RacingCP.Count == 0 || CustomLevel.RacingCP.Peek() != this)
                {
                    CustomLevel.RacingCP.Push(this);
                    RCManager.racingSpawnPointRotation = gameObj.transform.rotation;
                    Anarchy.UI.Chat.Add($"<color=#{User.MainColor}>Checkpoint[<color=#{User.SubColor}>" + CustomLevel.RacingCP.Count + "</color>] set</color>");
                }
                gameObj.GetComponent<HERO>().FillGas();
                RCManager.racingSpawnPoint = base.gameObject.transform.position;
                RCManager.racingSpawnPointSet = true;
                break;

            case Layers.NetworkObjectN:
                hero.gasUsageTrack = 0;
                break;
        }
    }
}