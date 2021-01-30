using System.Collections.Generic;
using UnityEngine;

namespace Anarchy.Commands.Chat
{
    internal class SpectateCommand : ChatCommand
    {
        internal static bool IsInSpecMode = false;
        private List<GameObject> spectateSprites = new List<GameObject>();

        public SpectateCommand() : base("spectate", false, true, false)
        {
        }

        public void EnterSpecMode(bool enter)
        {
            if (enter)
            {
                spectateSprites = new List<GameObject>();
                UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
                for (int i = 0; i < array.Length; i++)
                {
                    GameObject gameObject = (GameObject)array[i];
                    if (!(gameObject.GetComponent<UISprite>() != null) || !gameObject.activeInHierarchy)
                    {
                        continue;
                    }
                    string text = gameObject.name;
                    if (text.Contains("blade") || text.Contains("bullet") || text.Contains("gas") || text.Contains("flare") || text.Contains("skill_cd"))
                    {
                        if (!spectateSprites.Contains(gameObject))
                        {
                            spectateSprites.Add(gameObject);
                        }
                        gameObject.SetActive(value: false);
                    }
                }
                string[] array2 = new string[2]
                {
                "Flare",
                "LabelInfoBottomRight"
                };
                string[] array3 = array2;
                foreach (string text2 in array3)
                {
                    GameObject gameObject2 = GameObject.Find(text2);
                    if (gameObject2 != null)
                    {
                        if (!spectateSprites.Contains(gameObject2))
                        {
                            spectateSprites.Add(gameObject2);
                        }
                        gameObject2.SetActive(value: false);
                    }
                }
                foreach (HERO player in FengGameManagerMKII.Heroes)
                {
                    if (player.BasePV.IsMine)
                    {
                        PhotonNetwork.Destroy(player.BasePV);
                    }
                }
                if (PhotonNetwork.player.IsTitan && !PhotonNetwork.player.Dead)
                {
                    foreach (TITAN titan in FengGameManagerMKII.Titans)
                    {
                        if (titan.BasePV.IsMine)
                        {
                            PhotonNetwork.Destroy(titan.BasePV);
                        }
                    }
                }
                NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[1], state: false);
                NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[2], state: false);
                NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[3], state: false);
                FengGameManagerMKII.FGM.needChooseSide = false;
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
                if (IN_GAME_MAIN_CAMERA.CameraMode == CameraType.ORIGINAL)
                {
                    Screen.lockCursor = false;
                    Screen.showCursor = false;
                }
                GameObject gameObject3 = GameObject.FindGameObjectWithTag("Player");
                if (gameObject3 != null && gameObject3.GetComponent<HERO>() != null)
                {
                    Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetMainObject(gameObject3.GetComponent<HERO>());
                }
                else
                {
                    Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetMainObject(null);
                }
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(val: false);
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            }
            else
            {
                if (GameObject.Find("cross1") != null)
                {
                    GameObject.Find("cross1").transform.localPosition = Vector3.up * 5000f;
                }
                if (spectateSprites != null)
                {
                    foreach (GameObject spectateSprite in spectateSprites)
                    {
                        if (spectateSprite != null)
                        {
                            spectateSprite.SetActive(value: true);
                        }
                    }
                }
                spectateSprites = new List<GameObject>();
                NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[1], state: false);
                NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[2], state: false);
                NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[3], state: false);
                FengGameManagerMKII.FGM.needChooseSide = true;
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetMainObject(null);
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(val: true);
                Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            }
        }

        public override bool Execute(string[] args)
        {
            if (args.Length > 0)
            {
                if (int.TryParse(args[0], out int id))
                {
                    var player = PhotonPlayer.Find(id);
                    if (player == null || player.Dead)
                    {
                        return false;
                    }
                    var hero = player.GameObject?.GetComponent<HERO>();
                    if (hero == null)
                    {
                        return false;
                    }

                    IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(hero, true, false);
                    IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(false);
                    return true;
                }
                return false;
            }
            IsInSpecMode = !IsInSpecMode;
            EnterSpecMode(IsInSpecMode);
            chatMessage = Lang["specMode" + (IsInSpecMode ? "Enter" : "Quit")];
            return true;
        }
    }
}