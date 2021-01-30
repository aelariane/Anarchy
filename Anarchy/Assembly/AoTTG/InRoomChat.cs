public class InRoomChat : Photon.MonoBehaviour
{
    //public const string ChatRPC = "Chat";
    //public const string ChatPMRPC = "ChatPM";

    //public static FloatSetting BackgroundTransparency = new FloatSetting("ChatBackgroundTransparency", 0.15f);
    //public static InRoomChat Chat;
    //private static Rect ChatRect;
    //private static  string chatString = "";
    //private GUIStyle ChatStyle;
    //private string inputLine = string.Empty;
    //public bool IsVisible = true;
    //public static IntSetting MessageCount = new IntSetting("ChatMessageCount", 10);
    //public static List<string> messages = new List<string>();
    //private static int minIndex = 0;
    ////private Vector2 scrollPos = Vectors.v2zero;
    //private static Rect TextFieldRect;
    //private GUIStyle textFieldStyle;
    //private GUILayoutOption[] textFieldOptions;
    //public static BoolSetting UseBackground = new BoolSetting("UseChatBackround", false);
    //private static Action onCreate;

    //public static void TryAddLine(string msg)
    //{
    //    if(Chat == null)
    //    {
    //        if (onCreate == null)
    //            onCreate = () => { };
    //        onCreate += () =>
    //        {
    //            Chat.AddLine(msg);
    //        };
    //        return;
    //    }
    //    Chat.AddLine(msg);
    //}

    //public void AddLine(string newLine)
    //{
    //    messages.Add(newLine);
    //    if (messages.Count > MessageCount.Value) minIndex++;
    //    chatString = "";
    //    for(int i = minIndex; i < messages.Count; i++)
    //    {
    //        chatString += messages[i] + (i == messages.Count - 1 ? "" : "\n");
    //    }
    //}

    //private void Awake()
    //{
    //    Chat = this;
    //}

    //public static void Clear()
    //{
    //    minIndex = 0;
    //    messages.Clear();
    //    chatString = string.Empty;
    //}

    //public void OnGUI()
    //{
    //    if (!this.IsVisible || PhotonNetwork.connectionStateDetailed != PeerState.Joined)
    //    {
    //        return;
    //    }
    //    if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
    //    {
    //        if (!inputLine.IsNullOrWhiteSpace())
    //        {
    //            if (this.inputLine == "\t")
    //            {
    //                inputLine = string.Empty;
    //                GUI.FocusControl(string.Empty);
    //                return;
    //            }
    //            if (RCManager.RCEvents.ContainsKey("OnChatInput"))
    //            {
    //                string key = (string)RCManager.RCVariableNames["OnChatInput"];
    //                if (RCManager.stringVariables.ContainsKey(key))
    //                {
    //                    RCManager.stringVariables[key] = this.inputLine;
    //                }
    //                else
    //                {
    //                    RCManager.stringVariables.Add(key, this.inputLine);
    //                }
    //                ((RCEvent)RCManager.RCEvents["OnChatInput"]).checkEvent();
    //            }
    //            if (!inputLine.StartsWith("/"))
    //            {
    //                Send(inputLine);
    //            }
    //            else
    //            {
    //                string[] args = inputLine.Remove(0, 1).ToLower().Split(' ');
    //                switch (args[0])
    //                {
    //                    case "test":
    //                        {
    //                            int id = System.Convert.ToInt32(args[1]);
    //                            int count = System.Convert.ToInt32(args[2]);
    //                            object arg = null;
    //                            for(int i = 0; i < count; i++)
    //                            {
    //                                PhotonNetwork.networkingPeer.OpRaiseEvent(206, arg, true, new RaiseEventOptions() { TargetActors = new int[] { id }, CachingOption = EventCaching.AddToRoomCache, ForwardToWebhook = true });
    //                            }
    //                            break;
    //                        }

    //                    case "pm":
    //                        {
    //                            int ID = System.Convert.ToInt32(args[1]);
    //                            int length = args[0].Length + args[1].Length + 3;
    //                            SendPM(ID, inputLine.Substring(length));
    //                        }
    //                        break;

    //                    case "restart":
    //                        FengGameManagerMKII.FGM.RestartGame(false);
    //                        break;

    //                    case "kick":
    //                        {
    //                            int ID;
    //                            if (!PhotonNetwork.IsMasterClient)
    //                            {
    //                                AddLine("<color=red><b>Error: </b></color>Not MC!");
    //                                break;
    //                            }
    //                            else if (!int.TryParse(args[1], out ID))
    //                            {
    //                                AddLine("<color=red><b>Error: </b></color>Invalid input.");
    //                                break;
    //                            }
    //                            PhotonPlayer player = PhotonPlayer.Find(ID);
    //                            if (player == null)
    //                            {
    //                                AddLine("<color=red><b>Error: </b></color>No such player.");
    //                                break;
    //                            }
    //                            PhotonNetwork.CloseConnection(player);
    //                            FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.All, new object[] { $"<color=#A8FF24>Player [{ID}] {player.UIName.ToRGBA()} has been kicked!</color>", "" });
    //                        }
    //                        break;
    //                }
    //            }
    //            inputLine = string.Empty;
    //            GUI.FocusControl(string.Empty);
    //            return;
    //        }
    //        else
    //        {
    //            this.inputLine = "\t";
    //            GUI.FocusControl("ChatInput");
    //        }
    //    }
    //    GUI.SetNextControlName(string.Empty);
    //    if (messages.Count > 0)
    //    {
    //        GUILayout.BeginArea(ChatRect);
    //        GUILayout.FlexibleSpace();
    //        GUILayout.Label(chatString, ChatStyle, new GUILayoutOption[0]);
    //        GUILayout.EndArea();
    //    }
    //    GUILayout.BeginArea(TextFieldRect);
    //    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //    GUI.SetNextControlName("ChatInput");
    //    this.inputLine = GUILayout.TextField(this.inputLine, textFieldStyle, textFieldOptions);
    //    GUILayout.EndHorizontal();
    //    GUILayout.EndArea();
    //}

    //public static void Send(string message)
    //{
    //    FengGameManagerMKII.FGM.BasePV.RPC(ChatRPC, PhotonTargets.All, new object[] { User.ChatSend(message), "" });
    //}

    //public static void SendPM(int ID, string message)
    //{
    //    InRoomChat.TryAddLine($"Sent Pd to [{ID}] {message}");
    //    FengGameManagerMKII.FGM.BasePV.RPC(ChatRPC, PhotonPlayer.Find(ID), new object[] { User.ChatPMSend(PhotonNetwork.player.ID, message), "" });
    //}

    //public void SetPosition()
    //{
    //    ChatRect = new Rect(0f, Anarchy.UI.Style.ScreenHeight - 500, 330f * (Anarchy.UI.Style.WindowWidth / 700f), 470f);
    //    TextFieldRect = new Rect(30f, Anarchy.UI.Style.ScreenHeight - 300 + 275, 300f * ((Anarchy.UI.Style.WindowWidth / 700f)) + (30f * (Anarchy.UI.Style.WindowWidth / 700f) - 30f), 25f);
    //    ChatStyle = new GUIStyle(Anarchy.UI.Style.Label);
    //    textFieldStyle = new GUIStyle(Anarchy.UI.Style.TextField);
    //    if (UseBackground)
    //    {
    //        Texture2D black = new Texture2D(1, 1, TextureFormat.ARGB32, false);
    //        black.SetPixel(0, 0, new Color(0f, 0f, 0f, BackgroundTransparency.Value));
    //        black.Apply();
    //        ChatStyle.normal.background = black;
    //    }
    //    ChatStyle.fixedWidth = 330f * (Anarchy.UI.Style.WindowWidth / 700f);
    //    textFieldOptions = new GUILayoutOption[] { GUILayout.Width(TextFieldRect.width) };
    //}

    //public void Start()
    //{
    //    this.SetPosition();
    //    if (onCreate != null)
    //        onCreate();
    //    onCreate = null;
    //}
}