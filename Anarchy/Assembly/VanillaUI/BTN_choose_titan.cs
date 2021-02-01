using ExitGames.Client.Photon;
using Optimization.Caching;
using UnityEngine;

public class BTN_choose_titan : MonoBehaviour
{
    private void OnClick()
    {
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PvpAhss)
        {
            string text = "AHSS";
            NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[0], true);
            FengGameManagerMKII.FGM.needChooseSide = false;
            if (!PhotonNetwork.IsMasterClient && FengGameManagerMKII.FGM.logic.RoundTime > 60f)
            {
                FengGameManagerMKII.FGM.NotSpawnPlayer(text);
                FengGameManagerMKII.FGM.BasePV.RPC("restartGameByClient", PhotonTargets.MasterClient, new object[0]);
            }
            else
            {
                FengGameManagerMKII.FGM.SpawnPlayerAt(text, "playerRespawn2");
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
                    text
                }
            };
            PhotonNetwork.player.SetCustomProperties(customProperties);
        }
        else
        {
            if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_CAPTURE)
            {
                FengGameManagerMKII.FGM.checkpoint = CacheGameObject.Find("PVPchkPtT");
            }
            string selection = CacheGameObject.Find("PopupListCharacterTITAN").GetComponent<UIPopupList>().selection;
            NGUITools.SetActive(base.transform.parent.gameObject, false);
            NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[0], true);
            if ((!PhotonNetwork.IsMasterClient && FengGameManagerMKII.FGM.logic.RoundTime > 60f) || FengGameManagerMKII.FGM.justSuicide)
            {
                FengGameManagerMKII.FGM.justSuicide = false;
                FengGameManagerMKII.FGM.NotSpawnNonAiTitan(selection);
            }
            else
            {
                FengGameManagerMKII.FGM.SpawnNonAiTitan(selection, "titanRespawn");
            }
            FengGameManagerMKII.FGM.needChooseSide = false;
            NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[1], false);
            NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[2], false);
            NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[3], false);
            IN_GAME_MAIN_CAMERA.usingTitan = true;
            IN_GAME_MAIN_CAMERA.MainCamera.setHUDposition();
        }
    }

    private void Start()
    {
        if (!FengGameManagerMKII.Level.TeamTitan)
        {
            base.gameObject.GetComponent<UIButton>().isEnabled = false;
        }
    }
}