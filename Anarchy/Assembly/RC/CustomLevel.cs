using Anarchy.Configuration;
using Anarchy.Configuration.Presets;
using Anarchy.UI;
using Optimization.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RC
{
    internal static class CustomLevel
    {
        public static string currentLevel = "";
        private static Anarchy.Skins.Maps.CustomMapSkin customMapSkin;
        public static string currentScript = "";
        public static string currentScriptLogic = "";
        public static bool customLevelLoaded = false;
        public static List<string[]> levelCache = new List<string[]>();
        public static bool logicLoaded = false;
        public static string oldScript = "";
        public static string oldScriptLogic = "";
        public static List<GameObject> groundList = new List<GameObject>();
        public static Stack<RacingCheckpointTrigger> RacingCP = new Stack<RacingCheckpointTrigger>();
        public static List<GameObject> racingDoors = new List<GameObject>();

        public static Dictionary<string, List<Vector3>> spawnPositions = new Dictionary<string, List<Vector3>>
        {
            {
                "Titan",
                new List<Vector3>()
            },
            {
                "PlayerC",
                new List<Vector3>()
            },
            {
                "PlayerM",
                new List<Vector3>()
            }
        };

        private static float updateTime = 1f;

        public static List<TitanSpawner> titanSpawners = new List<TitanSpawner>();
        private static bool unloading = false;

        public static void compileScript(string str)
        {
            string[] strArray2 = str.Replace(" ", string.Empty).Split(new string[]
            {
            "\n",
            "\r\n"
            }, System.StringSplitOptions.RemoveEmptyEntries);
            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
            int num = 0;
            int num2 = 0;
            bool flag = false;
            for (int num3 = 0; num3 < strArray2.Length; num3++)
            {
                if (strArray2[num3] == "{")
                {
                    num++;
                }
                else if (strArray2[num3] == "}")
                {
                    num2++;
                }
                else
                {
                    int num4 = 0;
                    int num5 = 0;
                    int num6 = 0;
                    foreach (char ch in strArray2[num3])
                    {
                        char c = ch;
                        if (c != '"')
                        {
                            switch (c)
                            {
                                case '(':
                                    num4++;
                                    break;

                                case ')':
                                    num5++;
                                    break;
                            }
                        }
                        else
                        {
                            num6++;
                        }
                    }
                    if (num4 != num5)
                    {
                        int num7 = num3 + 1;
                        Chat.Add("Script Error: Parentheses not equal! (line " + num7.ToString() + ")");
                        flag = true;
                    }
                    if (num6 % 2 != 0)
                    {
                        Chat.Add("Script Error: Quotations not equal! (line " + (num3 + 1).ToString() + ")");
                        flag = true;
                    }
                }
            }
            if (num != num2)
            {
                Chat.Add("Script Error: Bracket count not equivalent!");
                flag = true;
            }
            if (!flag)
            {
                try
                {
                    for (int num3 = 0; num3 < strArray2.Length; num3++)
                    {
                        if (strArray2[num3].StartsWith("On") && strArray2[num3 + 1] == "{")
                        {
                            int key = num3;
                            int num8 = num3 + 2;
                            int num9 = 0;
                            for (int i = num3 + 2; i < strArray2.Length; i++)
                            {
                                if (strArray2[i] == "{")
                                {
                                    num9++;
                                }
                                if (strArray2[i] == "}")
                                {
                                    if (num9 > 0)
                                    {
                                        num9--;
                                    }
                                    else
                                    {
                                        num8 = i - 1;
                                        i = strArray2.Length;
                                    }
                                }
                            }
                            hashtable.Add(key, num8);
                            num3 = num8;
                        }
                    }
                    foreach (object obj in hashtable.Keys)
                    {
                        int num10 = (int)obj;
                        string str2 = strArray2[num10];
                        int num8 = (int)hashtable[num10];
                        string[] stringArray = new string[num8 - num10 + 1];
                        int index = 0;
                        for (int num3 = num10; num3 <= num8; num3++)
                        {
                            stringArray[index] = strArray2[num3];
                            index++;
                        }
                        RCEvent event2 = parseBlock(stringArray, 0, 0, null);
                        if (str2.StartsWith("OnPlayerEnterRegion"))
                        {
                            int num11 = str2.IndexOf('[');
                            int num12 = str2.IndexOf(']');
                            string str3 = str2.Substring(num11 + 2, num12 - num11 - 3);
                            num11 = str2.IndexOf('(');
                            num12 = str2.IndexOf(')');
                            string str4 = str2.Substring(num11 + 2, num12 - num11 - 3);
                            if (RCManager.RCRegionTriggers.ContainsKey(str3))
                            {
                                RegionTrigger trigger = (RegionTrigger)RCManager.RCRegionTriggers[str3];
                                trigger.playerEventEnter = event2;
                                trigger.myName = str3;
                                RCManager.RCRegionTriggers[str3] = trigger;
                            }
                            else
                            {
                                RegionTrigger trigger = new RegionTrigger
                                {
                                    playerEventEnter = event2,
                                    myName = str3
                                };
                                RCManager.RCRegionTriggers.Add(str3, trigger);
                            }
                            RCManager.RCVariableNames.Add("OnPlayerEnterRegion[" + str3 + "]", str4);
                        }
                        else if (str2.StartsWith("OnPlayerLeaveRegion"))
                        {
                            int num11 = str2.IndexOf('[');
                            int num12 = str2.IndexOf(']');
                            string str3 = str2.Substring(num11 + 2, num12 - num11 - 3);
                            num11 = str2.IndexOf('(');
                            num12 = str2.IndexOf(')');
                            string str4 = str2.Substring(num11 + 2, num12 - num11 - 3);
                            if (RCManager.RCRegionTriggers.ContainsKey(str3))
                            {
                                RegionTrigger trigger = (RegionTrigger)RCManager.RCRegionTriggers[str3];
                                trigger.playerEventExit = event2;
                                trigger.myName = str3;
                                RCManager.RCRegionTriggers[str3] = trigger;
                            }
                            else
                            {
                                RegionTrigger trigger = new RegionTrigger
                                {
                                    playerEventExit = event2,
                                    myName = str3
                                };
                                RCManager.RCRegionTriggers.Add(str3, trigger);
                            }
                            RCManager.RCVariableNames.Add("OnPlayerExitRegion[" + str3 + "]", str4);
                        }
                        else if (str2.StartsWith("OnTitanEnterRegion"))
                        {
                            int num11 = str2.IndexOf('[');
                            int num12 = str2.IndexOf(']');
                            string str3 = str2.Substring(num11 + 2, num12 - num11 - 3);
                            num11 = str2.IndexOf('(');
                            num12 = str2.IndexOf(')');
                            string str4 = str2.Substring(num11 + 2, num12 - num11 - 3);
                            if (RCManager.RCRegionTriggers.ContainsKey(str3))
                            {
                                RegionTrigger trigger = (RegionTrigger)RCManager.RCRegionTriggers[str3];
                                trigger.titanEventEnter = event2;
                                trigger.myName = str3;
                                RCManager.RCRegionTriggers[str3] = trigger;
                            }
                            else
                            {
                                RegionTrigger trigger = new RegionTrigger
                                {
                                    titanEventEnter = event2,
                                    myName = str3
                                };
                                RCManager.RCRegionTriggers.Add(str3, trigger);
                            }
                            RCManager.RCVariableNames.Add("OnTitanEnterRegion[" + str3 + "]", str4);
                        }
                        else if (str2.StartsWith("OnTitanLeaveRegion"))
                        {
                            int num11 = str2.IndexOf('[');
                            int num12 = str2.IndexOf(']');
                            string str3 = str2.Substring(num11 + 2, num12 - num11 - 3);
                            num11 = str2.IndexOf('(');
                            num12 = str2.IndexOf(')');
                            string str4 = str2.Substring(num11 + 2, num12 - num11 - 3);
                            if (RCManager.RCRegionTriggers.ContainsKey(str3))
                            {
                                RegionTrigger trigger = (RegionTrigger)RCManager.RCRegionTriggers[str3];
                                trigger.titanEventExit = event2;
                                trigger.myName = str3;
                                RCManager.RCRegionTriggers[str3] = trigger;
                            }
                            else
                            {
                                RegionTrigger trigger = new RegionTrigger
                                {
                                    titanEventExit = event2,
                                    myName = str3
                                };
                                RCManager.RCRegionTriggers.Add(str3, trigger);
                            }
                            RCManager.RCVariableNames.Add("OnTitanExitRegion[" + str3 + "]", str4);
                        }
                        else if (str2.StartsWith("OnFirstLoad()"))
                        {
                            RCManager.RCEvents.Add("OnFirstLoad", event2);
                        }
                        else if (str2.StartsWith("OnRoundStart()"))
                        {
                            RCManager.RCEvents.Add("OnRoundStart", event2);
                        }
                        else if (str2.StartsWith("OnUpdate()"))
                        {
                            RCManager.RCEvents.Add("OnUpdate", event2);
                        }
                        else if (str2.StartsWith("OnTitanDie"))
                        {
                            int num11 = str2.IndexOf('(');
                            int num12 = str2.LastIndexOf(')');
                            string[] strArray3 = str2.Substring(num11 + 1, num12 - num11 - 1).Split(new char[]
                            {
                            ','
                            });
                            strArray3[0] = strArray3[0].Substring(1, strArray3[0].Length - 2);
                            strArray3[1] = strArray3[1].Substring(1, strArray3[1].Length - 2);
                            RCManager.RCVariableNames.Add("OnTitanDie", strArray3);
                            RCManager.RCEvents.Add("OnTitanDie", event2);
                        }
                        else if (str2.StartsWith("OnPlayerDieByTitan"))
                        {
                            RCManager.RCEvents.Add("OnPlayerDieByTitan", event2);
                            int num11 = str2.IndexOf('(');
                            int num12 = str2.LastIndexOf(')');
                            string[] strArray3 = str2.Substring(num11 + 1, num12 - num11 - 1).Split(new char[]
                            {
                            ','
                            });
                            strArray3[0] = strArray3[0].Substring(1, strArray3[0].Length - 2);
                            strArray3[1] = strArray3[1].Substring(1, strArray3[1].Length - 2);
                            RCManager.RCVariableNames.Add("OnPlayerDieByTitan", strArray3);
                        }
                        else if (str2.StartsWith("OnPlayerDieByPlayer"))
                        {
                            RCManager.RCEvents.Add("OnPlayerDieByPlayer", event2);
                            int num11 = str2.IndexOf('(');
                            int num12 = str2.LastIndexOf(')');
                            string[] strArray3 = str2.Substring(num11 + 1, num12 - num11 - 1).Split(new char[]
                            {
                            ','
                            });
                            strArray3[0] = strArray3[0].Substring(1, strArray3[0].Length - 2);
                            strArray3[1] = strArray3[1].Substring(1, strArray3[1].Length - 2);
                            RCManager.RCVariableNames.Add("OnPlayerDieByPlayer", strArray3);
                        }
                        else if (str2.StartsWith("OnChatInput"))
                        {
                            RCManager.RCEvents.Add("OnChatInput", event2);
                            int num11 = str2.IndexOf('(');
                            int num12 = str2.LastIndexOf(')');
                            string str4 = str2.Substring(num11 + 1, num12 - num11 - 1);
                            RCManager.RCVariableNames.Add("OnChatInput", str4.Substring(1, str4.Length - 2));
                        }
                    }
                }
                catch (UnityException exception)
                {
                    Chat.Add(exception.Message);
                }
            }
        }

        public static int conditionType(string str)
        {
            if (!str.StartsWith("Int"))
            {
                if (str.StartsWith("Bool"))
                {
                    return 1;
                }
                if (str.StartsWith("String"))
                {
                    return 2;
                }
                if (str.StartsWith("Float"))
                {
                    return 3;
                }
                if (str.StartsWith("Titan"))
                {
                    return 5;
                }
                if (str.StartsWith("Player"))
                {
                    return 4;
                }
            }
            return 0;
        }

        private static void customLevelClient(string[] content, bool renewHash)
        {
            bool flag = false;
            bool flag2 = false;
            if (content[content.Length - 1].StartsWith("a"))
            {
                flag = true;
            }
            else if (content[content.Length - 1].StartsWith("z"))
            {
                flag2 = true;
                logicLoaded = true;
                customLevelLoaded = true;
                SpawnPlayerCustomMap();
                Minimap.TryRecaptureInstance();
                unloadAssets();
                IN_GAME_MAIN_CAMERA.MainCamera.GetComponent<TiltShift>().enabled = false;
            }
            if (renewHash)
            {
                if (flag)
                {
                    currentLevel = string.Empty;
                    levelCache.Clear();
                    spawnPositions["Titan"].Clear();
                    spawnPositions["PlayerC"].Clear();
                    spawnPositions["PlayerM"].Clear();
                    racingDoors = new List<GameObject>();
                    for (int num = 0; num < content.Length; num++)
                    {
                        string[] strArray = content[num].Split(new char[]
                        {
                            ','
                        });
                        if (strArray[0] == "titan")
                        {
                            spawnPositions["Titan"].Add(new Vector3(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3])));
                        }
                        else if (strArray[0] == "playerC")
                        {
                            spawnPositions["PlayerC"].Add(new Vector3(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3])));
                        }
                        else if (strArray[0] == "playerM")
                        {
                            spawnPositions["PlayerM"].Add(new Vector3(Convert.ToSingle(strArray[1]), Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3])));
                        }
                    }
                    SpawnPlayerCustomMap();
                }
                currentLevel += content[content.Length - 1];
                levelCache.Add(content);
                ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                propertiesToSet.Add("currentLevel", currentLevel);
                PhotonNetwork.player.SetCustomProperties(propertiesToSet);
            }
            if (!flag && !flag2)
            {
                for (int num = 0; num < content.Length; num++)
                {
                    string[] strArray = content[num].Split(new char[]
                    {
                        ','
                    });
                    if (strArray[0].StartsWith("custom"))
                    {
                        float num3 = 1f;
                        GameObject obj2 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCManager.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[12]), Convert.ToSingle(strArray[13]), Convert.ToSingle(strArray[14])), new Quaternion(Convert.ToSingle(strArray[15]), Convert.ToSingle(strArray[16]), Convert.ToSingle(strArray[17]), Convert.ToSingle(strArray[18])));
                        if (strArray[2] != "default")
                        {
                            if (strArray[2].StartsWith("transparent"))
                            {
                                float num4;
                                if (float.TryParse(strArray[2].Substring(11), out num4))
                                {
                                    num3 = num4;
                                }
                                foreach (Renderer renderer in obj2.GetComponentsInChildren<Renderer>())
                                {
                                    renderer.material = (Material)RCManager.Load("transparent");
                                    if (Convert.ToSingle(strArray[10]) != 1f || Convert.ToSingle(strArray[11]) != 1f)
                                    {
                                        renderer.material.mainTextureScale = new Vector2(renderer.material.mainTextureScale.x * Convert.ToSingle(strArray[10]), renderer.material.mainTextureScale.y * Convert.ToSingle(strArray[11]));
                                    }
                                }
                            }
                            else
                            {
                                foreach (Renderer renderer2 in obj2.GetComponentsInChildren<Renderer>())
                                {
                                    renderer2.material = (Material)RCManager.Load(strArray[2]);
                                    if (Convert.ToSingle(strArray[10]) != 1f || Convert.ToSingle(strArray[11]) != 1f)
                                    {
                                        renderer2.material.mainTextureScale = new Vector2(renderer2.material.mainTextureScale.x * Convert.ToSingle(strArray[10]), renderer2.material.mainTextureScale.y * Convert.ToSingle(strArray[11]));
                                    }
                                }
                            }
                        }
                        float num5 = obj2.transform.localScale.x * Convert.ToSingle(strArray[3]);
                        num5 -= 0.001f;
                        float num6 = obj2.transform.localScale.y * Convert.ToSingle(strArray[4]);
                        float num7 = obj2.transform.localScale.z * Convert.ToSingle(strArray[5]);
                        obj2.transform.localScale = new Vector3(num5, num6, num7);
                        if (strArray[6] != "0")
                        {
                            Color color = new Color(Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), num3);
                            MeshFilter[] componentsInChildren2 = obj2.GetComponentsInChildren<MeshFilter>();
                            for (int i = 0; i < componentsInChildren2.Length; i++)
                            {
                                Mesh mesh = componentsInChildren2[i].mesh;
                                Color[] colorArray = new Color[mesh.vertexCount];
                                for (int num8 = 0; num8 < mesh.vertexCount; num8++)
                                {
                                    colorArray[num8] = color;
                                }
                                mesh.colors = colorArray;
                            }
                        }
                    }
                    else if (strArray[0].StartsWith("base"))
                    {
                        if (strArray.Length < 15)
                        {
                            UnityEngine.Object.Instantiate(Resources.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3]), Convert.ToSingle(strArray[4])), new Quaternion(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8])));
                        }
                        else
                        {
                            float num9 = 1f;
                            GameObject obj3 = (GameObject)UnityEngine.Object.Instantiate((GameObject)Resources.Load(strArray[1]), new Vector3(Convert.ToSingle(strArray[12]), Convert.ToSingle(strArray[13]), Convert.ToSingle(strArray[14])), new Quaternion(Convert.ToSingle(strArray[15]), Convert.ToSingle(strArray[16]), Convert.ToSingle(strArray[17]), Convert.ToSingle(strArray[18])));
                            if (strArray[2] != "default")
                            {
                                if (strArray[2].StartsWith("transparent"))
                                {
                                    float num10;
                                    if (float.TryParse(strArray[2].Substring(11), out num10))
                                    {
                                        num9 = num10;
                                    }
                                    foreach (Renderer renderer3 in obj3.GetComponentsInChildren<Renderer>())
                                    {
                                        renderer3.material = (Material)RCManager.Load("transparent");
                                        if (Convert.ToSingle(strArray[10]) != 1f || Convert.ToSingle(strArray[11]) != 1f)
                                        {
                                            renderer3.material.mainTextureScale = new Vector2(renderer3.material.mainTextureScale.x * Convert.ToSingle(strArray[10]), renderer3.material.mainTextureScale.y * Convert.ToSingle(strArray[11]));
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (Renderer renderer4 in obj3.GetComponentsInChildren<Renderer>())
                                    {
                                        if (!renderer4.name.Contains("Particle System") || !obj3.name.Contains("aot_supply"))
                                        {
                                            renderer4.material = (Material)RCManager.Load(strArray[2]);
                                            if (Convert.ToSingle(strArray[10]) != 1f || Convert.ToSingle(strArray[11]) != 1f)
                                            {
                                                renderer4.material.mainTextureScale = new Vector2(renderer4.material.mainTextureScale.x * Convert.ToSingle(strArray[10]), renderer4.material.mainTextureScale.y * Convert.ToSingle(strArray[11]));
                                            }
                                        }
                                    }
                                }
                            }
                            float num11 = obj3.transform.localScale.x * Convert.ToSingle(strArray[3]);
                            num11 -= 0.001f;
                            float num12 = obj3.transform.localScale.y * Convert.ToSingle(strArray[4]);
                            float num13 = obj3.transform.localScale.z * Convert.ToSingle(strArray[5]);
                            obj3.transform.localScale = new Vector3(num11, num12, num13);
                            if (strArray[6] != "0")
                            {
                                Color color2 = new Color(Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), num9);
                                MeshFilter[] componentsInChildren3 = obj3.GetComponentsInChildren<MeshFilter>();
                                for (int j = 0; j < componentsInChildren3.Length; j++)
                                {
                                    Mesh mesh2 = componentsInChildren3[j].mesh;
                                    Color[] colorArray2 = new Color[mesh2.vertexCount];
                                    for (int num14 = 0; num14 < mesh2.vertexCount; num14++)
                                    {
                                        colorArray2[num14] = color2;
                                    }
                                    mesh2.colors = colorArray2;
                                }
                            }
                        }
                    }
                    else if (strArray[0].StartsWith("misc"))
                    {
                        if (strArray[1].StartsWith("barrier"))
                        {
                            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(CacheResources.RCLoad(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                            float num15 = gameObject.transform.localScale.x * Convert.ToSingle(strArray[2]);
                            num15 -= 0.001f;
                            float num16 = gameObject.transform.localScale.y * Convert.ToSingle(strArray[3]);
                            float num17 = gameObject.transform.localScale.z * Convert.ToSingle(strArray[4]);
                            gameObject.transform.localScale = new Vector3(num15, num16, num17);
                        }
                        else if (strArray[1].StartsWith("racingStart"))
                        {
                            GameObject obj4 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.RCLoad(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                            float num18 = obj4.transform.localScale.x * Convert.ToSingle(strArray[2]);
                            num18 -= 0.001f;
                            float num19 = obj4.transform.localScale.y * Convert.ToSingle(strArray[3]);
                            float num20 = obj4.transform.localScale.z * Convert.ToSingle(strArray[4]);
                            obj4.transform.localScale = new Vector3(num18, num19, num20);
                            if (racingDoors != null)
                            {
                                racingDoors.Add(obj4);
                            }
                        }
                        else if (strArray[1].StartsWith("racingEnd"))
                        {
                            GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.RCLoad(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                            float num21 = gameObject2.transform.localScale.x * Convert.ToSingle(strArray[2]);
                            num21 -= 0.001f;
                            float num22 = gameObject2.transform.localScale.y * Convert.ToSingle(strArray[3]);
                            float num23 = gameObject2.transform.localScale.z * Convert.ToSingle(strArray[4]);
                            gameObject2.transform.localScale = new Vector3(num21, num22, num23);
                            gameObject2.AddComponent<LevelTriggerRacingEnd>();
                        }
                        else if (strArray[1].StartsWith("region") && PhotonNetwork.IsMasterClient)
                        {
                            Vector3 loc = new Vector3(Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8]));
                            RCRegion region = new RCRegion(loc, Convert.ToSingle(strArray[3]), Convert.ToSingle(strArray[4]), Convert.ToSingle(strArray[5]));
                            string key = strArray[2];
                            if (RCManager.RCRegionTriggers.ContainsKey(key))
                            {
                                GameObject obj5 = (GameObject)UnityEngine.Object.Instantiate((GameObject)RCManager.Load("region"));
                                obj5.transform.position = loc;
                                obj5.AddComponent<RegionTrigger>();
                                obj5.GetComponent<RegionTrigger>().CopyTrigger((RegionTrigger)RCManager.RCRegionTriggers[key]);
                                float num24 = obj5.transform.localScale.x * Convert.ToSingle(strArray[3]);
                                num24 -= 0.001f;
                                float num25 = obj5.transform.localScale.y * Convert.ToSingle(strArray[4]);
                                float num26 = obj5.transform.localScale.z * Convert.ToSingle(strArray[5]);
                                obj5.transform.localScale = new Vector3(num24, num25, num26);
                                region.myBox = obj5;
                            }
                            RCManager.RCRegions.Add(key, region);
                        }
                    }
                    else if (strArray[0].StartsWith("racing"))
                    {
                        if (strArray[1].StartsWith("start"))
                        {
                            GameObject obj6 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.RCLoad(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                            float num27 = obj6.transform.localScale.x * Convert.ToSingle(strArray[2]);
                            num27 -= 0.001f;
                            float num28 = obj6.transform.localScale.y * Convert.ToSingle(strArray[3]);
                            float num29 = obj6.transform.localScale.z * Convert.ToSingle(strArray[4]);
                            obj6.transform.localScale = new Vector3(num27, num28, num29);
                            if (racingDoors != null)
                            {
                                racingDoors.Add(obj6);
                            }
                        }
                        else if (strArray[1].StartsWith("end"))
                        {
                            GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.RCLoad(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                            float num30 = gameObject3.transform.localScale.x * Convert.ToSingle(strArray[2]);
                            num30 -= 0.001f;
                            float num31 = gameObject3.transform.localScale.y * Convert.ToSingle(strArray[3]);
                            float num32 = gameObject3.transform.localScale.z * Convert.ToSingle(strArray[4]);
                            gameObject3.transform.localScale = new Vector3(num30, num31, num32);
                            gameObject3.GetComponentInChildren<Collider>().gameObject.AddComponent<LevelTriggerRacingEnd>();
                        }
                        else if (strArray[1].StartsWith("kill"))
                        {
                            GameObject gameObject4 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.RCLoad(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                            float num33 = gameObject4.transform.localScale.x * Convert.ToSingle(strArray[2]);
                            num33 -= 0.001f;
                            float num34 = gameObject4.transform.localScale.y * Convert.ToSingle(strArray[3]);
                            float num35 = gameObject4.transform.localScale.z * Convert.ToSingle(strArray[4]);
                            gameObject4.transform.localScale = new Vector3(num33, num34, num35);
                            gameObject4.GetComponentInChildren<Collider>().gameObject.AddComponent<RacingKillTrigger>();
                        }
                        else if (strArray[1].StartsWith("checkpoint"))
                        {
                            GameObject gameObject5 = (GameObject)UnityEngine.Object.Instantiate(CacheResources.RCLoad(strArray[1]), new Vector3(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7])), new Quaternion(Convert.ToSingle(strArray[8]), Convert.ToSingle(strArray[9]), Convert.ToSingle(strArray[10]), Convert.ToSingle(strArray[11])));
                            float num36 = gameObject5.transform.localScale.x * Convert.ToSingle(strArray[2]);
                            num36 -= 0.001f;
                            float num37 = gameObject5.transform.localScale.y * Convert.ToSingle(strArray[3]);
                            float num38 = gameObject5.transform.localScale.z * Convert.ToSingle(strArray[4]);
                            gameObject5.transform.localScale = new Vector3(num36, num37, num38);
                            gameObject5.GetComponentInChildren<Collider>().gameObject.AddComponent<RacingCheckpointTrigger>();
                        }
                    }
                    else if (strArray[0].StartsWith("map"))
                    {
                        if (strArray[1].StartsWith("disablebounds"))
                        {
                            UnityEngine.Object.Destroy(GameObject.Find("gameobjectOutSide"));
                            UnityEngine.Object.Instantiate(RCManager.Load("outside"));
                        }
                    }
                    else if (PhotonNetwork.IsMasterClient && strArray[0].StartsWith("photon"))
                    {
                        if (strArray[1].StartsWith("Cannon"))
                        {
                            if (strArray.Length > 15)
                            {
                                GameObject go = PhotonNetwork.Instantiate("RCAsset/" + strArray[1] + "Prop", new Vector3(Convert.ToSingle(strArray[12]), Convert.ToSingle(strArray[13]), Convert.ToSingle(strArray[14])), new Quaternion(Convert.ToSingle(strArray[15]), Convert.ToSingle(strArray[0x10]), Convert.ToSingle(strArray[0x11]), Convert.ToSingle(strArray[0x12])), 0);
                                go.GetComponent<CannonPropRegion>().settings = content[num];
                                go.GetPhotonView().RPC("SetSize", PhotonTargets.AllBuffered, new object[] { content[num] });
                            }
                            else
                            {
                                PhotonNetwork.Instantiate("RCAsset/" + strArray[1] + "Prop", new Vector3(Convert.ToSingle(strArray[2]), Convert.ToSingle(strArray[3]), Convert.ToSingle(strArray[4])), new Quaternion(Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]), Convert.ToSingle(strArray[7]), Convert.ToSingle(strArray[8])), 0).GetComponent<CannonPropRegion>().settings = content[num];
                            }
                        }
                        else
                        {
                            TitanSpawner item = new TitanSpawner();
                            float num5 = 30f;
                            if (float.TryParse(strArray[2], out float num3))
                            {
                                num5 = Mathf.Max(Convert.ToSingle(strArray[2]), 1f);
                            }
                            item.time = num5;
                            item.delay = num5;
                            item.name = strArray[1];
                            if (strArray[3] == "1")
                            {
                                item.endless = true;
                            }
                            else
                            {
                                item.endless = false;
                            }
                            item.location = new Vector3(Convert.ToSingle(strArray[4]), Convert.ToSingle(strArray[5]), Convert.ToSingle(strArray[6]));
                            titanSpawners.Add(item);
                        }
                    }
                }
            }
        }

        public static IEnumerator LoadLevel()
        {
            if (currentLevel != string.Empty)
            {
                WaitForSeconds awaiter = new WaitForSeconds(0.25f);
                List<PhotonPlayer> cached = new List<PhotonPlayer>();
                List<PhotonPlayer> send = new List<PhotonPlayer>();
                foreach (PhotonPlayer player in PhotonNetwork.playerList)
                {
                    if (player.IsLocal)
                    {
                        continue;
                    }
                    if (player.CurrentLevel == currentLevel)
                    {
                        cached.Add(player);
                    }
                    else
                    {
                        send.Add(player);
                    }
                }
                if (cached.Count > 0)
                {
                    FengGameManagerMKII.FGM.BasePV.RPC("customlevelRPC", cached.ToArray(), new object[] { new string[] { "loadcached" } });
                }
                if (send.Count > 0)
                {
                    for (int i = 0; i < levelCache.Count; i++)
                    {
                        FengGameManagerMKII.FGM.BasePV.RPC("customlevelRPC", send.ToArray(), new object[] { levelCache[i] });
                        yield return awaiter;
                    }
                }
            }
            else
            {
                FengGameManagerMKII.FGM.BasePV.RPC("customlevelRPC", PhotonTargets.Others, new object[] { new string[] { "loadempty" } });
                customLevelLoaded = true;
            }
            yield break;
        }

        public static IEnumerator LoadLevelCache()
        {
            WaitForEndOfFrame awaiter = new WaitForEndOfFrame();
            for (int i = 0; i < levelCache.Count; i++)
            {
                customLevelClient(levelCache[i], false);
                yield return awaiter;
            }
            customLevelLoaded = true;
            yield break;
        }

        public static void OnUpdate()
        {
            if (RCManager.RCEvents.ContainsKey("OnUpdate"))
            {
                updateTime -= GameLogic.GameLogic.UpdateInterval;
                if (updateTime > 0f)
                {
                    return;
                } ((RCEvent)RCManager.RCEvents["OnUpdate"]).checkEvent();
                updateTime = 1f;
            }
        }

        public static int operantType(string str, int condition)
        {
            switch (condition)
            {
                case 0:
                case 3:
                    if (str.StartsWith("Equals"))
                    {
                        return 2;
                    }
                    if (str.StartsWith("NotEquals"))
                    {
                        return 5;
                    }
                    if (!str.StartsWith("LessThan"))
                    {
                        if (str.StartsWith("LessThanOrEquals"))
                        {
                            return 1;
                        }
                        if (str.StartsWith("GreaterThanOrEquals"))
                        {
                            return 3;
                        }
                        if (str.StartsWith("GreaterThan"))
                        {
                            return 4;
                        }
                    }
                    return 0;

                case 1:
                case 4:
                case 5:
                    if (str.StartsWith("Equals"))
                    {
                        return 2;
                    }
                    if (str.StartsWith("NotEquals"))
                    {
                        return 5;
                    }
                    return 0;

                case 2:
                    if (str.StartsWith("Equals"))
                    {
                        return 0;
                    }
                    if (str.StartsWith("NotEquals"))
                    {
                        return 1;
                    }
                    if (str.StartsWith("Contains"))
                    {
                        return 2;
                    }
                    if (str.StartsWith("NotContains"))
                    {
                        return 3;
                    }
                    if (str.StartsWith("StartsWith"))
                    {
                        return 4;
                    }
                    if (str.StartsWith("NotStartsWith"))
                    {
                        return 5;
                    }
                    if (str.StartsWith("EndsWith"))
                    {
                        return 6;
                    }
                    if (str.StartsWith("NotEndsWith"))
                    {
                        return 7;
                    }
                    return 0;

                default:
                    return 0;
            }
        }

        public static void OnLoadLevel()
        {
            FengGameManagerMKII.FGM.logic.RoundTime = 0f;
            RCManager.racingSpawnPoint = Vectors.zero;
            RCManager.racingSpawnPointSet = false;
            groundList = new List<GameObject>();
            logicLoaded = false;
            customLevelLoaded = IN_GAME_MAIN_CAMERA.GameType == GameType.Single || FengGameManagerMKII.Level.Name.StartsWith("Custom") == false;
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
            {
                updateTime = 1f;
                if (oldScriptLogic != currentScriptLogic)
                {
                    RCManager.ClearAll();
                    oldScriptLogic = currentScriptLogic;
                    compileScript(currentScriptLogic);
                    if (RCManager.RCEvents.ContainsKey("OnFirstLoad"))
                    {
                        ((RCEvent)RCManager.RCEvents["OnFirstLoad"]).checkEvent();
                    }
                }
                logicLoaded = true;
                if (RCManager.RCEvents.ContainsKey("OnRoundStart"))
                {
                    ((RCEvent)RCManager.RCEvents["OnRoundStart"]).checkEvent();
                }
            }
            else
            {
                logicLoaded = true;
            }
            if (FengGameManagerMKII.Level.Name.StartsWith("Custom") && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
            {
                GameObject supplyGO = GameObject.Find("aot_supply");
                if (supplyGO != null)
                {
                    Minimap.TrackGameObjectOnMinimap(supplyGO, Color.white, false, true, Minimap.IconStyle.Supply);
                }
                new LoadingMapScreen().Enable();
                GameObject[] array4 = GameObject.FindGameObjectsWithTag("playerRespawn");
                for (int i = 0; i < array4.Length; i++)
                {
                    array4[i].transform.position = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f));
                }
                foreach (GameObject obj2 in (GameObject[])UnityEngine.Object.FindObjectsOfType(typeof(GameObject)))
                {
                    if (obj2.name.Contains("TREE") || obj2.name.Contains("aot_supply"))
                    {
                        UnityEngine.Object.Destroy(obj2);
                    }
                    else if (obj2.name == "Cube_001" && obj2.transform.parent.gameObject.tag != "player" && obj2.renderer != null)
                    {
                        groundList.Add(obj2);
                        obj2.renderer.material.mainTexture = ((Material)RCManager.Load("grass")).mainTexture;
                    }
                }
                RacingCP = new Stack<RacingCheckpointTrigger>();
                racingDoors = new List<GameObject>();
                RCManager.allowedToCannon = new Dictionary<int, CannonValues>();
                if (!PhotonNetwork.IsMasterClient)
                {
                    if (SkinSettings.CustomSkins.Value == 2)
                    {
                        if (SkinSettings.CustomMapSet.Value != Anarchy.Configuration.StringSetting.NotDefine)
                        {
                            var set = new CustomMapPreset(SkinSettings.CustomMapSet.Value);
                            set.Load();
                            LoadSkin(set.ToSkinData(), null);
                        }
                    }
                }
                if (PhotonNetwork.IsMasterClient)
                {
                    RCManager.SpawnCapCustom.Value = Math.Min(50, RCManager.SpawnCapCustom.Value);
                    string[] customSkin = new string[7] { "", "", "", "", "", "", "" };
                    if (SkinSettings.CustomMapSet.Value != Anarchy.Configuration.StringSetting.NotDefine)
                    {
                        if (SkinSettings.CustomSkins.Value == 1)
                        {
                            var set = new CustomMapPreset(SkinSettings.CustomMapSet.Value);
                            set.Load();
                            customSkin = set.ToSkinData();
                        }
                    }
                    object[] args = new object[] { customSkin, (int)RCManager.GameType.ToValue() };
                    FengGameManagerMKII.FGM.BasePV.RPC("clearlevel", PhotonTargets.AllBuffered, args);
                    RCManager.RCRegions.Clear();
                    if (oldScript != currentScript)
                    {
                        levelCache.Clear();
                        spawnPositions["Titan"].Clear();
                        spawnPositions["PlayerC"].Clear();
                        spawnPositions["PlayerM"].Clear();
                        titanSpawners.Clear();
                        racingDoors.Clear();
                        currentLevel = string.Empty;
                        if (currentScript == string.Empty)
                        {
                            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
                            hashtable.Add("currentLevel", currentLevel);
                            PhotonNetwork.player.SetCustomProperties(hashtable);
                            oldScript = currentScript;
                        }
                        else
                        {
                            string[] strArray4 = System.Text.RegularExpressions.Regex.Replace(currentScript, "\\s+", "").Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Split(new char[]
                            {
                                ';'
                            });
                            for (int num = 0; num < Mathf.FloorToInt((float)((strArray4.Length - 1) / 100)) + 1; num++)
                            {
                                if (num < Mathf.FloorToInt((float)(strArray4.Length / 100)))
                                {
                                    string[] strArray5 = new string[101];
                                    int num2 = 0;
                                    for (int num3 = 100 * num; num3 < 100 * num + 100; num3++)
                                    {
                                        if (strArray4[num3].StartsWith("spawnpoint"))
                                        {
                                            string[] strArray6 = strArray4[num3].Split(new char[]
                                            {
                                                ','
                                            });
                                            if (strArray6[1] == "titan")
                                            {
                                                spawnPositions["Titan"].Add(new Vector3(Convert.ToSingle(strArray6[2]), Convert.ToSingle(strArray6[3]), Convert.ToSingle(strArray6[4])));
                                            }
                                            else if (strArray6[1] == "playerC")
                                            {
                                                spawnPositions["PlayerC"].Add(new Vector3(Convert.ToSingle(strArray6[2]), Convert.ToSingle(strArray6[3]), Convert.ToSingle(strArray6[4])));
                                            }
                                            else if (strArray6[1] == "playerM")
                                            {
                                                spawnPositions["PlayerM"].Add(new Vector3(Convert.ToSingle(strArray6[2]), Convert.ToSingle(strArray6[3]), Convert.ToSingle(strArray6[4])));
                                            }
                                        }
                                        strArray5[num2] = strArray4[num3];
                                        num2++;
                                    }
                                    string str5 = UnityEngine.Random.Range(10000, 99999).ToString();
                                    strArray5[100] = str5;
                                    currentLevel += str5;
                                    levelCache.Add(strArray5);
                                }
                                else
                                {
                                    string[] strArray7 = new string[strArray4.Length % 100 + 1];
                                    int num4 = 0;
                                    for (int num5 = 100 * num; num5 < 100 * num + strArray4.Length % 100; num5++)
                                    {
                                        if (strArray4[num5].StartsWith("spawnpoint"))
                                        {
                                            string[] strArray8 = strArray4[num5].Split(new char[]
                                            {
                                                ','
                                            });
                                            if (strArray8[1] == "titan")
                                            {
                                                spawnPositions["Titan"].Add(new Vector3(Convert.ToSingle(strArray8[2]), Convert.ToSingle(strArray8[3]), Convert.ToSingle(strArray8[4])));
                                            }
                                            else if (strArray8[1] == "playerC")
                                            {
                                                spawnPositions["PlayerC"].Add(new Vector3(Convert.ToSingle(strArray8[2]), Convert.ToSingle(strArray8[3]), Convert.ToSingle(strArray8[4])));
                                            }
                                            else if (strArray8[1] == "playerM")
                                            {
                                                spawnPositions["PlayerM"].Add(new Vector3(Convert.ToSingle(strArray8[2]), Convert.ToSingle(strArray8[3]), Convert.ToSingle(strArray8[4])));
                                            }
                                        }
                                        strArray7[num4] = strArray4[num5];
                                        num4++;
                                    }
                                    string str6 = UnityEngine.Random.Range(10000, 99999).ToString();
                                    strArray7[strArray4.Length % 100] = str6;
                                    currentLevel += str6;
                                    levelCache.Add(strArray7);
                                }
                            }
                            List<string> list = new List<string>();
                            foreach (Vector3 vector in spawnPositions["Titan"])
                            {
                                List<string> list2 = list;
                                string[] array = new string[6];
                                array[0] = "titan,";
                                string[] array6 = array;
                                int num6 = 1;
                                float x4 = vector.x;
                                array6[num6] = x4.ToString();
                                array[2] = ",";
                                string[] array7 = array;
                                int num7 = 3;
                                float y = vector.y;
                                array7[num7] = y.ToString();
                                array[4] = ",";
                                string[] array8 = array;
                                int num8 = 5;
                                float z = vector.z;
                                array8[num8] = z.ToString();
                                list2.Add(string.Concat(array));
                            }
                            foreach (Vector3 vector2 in spawnPositions["PlayerC"])
                            {
                                List<string> list3 = list;
                                string[] array2 = new string[6];
                                array2[0] = "playerC,";
                                string[] array9 = array2;
                                int num9 = 1;
                                float x2 = vector2.x;
                                array9[num9] = x2.ToString();
                                array2[2] = ",";
                                string[] array10 = array2;
                                int num10 = 3;
                                float y2 = vector2.y;
                                array10[num10] = y2.ToString();
                                array2[4] = ",";
                                string[] array11 = array2;
                                int num11 = 5;
                                float z2 = vector2.z;
                                array11[num11] = z2.ToString();
                                list3.Add(string.Concat(array2));
                            }
                            foreach (Vector3 vector3 in spawnPositions["PlayerM"])
                            {
                                List<string> list4 = list;
                                string[] array3 = new string[6];
                                array3[0] = "playerM,";
                                string[] array12 = array3;
                                int num12 = 1;
                                float x3 = vector3.x;
                                array12[num12] = x3.ToString();
                                array3[2] = ",";
                                string[] array13 = array3;
                                int num13 = 3;
                                float y3 = vector3.y;
                                array13[num13] = y3.ToString();
                                array3[4] = ",";
                                string[] array14 = array3;
                                int num14 = 5;
                                float z3 = vector3.z;
                                array14[num14] = z3.ToString();
                                list4.Add(string.Concat(array3));
                            }
                            string item = "a" + UnityEngine.Random.Range(10000, 99999).ToString();
                            list.Add(item);
                            currentLevel = item + currentLevel;
                            levelCache.Insert(0, list.ToArray());
                            string str7 = "z" + UnityEngine.Random.Range(10000, 99999).ToString();
                            levelCache.Add(new string[]
                            {
                                str7
                            });
                            currentLevel += str7;
                            ExitGames.Client.Photon.Hashtable hashtable2 = new ExitGames.Client.Photon.Hashtable();
                            hashtable2.Add("currentLevel", currentLevel);
                            PhotonNetwork.player.SetCustomProperties(hashtable2);
                            oldScript = currentScript;
                        }
                    }
                    FengGameManagerMKII.FGM.StartCoroutine(LoadLevel());
                    FengGameManagerMKII.FGM.StartCoroutine(LoadLevelCache());
                }
            }
        }

        public static void LoadSkin(string[] data, PhotonMessageInfo info)
        {
            if (data.Length != 7)
            {
                return;
            }
            if (customMapSkin == null)
            {
                customMapSkin = new Anarchy.Skins.Maps.CustomMapSkin(data);
            }
            Anarchy.Skins.Skin.Check(customMapSkin, data);
        }

        public static RCEvent parseBlock(string[] stringArray, int eventClass, int eventType, RCCondition condition)
        {
            System.Collections.Generic.List<RCAction> sentTrueActions = new System.Collections.Generic.List<RCAction>();
            RCEvent event2 = new RCEvent(null, null, 0, 0);
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (stringArray[i].StartsWith("If") && stringArray[i + 1] == "{")
                {
                    int num2 = i + 2;
                    int num3 = i + 2;
                    int num4 = 0;
                    for (int length = i + 2; length < stringArray.Length; length++)
                    {
                        if (stringArray[length] == "{")
                        {
                            num4++;
                        }
                        if (stringArray[length] == "}")
                        {
                            if (num4 > 0)
                            {
                                num4--;
                            }
                            else
                            {
                                num3 = length - 1;
                                length = stringArray.Length;
                            }
                        }
                    }
                    string[] strArray = new string[num3 - num2 + 1];
                    int num5 = 0;
                    for (int num6 = num2; num6 <= num3; num6++)
                    {
                        strArray[num5] = stringArray[num6];
                        num5++;
                    }
                    int index = stringArray[i].IndexOf("(");
                    int num7 = stringArray[i].LastIndexOf(")");
                    string str = stringArray[i].Substring(index + 1, num7 - index - 1);
                    int num8 = conditionType(str);
                    int num9 = str.IndexOf('.');
                    str = str.Substring(num9 + 1);
                    int num10 = operantType(str, num8);
                    index = str.IndexOf('(');
                    num7 = str.LastIndexOf(")");
                    string[] strArray2 = str.Substring(index + 1, num7 - index - 1).Split(new char[]
                    {
                    ','
                    });
                    RCCondition condition2 = new RCCondition(num10, num8, returnHelper(strArray2[0]), returnHelper(strArray2[1]));
                    RCEvent event3 = parseBlock(strArray, 1, 0, condition2);
                    RCAction action = new RCAction(0, 0, event3, null);
                    event2 = event3;
                    sentTrueActions.Add(action);
                    i = num3;
                }
                else if (stringArray[i].StartsWith("While") && stringArray[i + 1] == "{")
                {
                    int num2 = i + 2;
                    int num3 = i + 2;
                    int num4 = 0;
                    for (int length = i + 2; length < stringArray.Length; length++)
                    {
                        if (stringArray[length] == "{")
                        {
                            num4++;
                        }
                        if (stringArray[length] == "}")
                        {
                            if (num4 > 0)
                            {
                                num4--;
                            }
                            else
                            {
                                num3 = length - 1;
                                length = stringArray.Length;
                            }
                        }
                    }
                    string[] strArray = new string[num3 - num2 + 1];
                    int num5 = 0;
                    for (int num6 = num2; num6 <= num3; num6++)
                    {
                        strArray[num5] = stringArray[num6];
                        num5++;
                    }
                    int index = stringArray[i].IndexOf("(");
                    int num7 = stringArray[i].LastIndexOf(")");
                    string str = stringArray[i].Substring(index + 1, num7 - index - 1);
                    int num8 = conditionType(str);
                    int num9 = str.IndexOf('.');
                    str = str.Substring(num9 + 1);
                    int num10 = operantType(str, num8);
                    index = str.IndexOf('(');
                    num7 = str.LastIndexOf(")");
                    string[] strArray2 = str.Substring(index + 1, num7 - index - 1).Split(new char[]
                    {
                    ','
                    });
                    RCCondition condition2 = new RCCondition(num10, num8, returnHelper(strArray2[0]), returnHelper(strArray2[1]));
                    RCEvent event3 = parseBlock(strArray, 3, 0, condition2);
                    RCAction action = new RCAction(0, 0, event3, null);
                    sentTrueActions.Add(action);
                    i = num3;
                }
                else if (stringArray[i].StartsWith("ForeachTitan") && stringArray[i + 1] == "{")
                {
                    int num2 = i + 2;
                    int num3 = i + 2;
                    int num4 = 0;
                    for (int length = i + 2; length < stringArray.Length; length++)
                    {
                        if (stringArray[length] == "{")
                        {
                            num4++;
                        }
                        if (stringArray[length] == "}")
                        {
                            if (num4 > 0)
                            {
                                num4--;
                            }
                            else
                            {
                                num3 = length - 1;
                                length = stringArray.Length;
                            }
                        }
                    }
                    string[] strArray = new string[num3 - num2 + 1];
                    int num5 = 0;
                    for (int num6 = num2; num6 <= num3; num6++)
                    {
                        strArray[num5] = stringArray[num6];
                        num5++;
                    }
                    int index = stringArray[i].IndexOf("(");
                    int num7 = stringArray[i].LastIndexOf(")");
                    string str = stringArray[i].Substring(index + 2, num7 - index - 3);
                    int num8 = 0;
                    RCEvent event3 = parseBlock(strArray, 2, num8, null);
                    event3.foreachVariableName = str;
                    RCAction action = new RCAction(0, 0, event3, null);
                    sentTrueActions.Add(action);
                    i = num3;
                }
                else if (stringArray[i].StartsWith("ForeachPlayer") && stringArray[i + 1] == "{")
                {
                    int num2 = i + 2;
                    int num3 = i + 2;
                    int num4 = 0;
                    for (int length = i + 2; length < stringArray.Length; length++)
                    {
                        if (stringArray[length] == "{")
                        {
                            num4++;
                        }
                        if (stringArray[length] == "}")
                        {
                            if (num4 > 0)
                            {
                                num4--;
                            }
                            else
                            {
                                num3 = length - 1;
                                length = stringArray.Length;
                            }
                        }
                    }
                    string[] strArray = new string[num3 - num2 + 1];
                    int num5 = 0;
                    for (int num6 = num2; num6 <= num3; num6++)
                    {
                        strArray[num5] = stringArray[num6];
                        num5++;
                    }
                    int index = stringArray[i].IndexOf("(");
                    int num7 = stringArray[i].LastIndexOf(")");
                    string str = stringArray[i].Substring(index + 2, num7 - index - 3);
                    int num8 = 1;
                    RCEvent event3 = parseBlock(strArray, 2, num8, null);
                    event3.foreachVariableName = str;
                    RCAction action = new RCAction(0, 0, event3, null);
                    sentTrueActions.Add(action);
                    i = num3;
                }
                else if (stringArray[i].StartsWith("Else") && stringArray[i + 1] == "{")
                {
                    int num2 = i + 2;
                    int num3 = i + 2;
                    int num4 = 0;
                    for (int length = i + 2; length < stringArray.Length; length++)
                    {
                        if (stringArray[length] == "{")
                        {
                            num4++;
                        }
                        if (stringArray[length] == "}")
                        {
                            if (num4 > 0)
                            {
                                num4--;
                            }
                            else
                            {
                                num3 = length - 1;
                                length = stringArray.Length;
                            }
                        }
                    }
                    string[] strArray = new string[num3 - num2 + 1];
                    int num5 = 0;
                    for (int num6 = num2; num6 <= num3; num6++)
                    {
                        strArray[num5] = stringArray[num6];
                        num5++;
                    }
                    if (stringArray[i] == "Else")
                    {
                        RCEvent event3 = parseBlock(strArray, 0, 0, null);
                        RCAction action = new RCAction(0, 0, event3, null);
                        event2.setElse(action);
                        i = num3;
                    }
                    else if (stringArray[i].StartsWith("Else If"))
                    {
                        int index = stringArray[i].IndexOf("(");
                        int num7 = stringArray[i].LastIndexOf(")");
                        string str = stringArray[i].Substring(index + 1, num7 - index - 1);
                        int num8 = conditionType(str);
                        int num9 = str.IndexOf('.');
                        str = str.Substring(num9 + 1);
                        int num10 = operantType(str, num8);
                        index = str.IndexOf('(');
                        num7 = str.LastIndexOf(")");
                        string[] strArray2 = str.Substring(index + 1, num7 - index - 1).Split(new char[]
                        {
                        ','
                        });
                        RCCondition condition2 = new RCCondition(num10, num8, returnHelper(strArray2[0]), returnHelper(strArray2[1]));
                        RCEvent event3 = parseBlock(strArray, 1, 0, condition2);
                        RCAction action = new RCAction(0, 0, event3, null);
                        event2.setElse(action);
                        i = num3;
                    }
                }
                else if (stringArray[i].StartsWith("VariableInt"))
                {
                    int num11 = 1;
                    int num12 = stringArray[i].IndexOf('.');
                    int num13 = stringArray[i].IndexOf('(');
                    int num14 = stringArray[i].LastIndexOf(')');
                    string str2 = stringArray[i].Substring(num12 + 1, num13 - num12 - 1);
                    string[] strArray3 = stringArray[i].Substring(num13 + 1, num14 - num13 - 1).Split(new char[]
                    {
                    ','
                    });
                    if (str2.StartsWith("SetRandom"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCActionHelper helper3 = returnHelper(strArray3[2]);
                        RCAction action = new RCAction(num11, 12, null, new RCActionHelper[]
                        {
                        helper,
                        helper2,
                        helper3
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Set"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 0, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Add"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 1, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Subtract"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 2, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Multiply"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 3, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Divide"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 4, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Modulo"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 5, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Power"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 6, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("VariableBool"))
                {
                    int num11 = 2;
                    int num12 = stringArray[i].IndexOf('.');
                    int num13 = stringArray[i].IndexOf('(');
                    int num14 = stringArray[i].LastIndexOf(')');
                    string str2 = stringArray[i].Substring(num12 + 1, num13 - num12 - 1);
                    string[] strArray3 = stringArray[i].Substring(num13 + 1, num14 - num13 - 1).Split(new char[]
                    {
                    ','
                    });
                    if (str2.StartsWith("SetToOpposite"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCAction action = new RCAction(num11, 11, null, new RCActionHelper[]
                        {
                        helper
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetRandom"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCAction action = new RCAction(num11, 12, null, new RCActionHelper[]
                        {
                        helper
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Set"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 0, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("VariableString"))
                {
                    int num11 = 3;
                    int num12 = stringArray[i].IndexOf('.');
                    int num13 = stringArray[i].IndexOf('(');
                    int num14 = stringArray[i].LastIndexOf(')');
                    string str2 = stringArray[i].Substring(num12 + 1, num13 - num12 - 1);
                    string[] strArray3 = stringArray[i].Substring(num13 + 1, num14 - num13 - 1).Split(new char[]
                    {
                    ','
                    });
                    if (str2.StartsWith("Set"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 0, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Concat"))
                    {
                        RCActionHelper[] helpers = new RCActionHelper[strArray3.Length];
                        for (int length = 0; length < strArray3.Length; length++)
                        {
                            helpers[length] = returnHelper(strArray3[length]);
                        }
                        RCAction action = new RCAction(num11, 7, null, helpers);
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Append"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 8, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Replace"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCActionHelper helper3 = returnHelper(strArray3[2]);
                        RCAction action = new RCAction(num11, 10, null, new RCActionHelper[]
                        {
                        helper,
                        helper2,
                        helper3
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Remove"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 9, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("VariableFloat"))
                {
                    int num11 = 4;
                    int num12 = stringArray[i].IndexOf('.');
                    int num13 = stringArray[i].IndexOf('(');
                    int num14 = stringArray[i].LastIndexOf(')');
                    string str2 = stringArray[i].Substring(num12 + 1, num13 - num12 - 1);
                    string[] strArray3 = stringArray[i].Substring(num13 + 1, num14 - num13 - 1).Split(new char[]
                    {
                    ','
                    });
                    if (str2.StartsWith("SetRandom"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCActionHelper helper3 = returnHelper(strArray3[2]);
                        RCAction action = new RCAction(num11, 12, null, new RCActionHelper[]
                        {
                        helper,
                        helper2,
                        helper3
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Set"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 0, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Add"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 1, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Subtract"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 2, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Multiply"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 3, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Divide"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 4, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Modulo"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 5, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Power"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 6, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("VariablePlayer"))
                {
                    int num11 = 5;
                    int num12 = stringArray[i].IndexOf('.');
                    int num13 = stringArray[i].IndexOf('(');
                    int num14 = stringArray[i].LastIndexOf(')');
                    string str2 = stringArray[i].Substring(num12 + 1, num13 - num12 - 1);
                    string[] strArray3 = stringArray[i].Substring(num13 + 1, num14 - num13 - 1).Split(new char[]
                    {
                    ','
                    });
                    if (str2.StartsWith("Set"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 0, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("VariableTitan"))
                {
                    int num11 = 6;
                    int num12 = stringArray[i].IndexOf('.');
                    int num13 = stringArray[i].IndexOf('(');
                    int num14 = stringArray[i].LastIndexOf(')');
                    string str2 = stringArray[i].Substring(num12 + 1, num13 - num12 - 1);
                    string[] strArray3 = stringArray[i].Substring(num13 + 1, num14 - num13 - 1).Split(new char[]
                    {
                    ','
                    });
                    if (str2.StartsWith("Set"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 0, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("Player"))
                {
                    int num11 = 7;
                    int num12 = stringArray[i].IndexOf('.');
                    int num13 = stringArray[i].IndexOf('(');
                    int num14 = stringArray[i].LastIndexOf(')');
                    string str2 = stringArray[i].Substring(num12 + 1, num13 - num12 - 1);
                    string[] strArray3 = stringArray[i].Substring(num13 + 1, num14 - num13 - 1).Split(new char[]
                    {
                    ','
                    });
                    if (str2.StartsWith("KillPlayer"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 0, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SpawnPlayerAt"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCActionHelper helper3 = returnHelper(strArray3[2]);
                        RCActionHelper helper4 = returnHelper(strArray3[3]);
                        RCAction action = new RCAction(num11, 2, null, new RCActionHelper[]
                        {
                        helper,
                        helper2,
                        helper3,
                        helper4
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SpawnPlayer"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCAction action = new RCAction(num11, 1, null, new RCActionHelper[]
                        {
                        helper
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("MovePlayer"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCActionHelper helper3 = returnHelper(strArray3[2]);
                        RCActionHelper helper4 = returnHelper(strArray3[3]);
                        RCAction action = new RCAction(num11, 3, null, new RCActionHelper[]
                        {
                        helper,
                        helper2,
                        helper3,
                        helper4
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetKills"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 4, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetDeaths"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 5, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetMaxDmg"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 6, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetTotalDmg"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 7, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetName"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 8, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetGuildName"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 9, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetTeam"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 10, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetCustomInt"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 11, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetCustomBool"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 12, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetCustomString"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 13, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetCustomFloat"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 14, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("Titan"))
                {
                    int num11 = 8;
                    int num12 = stringArray[i].IndexOf('.');
                    int num13 = stringArray[i].IndexOf('(');
                    int num14 = stringArray[i].LastIndexOf(')');
                    string str2 = stringArray[i].Substring(num12 + 1, num13 - num12 - 1);
                    string[] strArray3 = stringArray[i].Substring(num13 + 1, num14 - num13 - 1).Split(new char[]
                    {
                    ','
                    });
                    if (str2.StartsWith("KillTitan"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCActionHelper helper3 = returnHelper(strArray3[2]);
                        RCAction action = new RCAction(num11, 0, null, new RCActionHelper[]
                        {
                        helper,
                        helper2,
                        helper3
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SpawnTitanAt"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCActionHelper helper3 = returnHelper(strArray3[2]);
                        RCActionHelper helper4 = returnHelper(strArray3[3]);
                        RCActionHelper helper5 = returnHelper(strArray3[4]);
                        RCActionHelper helper6 = returnHelper(strArray3[5]);
                        RCActionHelper helper7 = returnHelper(strArray3[6]);
                        RCAction action = new RCAction(num11, 2, null, new RCActionHelper[]
                        {
                        helper,
                        helper2,
                        helper3,
                        helper4,
                        helper5,
                        helper6,
                        helper7
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SpawnTitan"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCActionHelper helper3 = returnHelper(strArray3[2]);
                        RCActionHelper helper4 = returnHelper(strArray3[3]);
                        RCAction action = new RCAction(num11, 1, null, new RCActionHelper[]
                        {
                        helper,
                        helper2,
                        helper3,
                        helper4
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("SetHealth"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCAction action = new RCAction(num11, 3, null, new RCActionHelper[]
                        {
                        helper,
                        helper2
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("MoveTitan"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCActionHelper helper2 = returnHelper(strArray3[1]);
                        RCActionHelper helper3 = returnHelper(strArray3[2]);
                        RCActionHelper helper4 = returnHelper(strArray3[3]);
                        RCAction action = new RCAction(num11, 4, null, new RCActionHelper[]
                        {
                        helper,
                        helper2,
                        helper3,
                        helper4
                        });
                        sentTrueActions.Add(action);
                    }
                }
                else if (stringArray[i].StartsWith("Game"))
                {
                    int num11 = 9;
                    int num12 = stringArray[i].IndexOf('.');
                    int num13 = stringArray[i].IndexOf('(');
                    int num14 = stringArray[i].LastIndexOf(')');
                    string str2 = stringArray[i].Substring(num12 + 1, num13 - num12 - 1);
                    string[] strArray3 = stringArray[i].Substring(num13 + 1, num14 - num13 - 1).Split(new char[]
                    {
                    ','
                    });
                    if (str2.StartsWith("PrintMessage"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCAction action = new RCAction(num11, 0, null, new RCActionHelper[]
                        {
                        helper
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("LoseGame"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCAction action = new RCAction(num11, 2, null, new RCActionHelper[]
                        {
                        helper
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("WinGame"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCAction action = new RCAction(num11, 1, null, new RCActionHelper[]
                        {
                        helper
                        });
                        sentTrueActions.Add(action);
                    }
                    else if (str2.StartsWith("Restart"))
                    {
                        RCActionHelper helper = returnHelper(strArray3[0]);
                        RCAction action = new RCAction(num11, 3, null, new RCActionHelper[]
                        {
                        helper
                        });
                        sentTrueActions.Add(action);
                    }
                }
            }
            return new RCEvent(condition, sentTrueActions, eventClass, eventType);
        }

        public static RCActionHelper returnHelper(string str)
        {
            string[] strArray = str.Split(new char[]
            {
            '.'
            });
            float num;
            if (float.TryParse(str, out num))
            {
                strArray = new string[]
                {
                str
                };
            }
            System.Collections.Generic.List<RCActionHelper> list = new System.Collections.Generic.List<RCActionHelper>();
            int sentType = 0;
            for (int num2 = 0; num2 < strArray.Length; num2++)
            {
                if (list.Count == 0)
                {
                    string str2 = strArray[num2];
                    int num3;
                    float num4;
                    if (str2.StartsWith("\"") && str2.EndsWith("\""))
                    {
                        RCActionHelper helper = new RCActionHelper(0, 0, str2.Substring(1, str2.Length - 2));
                        list.Add(helper);
                        sentType = 2;
                    }
                    else if (int.TryParse(str2, out num3))
                    {
                        RCActionHelper helper = new RCActionHelper(0, 0, num3);
                        list.Add(helper);
                        sentType = 0;
                    }
                    else if (float.TryParse(str2, out num4))
                    {
                        RCActionHelper helper = new RCActionHelper(0, 0, num4);
                        list.Add(helper);
                        sentType = 3;
                    }
                    else if (str2.ToLower() == "true" || str2.ToLower() == "false")
                    {
                        RCActionHelper helper = new RCActionHelper(0, 0, System.Convert.ToBoolean(str2.ToLower()));
                        list.Add(helper);
                        sentType = 1;
                    }
                    else if (str2.StartsWith("Variable"))
                    {
                        int index = str2.IndexOf('(');
                        int num5 = str2.LastIndexOf(')');
                        if (str2.StartsWith("VariableInt"))
                        {
                            str2 = str2.Substring(index + 1, num5 - index - 1);
                            RCActionHelper helper = new RCActionHelper(1, 0, returnHelper(str2));
                            list.Add(helper);
                            sentType = 0;
                        }
                        else if (str2.StartsWith("VariableBool"))
                        {
                            str2 = str2.Substring(index + 1, num5 - index - 1);
                            RCActionHelper helper = new RCActionHelper(1, 1, returnHelper(str2));
                            list.Add(helper);
                            sentType = 1;
                        }
                        else if (str2.StartsWith("VariableString"))
                        {
                            str2 = str2.Substring(index + 1, num5 - index - 1);
                            RCActionHelper helper = new RCActionHelper(1, 2, returnHelper(str2));
                            list.Add(helper);
                            sentType = 2;
                        }
                        else if (str2.StartsWith("VariableFloat"))
                        {
                            str2 = str2.Substring(index + 1, num5 - index - 1);
                            RCActionHelper helper = new RCActionHelper(1, 3, returnHelper(str2));
                            list.Add(helper);
                            sentType = 3;
                        }
                        else if (str2.StartsWith("VariablePlayer"))
                        {
                            str2 = str2.Substring(index + 1, num5 - index - 1);
                            RCActionHelper helper = new RCActionHelper(1, 4, returnHelper(str2));
                            list.Add(helper);
                            sentType = 4;
                        }
                        else if (str2.StartsWith("VariableTitan"))
                        {
                            str2 = str2.Substring(index + 1, num5 - index - 1);
                            RCActionHelper helper = new RCActionHelper(1, 5, returnHelper(str2));
                            list.Add(helper);
                            sentType = 5;
                        }
                    }
                    else if (str2.StartsWith("Region"))
                    {
                        int index = str2.IndexOf('(');
                        int num5 = str2.LastIndexOf(')');
                        if (str2.StartsWith("RegionRandomX"))
                        {
                            str2 = str2.Substring(index + 1, num5 - index - 1);
                            RCActionHelper helper = new RCActionHelper(4, 0, returnHelper(str2));
                            list.Add(helper);
                            sentType = 3;
                        }
                        else if (str2.StartsWith("RegionRandomY"))
                        {
                            str2 = str2.Substring(index + 1, num5 - index - 1);
                            RCActionHelper helper = new RCActionHelper(4, 1, returnHelper(str2));
                            list.Add(helper);
                            sentType = 3;
                        }
                        else if (str2.StartsWith("RegionRandomZ"))
                        {
                            str2 = str2.Substring(index + 1, num5 - index - 1);
                            RCActionHelper helper = new RCActionHelper(4, 2, returnHelper(str2));
                            list.Add(helper);
                            sentType = 3;
                        }
                    }
                }
                else if (list.Count > 0)
                {
                    string str2 = strArray[num2];
                    if (list[list.Count - 1].helperClass == 1)
                    {
                        switch (list[list.Count - 1].helperType)
                        {
                            case 4:
                                if (str2.StartsWith("GetTeam()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 1, null);
                                    list.Add(helper);
                                    sentType = 0;
                                }
                                else if (str2.StartsWith("GetType()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 0, null);
                                    list.Add(helper);
                                    sentType = 0;
                                }
                                else if (str2.StartsWith("GetIsAlive()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 2, null);
                                    list.Add(helper);
                                    sentType = 1;
                                }
                                else if (str2.StartsWith("GetTitan()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 3, null);
                                    list.Add(helper);
                                    sentType = 0;
                                }
                                else if (str2.StartsWith("GetKills()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 4, null);
                                    list.Add(helper);
                                    sentType = 0;
                                }
                                else if (str2.StartsWith("GetDeaths()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 5, null);
                                    list.Add(helper);
                                    sentType = 0;
                                }
                                else if (str2.StartsWith("GetMaxDmg()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 6, null);
                                    list.Add(helper);
                                    sentType = 0;
                                }
                                else if (str2.StartsWith("GetTotalDmg()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 7, null);
                                    list.Add(helper);
                                    sentType = 0;
                                }
                                else if (str2.StartsWith("GetCustomInt()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 8, null);
                                    list.Add(helper);
                                    sentType = 0;
                                }
                                else if (str2.StartsWith("GetCustomBool()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 9, null);
                                    list.Add(helper);
                                    sentType = 1;
                                }
                                else if (str2.StartsWith("GetCustomString()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 10, null);
                                    list.Add(helper);
                                    sentType = 2;
                                }
                                else if (str2.StartsWith("GetCustomFloat()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 11, null);
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                else if (str2.StartsWith("GetPositionX()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 14, null);
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                else if (str2.StartsWith("GetPositionY()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 15, null);
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                else if (str2.StartsWith("GetPositionZ()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 16, null);
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                else if (str2.StartsWith("GetName()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 12, null);
                                    list.Add(helper);
                                    sentType = 2;
                                }
                                else if (str2.StartsWith("GetGuildName()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 13, null);
                                    list.Add(helper);
                                    sentType = 2;
                                }
                                else if (str2.StartsWith("GetSpeed()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(2, 17, null);
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                break;

                            case 5:
                                if (str2.StartsWith("GetType()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(3, 0, null);
                                    list.Add(helper);
                                    sentType = 0;
                                }
                                else if (str2.StartsWith("GetSize()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(3, 1, null);
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                else if (str2.StartsWith("GetHealth()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(3, 2, null);
                                    list.Add(helper);
                                    sentType = 0;
                                }
                                else if (str2.StartsWith("GetPositionX()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(3, 3, null);
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                else if (str2.StartsWith("GetPositionY()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(3, 4, null);
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                else if (str2.StartsWith("GetPositionZ()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(3, 5, null);
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                break;

                            default:
                                if (str2.StartsWith("ConvertToInt()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(5, sentType, null);
                                    list.Add(helper);
                                    sentType = 0;
                                }
                                else if (str2.StartsWith("ConvertToBool()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(5, sentType, null);
                                    list.Add(helper);
                                    sentType = 1;
                                }
                                else if (str2.StartsWith("ConvertToString()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(5, sentType, null);
                                    list.Add(helper);
                                    sentType = 2;
                                }
                                else if (str2.StartsWith("ConvertToFloat()"))
                                {
                                    RCActionHelper helper = new RCActionHelper(5, sentType, null);
                                    list.Add(helper);
                                    sentType = 3;
                                }
                                break;
                        }
                    }
                    else if (str2.StartsWith("ConvertToInt()"))
                    {
                        RCActionHelper helper = new RCActionHelper(5, sentType, null);
                        list.Add(helper);
                        sentType = 0;
                    }
                    else if (str2.StartsWith("ConvertToBool()"))
                    {
                        RCActionHelper helper = new RCActionHelper(5, sentType, null);
                        list.Add(helper);
                        sentType = 1;
                    }
                    else if (str2.StartsWith("ConvertToString()"))
                    {
                        RCActionHelper helper = new RCActionHelper(5, sentType, null);
                        list.Add(helper);
                        sentType = 2;
                    }
                    else if (str2.StartsWith("ConvertToFloat()"))
                    {
                        RCActionHelper helper = new RCActionHelper(5, sentType, null);
                        list.Add(helper);
                        sentType = 3;
                    }
                }
            }
            for (int num2 = list.Count - 1; num2 > 0; num2--)
            {
                list[num2 - 1].setNextHelper(list[num2]);
            }
            return list[0];
        }

        public static void RPC(string[] content)
        {
            if (content.Length == 1 && content[0] == "loadcached")
            {
                FengGameManagerMKII.FGM.StartCoroutine(LoadLevelCache());
                customLevelLoaded = true;
                return;
            }
            if (content.Length == 1 && content[0] == "loadempty")
            {
                currentLevel = string.Empty;
                levelCache.Clear();
                spawnPositions["Titan"].Clear();
                spawnPositions["PlayerC"].Clear();
                spawnPositions["PlayerM"].Clear();
                ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable();
                propertiesToSet.Add("currentLevel", currentLevel);
                PhotonNetwork.player.SetCustomProperties(propertiesToSet);
                customLevelLoaded = true;
                SpawnPlayerCustomMap();
                return;
            }
            customLevelClient(content, true);
        }

        public static IEnumerator SendRPCToPlayer(PhotonPlayer player)
        {
            if (currentLevel != string.Empty)
            {
                WaitForSeconds awaiter = new WaitForSeconds(0.25f);
                for (int i = 0; i < levelCache.Count; i++)
                {
                    if (player.Properties["currentLevel"] != null && currentLevel != string.Empty && player.CurrentLevel == currentLevel)
                    {
                        if (i == 0)
                        {
                            FengGameManagerMKII.FGM.BasePV.RPC("customlevelRPC", player, new object[] { new string[] { "loadcached" } });
                        }
                    }
                    else
                    {
                        FengGameManagerMKII.FGM.BasePV.RPC("customlevelRPC", player, new object[] { levelCache[i] });
                    }
                    yield return awaiter;
                }
            }
            else
            {
                FengGameManagerMKII.FGM.BasePV.RPC("customlevelRPC", player, new object[] { new string[] { "loadempty" } });
                customLevelLoaded = true;
            }
            yield break;
        }

        public static void SpawnPlayerCustomMap()
        {
            if (!FengGameManagerMKII.FGM.needChooseSide && IN_GAME_MAIN_CAMERA.MainCamera.gameOver)
            {
                if (PhotonNetwork.player.Dead)
                {
                    if (!PhotonNetwork.player.IsTitan)
                    {
                        FengGameManagerMKII.FGM.SpawnPlayer(FengGameManagerMKII.FGM.myLastHero);
                    }
                    else
                    {
                        FengGameManagerMKII.FGM.SpawnNonAiTitan("RANDOM");
                    }
                }
                FengGameManagerMKII.FGM.ShowHUDInfoCenter(string.Empty);
            }
        }

        private static void unloadAssets()
        {
            if (unloading)
            {
                return;
            }
            FengGameManagerMKII.FGM.StartCoroutine(unloadAssetsE(10f));
        }

        public static IEnumerator unloadAssetsE(float time)
        {
            yield return new WaitForSeconds(time);
            Resources.UnloadUnusedAssets();
            unloading = false;
            yield break;
        }
    }
}