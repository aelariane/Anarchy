using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Anarchy.Commands.Chat
{
    internal class SpectateCommand : ChatCommand
    {
        internal static bool IsInSpecMode = false;

        public SpectateCommand() : base("spectate", false, true, false)
        {
        }

        public void EnterSpecMode(bool enter)
        {
            var spectateSprites = new List<GameObject>();
            if (enter)
            {
                foreach (GameObject gameObject in UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
                {
                    if (gameObject.GetComponent<UISprite>() != null && gameObject.activeInHierarchy)
                    {
                        string text = gameObject.name;
                        if (text.Contains("blade") || text.Contains("bullet") || text.Contains("gas") || text.Contains("flare") || text.Contains("skill_cd"))
                        {
                            if (!spectateSprites.Contains(gameObject))
                            {
                                spectateSprites.Add(gameObject);
                            }
                            gameObject.SetActive(false);
                        }
                    }
                }
                string[] array2 = new string[]
                {
                "Flare",
                "LabelInfoBottomRight"
                };
                foreach (string text2 in array2)
                {
                    GameObject gameObject2 = GameObject.Find(text2);
                    if (gameObject2 != null)
                    {
                        if (!spectateSprites.Contains(gameObject2))
                        {
                            spectateSprites.Add(gameObject2);
                        }
                        gameObject2.SetActive(false);
                    }
                }
                foreach (object obj in FengGameManagerMKII.Heroes)
                {
                    HERO hero = (HERO)obj;
                    if (hero.BasePV.IsMine) 
                    {
                        PhotonNetwork.Destroy(hero.BasePV);
                    }
                }
                if (PhotonNetwork.player.IsTitan && !PhotonNetwork.player.Dead)
                {
                    foreach (object obj2 in FengGameManagerMKII.Titans)
                    {
                        TITAN titan = (TITAN)obj2;
                        if (titan.BasePV.IsMine)
                        {
                            PhotonNetwork.Destroy(titan.BasePV);
                        }
                    }
                }
                NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], false);
                NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], false);
                NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], false);
                FengGameManagerMKII.FGM.NeedChooseSide = false;
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
                if (IN_GAME_MAIN_CAMERA.CameraMode == CameraType.ORIGINAL)
                {
                    Screen.lockCursor = false;
                    Screen.showCursor = false;
                }
                GameObject gameObject3 = GameObject.FindGameObjectWithTag("Player");
                bool flag8 = gameObject3 != null && gameObject3.GetComponent<HERO>() != null;
                if (flag8)
                {
                    Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetMainObject(gameObject3, true, false);
                }
                else
                {
                    Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetMainObject(null, true, false);
                }
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(false);
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                //base.StartCoroutine(this.reloadSky());
            }
            else
            {
                if (GameObject.Find("cross1") != null)
                {
                    GameObject.Find("cross1").transform.localPosition = Vector3.up * 5000f;
                }
                if (spectateSprites != null)
                {
                    foreach (GameObject gameObject4 in spectateSprites)
                    {
                        if (gameObject4 != null)
                        {
                            gameObject4.SetActive(true);
                        }
                    }
                }
                spectateSprites = new List<GameObject>();
                NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[1], false);
                NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[2], false);
                NGUITools.SetActive(GameObject.Find("UI_IN_GAME").GetComponent<UIReferArray>().panels[3], false);
                FengGameManagerMKII.FGM.NeedChooseSide = true;
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetMainObject(null, true, false);
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(true);
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            }
        }

        public override bool Execute(string[] args)
        {
            IsInSpecMode = !IsInSpecMode;
            EnterSpecMode(IsInSpecMode);
            chatMessage = Lang["specMode" + (IsInSpecMode ? "Enter" : "Quit")];
            return true;
        }
    }
}
    