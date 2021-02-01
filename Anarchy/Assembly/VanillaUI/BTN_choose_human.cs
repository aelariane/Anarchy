using ExitGames.Client.Photon;
using Optimization.Caching;
using UnityEngine;

public class BTN_choose_human : MonoBehaviour
{
    private void OnClick()
    {
        string selection = CacheGameObject.Find("PopupListCharacterHUMAN").GetComponent<UIPopupList>().selection;
        NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[0], true);
        FengGameManagerMKII.FGM.needChooseSide = false;
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
        {
            FengGameManagerMKII.FGM.checkpoint = CacheGameObject.Find("PVPchkPtH");
        }
        if (!PhotonNetwork.IsMasterClient && FengGameManagerMKII.FGM.logic.RoundTime > 60f)
        {
            if (!this.isPlayerAllDead())
            {
                FengGameManagerMKII.FGM.NotSpawnPlayer(selection);
            }
            else
            {
                FengGameManagerMKII.FGM.NotSpawnPlayer(selection);
                FengGameManagerMKII.FGM.BasePV.RPC("restartGameByClient", PhotonTargets.MasterClient, new object[0]);
            }
        }
        else if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.BossFightCT || IN_GAME_MAIN_CAMERA.GameMode == GameMode.Trost || IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
        {
            if (this.isPlayerAllDead())
            {
                FengGameManagerMKII.FGM.NotSpawnPlayer(selection);
                FengGameManagerMKII.FGM.BasePV.RPC("restartGameByClient", PhotonTargets.MasterClient, new object[0]);
            }
            else
            {
                FengGameManagerMKII.FGM.SpawnPlayer(selection);
            }
        }
        else
        {
            FengGameManagerMKII.FGM.SpawnPlayer(selection);
        }
        NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[1], false);
        NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[2], false);
        NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[3], false);
        IN_GAME_MAIN_CAMERA.usingTitan = false;
        IN_GAME_MAIN_CAMERA.MainCamera.setHUDposition();
        Hashtable customProperties = new Hashtable
        {
            {
                PhotonPlayerProperty.character,
                selection
            }
        };
        PhotonNetwork.player.SetCustomProperties(customProperties);
    }

    public bool isPlayerAllDead()
    {
        int num = 0;
        int num2 = 0;
        foreach (PhotonPlayer photonPlayer in PhotonNetwork.playerList)
        {
            if ((int)photonPlayer.Properties[PhotonPlayerProperty.isTitan] == 1)
            {
                num++;
                if ((bool)photonPlayer.Properties[PhotonPlayerProperty.dead])
                {
                    num2++;
                }
            }
        }
        return num == num2;
    }
}