using UnityEngine;

public class LevelTriggerRacingEnd : MonoBehaviour
{
    private bool disable;

    private void OnTriggerStay(Collider other)
    {
        if (this.disable)
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
            {
                FengGameManagerMKII.FGM.GameWin();
                this.disable = true;
            }
            else if (other.gameObject.GetComponent<HERO>().BasePV.IsMine)
            {
                FengGameManagerMKII.FGM.MultiplayerRacingFinsih();
                this.disable = true;
            }
        }
    }

    private void Start()
    {
        this.disable = false;
    }
}