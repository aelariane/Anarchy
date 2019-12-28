using ExitGames.Client.Photon;
using Optimization.Caching;
using UnityEngine;

public class BTN_choose_titan : MonoBehaviour
{
    private void OnClick()
    {
        if (IN_GAME_MAIN_CAMERA.GameMode == GameMode.PVP_AHSS)
        {
            string text = "AHSS";
            NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[0], true);
            FengGameManagerMKII.FGM.NeedChooseSide = false;
            if (!PhotonNetwork.IsMasterClient && FengGameManagerMKII.FGM.Logic.RoundTime > 60f)
            {
                FengGameManagerMKII.FGM.NOTSpawnPlayer(text);
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
            if ((!PhotonNetwork.IsMasterClient && FengGameManagerMKII.FGM.Logic.RoundTime > 60f) || FengGameManagerMKII.FGM.JustSuicide)
            {
                FengGameManagerMKII.FGM.JustSuicide = false;
                FengGameManagerMKII.FGM.NOTSpawnNonAITitan(selection);
            }
            else
            {
                FengGameManagerMKII.FGM.SpawnNonAITitan(selection, "titanRespawn");
            }
            FengGameManagerMKII.FGM.NeedChooseSide = false;
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