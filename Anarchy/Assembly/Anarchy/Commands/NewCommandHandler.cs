using Anarchy;
using Anarchy.NameAnimation;
using Anarchy.Network;
using Aottg.Extensions.ChatCommands;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AoTTG.Anarchy.Commands
{
    public class NewCommandHandler : ChatCommandModule
    {
        private global::Anarchy.Localization.Locale Lang { get; set; }
        private global::Anarchy.Localization.Locale English { get; set; }
        private Lerp _lerp;

        public NewCommandHandler() : base()
        {
            Lang = new global::Anarchy.Localization.Locale("ChatCommands")
            {
                Formateable = true
            };
            Lang.Reload();
            Lang.FormatColors();

            English = new global::Anarchy.Localization.Locale(global::Anarchy.Localization.Language.DefaultLanguage, "ChatCommands", true, ',');
            English.Reload();
            English.FormatColors();

            _lerp = new Lerp(PhotonNetwork.player.UIName)
            {
                Colors = new List<Color> { Color.magenta, Color.blue, Color.cyan, Color.blue, Color.magenta },
                Time = 5f
            };
        }

        [ChatCommand("animate")]
        [ChatCommandDescription("Animates your name with 4 colors")]
        public void Animate(ChatCommandContext ctx)
        {
            if (!_lerp.Active)
            {
                _lerp.Active = true;
                FengGameManagerMKII.FGM.StartCoroutine(_lerp.Animate());
            }
            else
            {
                _lerp.Active = false;
                FengGameManagerMKII.FGM.StopCoroutine(_lerp.Animate());
            }
        }

        [ChatCommand("aso")]
        public void ASO(ChatCommandContext ctx)
        {
            if (FengGameManagerMKII.FGM.logic.Mode != GameMode.Racing)
            {
                ctx.SendLocalMessage(Lang["notRacingMode"]);
                return;
            }
            GameModes.AsoRacing.State = !GameModes.AsoRacing.State;
            ctx.SendLocalMessage(Lang["asorace" + GameModes.AsoRacing.State.ToString()]);
            Extras.SendLocalizedText("asorace" + GameModes.AsoRacing.State.ToString(), null);
        }

        [ChatCommand("team")]
        public void SwitchTeam(ChatCommandContext ctx)
        {
            if (!GameModes.TeamMode.Enabled || GameModes.TeamMode.Selection > 1)
            {
                ctx.SendLocalMessage(Lang["errTeamsLocked"].HtmlColor("FF0000"));
                return;
            }
            if (ctx.Arguments.Length <= 0)
            {
                return;
            }
            int team = 0;
            switch (ctx.Arguments[0].ToLower())
            {
                case "0":
                case "individual":
                    break;

                case "1":
                case "cyan":
                    team = 1;
                    break;

                case "2":
                case "magenta":
                    team = 2;
                    break;

                default:
                    ctx.SendLocalMessage(Lang["errTeamsInvalid"].HtmlColor("FF0000"));
                    return;
            }
            FengGameManagerMKII.FGM.BasePV.RPC("setTeamRPC", PhotonNetwork.player, new object[] { team });
            ctx.SendLocalMessage(Lang.Format("teamChanged", GetTeamName(team)));
            if (PhotonNetwork.player.GameObject != null && PhotonNetwork.player.GameObject.GetComponent<HERO>() != null)
            {
                PhotonNetwork.player.GameObject.GetPhotonView().RPC("netDie2", PhotonTargets.All, new object[] { -1, "Team switch" });
            }
            return;
        }

        private string GetTeamName(int team)
        {
            switch (team)
            {
                default:
                case 0:
                    return "<color=lime>" + Lang["teamIndividuals"] + "</color>";

                case 1:
                    return "<color=cyan>" + Lang["teamCyan"] + "</color>";

                case 2:
                    return "<color=magenta>" + Lang["teamMagenta"] + "</color>";
            }
        }

        [ChatCommand("check")]
        public void CheckAnarchyUser(ChatCommandContext ctx)
        {
            string chatMessage = "";
            if (ctx.Arguments.Length < 1)
            {
                chatMessage = Lang["errArg"];
                return;
            }

            int anarchyInt = 0;
            int abuseInt = 0;

            if (ctx.Arguments[0] != "desc")
            {
                if (int.TryParse(ctx.Arguments[0], out int playerId) == false)
                {
                    chatMessage = Lang["errArg"];
                    return;
                }

                PhotonPlayer player = PhotonPlayer.Find(playerId);

                if (player.Properties.ContainsKey(PhotonPlayerProperty.anarchyFlags) == false && player.Properties.ContainsKey(PhotonPlayerProperty.anarchyAbuseFlags) == false)
                {
                    chatMessage = Lang["noAnarchyFeaturesFound"];
                    return;
                }
                anarchyInt = (int)player.Properties[PhotonPlayerProperty.anarchyFlags];
                abuseInt = (int)player.Properties[PhotonPlayerProperty.anarchyAbuseFlags];

                if (anarchyInt == 0 && abuseInt == 0)
                {
                    chatMessage = Lang["noAnarchyFeaturesFound"];
                    return;
                }
            }
            else
            {
                if (ctx.Arguments[1] == "abuse")
                {
                    abuseInt = int.Parse(ctx.Arguments[2]);
                }
                else
                {
                    anarchyInt = int.Parse(ctx.Arguments[1]);
                }
            }

            if (anarchyInt > 0)
            {
                List<string> anarchyFeatures = new List<string>();
                if ((anarchyInt & (int)AnarchyFlags.DisableBodyLean) == (int)AnarchyFlags.DisableBodyLean)
                {
                    anarchyFeatures.Add(Lang["bodyLeanAbuse"]);
                }
                if ((anarchyInt & (int)AnarchyFlags.LegacyBurst) == (int)AnarchyFlags.LegacyBurst)
                {
                    anarchyFeatures.Add(Lang["legacyBurstAbuse"]);
                }
                if ((anarchyInt & (int)AnarchyFlags.NewTPSCamera) == (int)AnarchyFlags.NewTPSCamera)
                {
                    anarchyFeatures.Add(Lang["newTpsAbuse"]);
                }
                if ((anarchyInt & (int)AnarchyFlags.DisableBurstCooldown) == (int)AnarchyFlags.DisableBurstCooldown)
                {
                    anarchyFeatures.Add(Lang["disableBurstCD"]);
                }
                chatMessage += Lang.Format("anarchyFeatures") + " " + string.Join(", ", anarchyFeatures.ToArray());
            }

            if (abuseInt > 0)
            {
                List<string> anarchyAbusiveFeatures = new List<string>();
                if ((abuseInt & (int)AbuseFlags.InfiniteGasInPvp) == (int)AbuseFlags.InfiniteGasInPvp)
                {
                    anarchyAbusiveFeatures.Add(Lang["infGasInPvp"]);
                }
                if (chatMessage.Length > 0)
                {
                    chatMessage += "\n";
                }
                chatMessage += Lang.Format("anarchyAbusiveFeatures") + " " + string.Join(", ", anarchyAbusiveFeatures.ToArray());
            }
            ctx.SendLocalMessage(chatMessage);
            return;
        }

        [ChatCommand("clear")]
        public void Clear(ChatCommandContext ctx)
        {
            if (ctx.Arguments.Length == 0)
            {
                global::Anarchy.UI.Chat.Clear();
            }
            else if (ctx.Arguments[0] == "-c")
            {
                global::Anarchy.UI.Log.Clear();
            }
        }

        [ChatCommand("gas")]
        public void ChangeGasAnim(ChatCommandContext ctx)
        {
            if (ctx.Arguments.Length <= 0)
            {
                ctx.SendLocalMessage(Lang["errArg"]);
                return;
            }
            HERO myHero = FengGameManagerMKII.Heroes.FirstOrDefault(x => x.IsLocal);
            if (myHero == null)
            {
                ctx.SendLocalMessage("Spawn your HERO first!");
                return;
            }
            switch (ctx.Arguments[0].ToLower())
            {
                case "help":
                    ctx.SendLocalMessage("Available options:\ngas, cross(2), meat(2), blood");
                    break;

                case "default":
                case "gas":
                    myHero.GasBurstAnimation = "FX/boost_smoke";
                    break;

                case "cross":
                    myHero.GasBurstAnimation = "redCross";
                    break;

                case "cross2":
                    myHero.GasBurstAnimation = "redCross1";
                    break;

                case "meat":
                    myHero.GasBurstAnimation = "hitMeat";
                    break;

                case "meat2":
                    myHero.GasBurstAnimation = "hitMeat2";
                    break;
                //  Will probably be annoying / abusive for weak PC user
                //case "splatter":
                //    break;
                case "blood":
                    myHero.GasBurstAnimation = "bloodExplore";
                    break;
                //abusive too
                //case "thunder":
                //    myHero.GasBurstAnimation = "FX/Thunder";
                //    break;
                default:
                    ctx.SendLocalMessage(Lang["errArg"]);
                    return;
            }
        }

        [ChatCommand("kick"/*, new string[] { "skick", "sban", "ban" }*/)]
        [ChatCommand("skick")]
        [ChatCommand("sban")]
        [ChatCommand("ban")]
        [MasterClientOnly]
        public void KickandBans(ChatCommandContext ctx)
        {
            bool super = false;
            bool ban = false;
            string chatMessage = "";
            switch (ctx.CommandName.ToLower())
            {
                case "kick":
                    super = false;
                    ban = false;
                    break;

                case "ban":
                    ban = true;
                    super = false;
                    break;

                case "skick":
                    ban = false;
                    super = true;
                    break;

                case "sban":
                    ban = true;
                    super = true;
                    break;
            }
            if (ctx.Arguments.Length <= 0)
            {
                ctx.SendLocalMessage(Lang.Format("errArg", ctx.CommandName));
                return;
            }
            int[] IDs = new int[ctx.Arguments.Length];
            bool permaBan = false;
            for (int i = 0; i < ctx.Arguments.Length; i++)
            {
                if (!int.TryParse(ctx.Arguments[i], out IDs[i]))
                {
                    if (ctx.Arguments[i] == "perma" || ctx.Arguments[i] == "--perma" || ctx.Arguments[i] == "-p")
                    {
                        permaBan = true;
                        IDs[i] = -128;
                    }
                }
            }
            string send = "";
            for (int i = 0; i < IDs.Length; i++)
            {
                PhotonPlayer target = PhotonPlayer.Find(IDs[i]);
                if (target == null)
                {
                    if (IDs[i] != -128)
                    {
                        if (chatMessage.Length > 0)
                        {
                            chatMessage += "\n";
                        }

                        chatMessage += Lang.Format("kickIgnore", IDs[i].ToString());
                    }
                    continue;
                }
                if (chatMessage.Length > 0)
                {
                    chatMessage += "\n";
                }

                if (send.Length > 0)
                {
                    send += "\n";
                }

                chatMessage += string.Format(Lang["kickSuccess"], new object[] { target.ID.ToString(), target.UIName.ToHTMLFormat() });
                // ctx.SendLocalMessage("kickSuccess" +  target.ID.ToString() + target.UIName.ToHTMLFormat() );
                if (ban)
                {
                    if (permaBan)
                    {
                        BanList.PermaBan(target);
                    }
                    else
                    {
                        BanList.Ban(target);
                    }
                }
                if (super)
                {
                    global::Anarchy.Network.Antis.SuperKick(target, ban, string.Empty);
                }
                else
                {
                    global::Anarchy.Network.Antis.Kick(target, ban, string.Empty);
                }
                ctx.SendLocalMessage(chatMessage);
            }
            return;
        }

        [ChatCommand("kill")]
        [MasterClientOnly]
        public void KillCMD(ChatCommandContext ctx)
        {
            if (ctx.Arguments.Length <= 0)
            {
                ctx.SendLocalMessage(Lang["errArg"]);
                return;
            }
            int ID;
            if (!int.TryParse(ctx.Arguments[0], out ID))
            {
                ctx.SendLocalMessage(Lang["errArg"]);
                return;
            }
            PhotonPlayer target = PhotonPlayer.Find(ID);
            if (target == null)
            {
                ctx.SendLocalMessage(Lang["errArg"]);
                return;
            }
            string killer = ctx.Arguments.Length > 1 ? ctx.Arguments[1] : "Kill";
            Abuse.Kill(target, killer);
        }

        [ChatCommand("mute")]
        [ChatCommand("unmute")]
        public void MuteCMD(ChatCommandContext ctx)
        {
            if (ctx.Arguments.Length == 0)
            {
                ctx.SendLocalMessage(Lang["errArg"]);
                return;
            }
            string key = "";
            bool value = true;
            switch (ctx.CommandName.ToLower())
            {
                case "mute":
                    key = "mute";
                    value = true;
                    break;

                case "unmute":
                    key = "unmute";
                    value = false;
                    break;
            }
            for (int i = 0; i < ctx.Arguments.Length; i++)
            {
                if (int.TryParse(ctx.Arguments[i], out int ID))
                {
                    PhotonPlayer target = PhotonPlayer.Find(ID);
                    if (target == null)
                    {
                        ctx.SendLocalMessage(Lang.Format("errPlayer", ID.ToString()));
                        continue;
                    }
                    ctx.SendLocalMessage(Lang.Format(key + (target.Muted == value ? "Failed" : "Succeed"), ID.ToString(), target.UIName.ToHTMLFormat()));
                    target.Muted = value;
                    FengGameManagerMKII.FGM.PlayerList.Update();
                }
            }
        }

        [ChatCommand("pause")]
        [ChatCommand("unpause")]
        [MasterClientOnly]
        public void Pause(ChatCommandContext ctx)
        {
            bool value = true;
            switch (ctx.CommandName.ToLower())
            {
                case "pause":
                    value = true;
                    break;

                case "unpause":
                    value = false;
                    break;
            }
            if (value && AnarchyManager.PauseWindow.IsActive)
            {
                ctx.SendLocalMessage(Lang["pauseErr"]);
                return;
            }
            else if (!value && !AnarchyManager.PauseWindow.IsActive)
            {
                ctx.SendLocalMessage(Lang["unpauseErr"]);
                return;
            }
            ctx.SendLocalMessage(Lang["pause" + value.ToString()]);
            FengGameManagerMKII.FGM.BasePV.RPC("pauseRPC", PhotonTargets.All, new object[] { value });
            Extras.SendLocalizedText("pause" + value.ToString(), null);
        }

        [ChatCommand("pm")]
        public void PM(ChatCommandContext ctx)
        {
            int pmID = -1;
            string chatMessage;
            if (ctx.Arguments.Length <= 0)
            {
                ctx.SendLocalMessage(Lang.Format("errArg", ctx.CommandName));
                return;
            }
            if (ctx.Arguments[0].ToLower() == "setid")
            {
                if (!int.TryParse(ctx.Arguments[1], out int id))
                {
                    ctx.SendLocalMessage(Lang.Format("errArg", ctx.CommandName));
                    return;
                }
                pmID = id;
                ctx.SendLocalMessage(Lang.Format("pmSetID", pmID.ToString()));
                return;
            }
            else if (!int.TryParse(ctx.Arguments[0], out int ID))
            {
                if (pmID <= 0)
                {
                    ctx.SendLocalMessage(Lang.Format("errArg", ctx.CommandName));
                    return;
                }
                else if (PhotonPlayer.Find(pmID) == null)
                {
                    ctx.SendLocalMessage(Lang.Format("errPM", pmID.ToString()));
                    return;
                }
                string message = string.Join(" ", ctx.Arguments, 0, ctx.Arguments.Length);
                chatMessage = string.Format(Lang["pmSent"], new object[] { pmID.ToString(), message });
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonPlayer.Find(pmID), new object[] { User.ChatPmSend(PhotonNetwork.player.ID, message), "" });
            }
            else
            {
                if (PhotonPlayer.Find(ID) == null)
                {
                    ctx.SendLocalMessage(Lang.Format("errPM", pmID.ToString()));
                    return;
                }
                string message = string.Join(" ", ctx.Arguments, 1, ctx.Arguments.Length - 1);
                chatMessage = string.Format(Lang["pmSent"], new object[] { ID.ToString(), message });
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonPlayer.Find(ID), new object[] { User.ChatPmSend(PhotonNetwork.player.ID, message), "" });
            }
            ctx.SendLocalMessage(chatMessage);
        }

        [ChatCommand("resetkd")]
        public void ResetKD(ChatCommandContext ctx)
        {
            if (ctx.Arguments.Length <= 0)
            {
                ResetProps(PhotonNetwork.player);
                ctx.SendLocalMessage(Lang["resetkdLocal"]);
                return;
            }
            if (!PhotonNetwork.IsMasterClient)
            {
                ctx.SendLocalMessage(Lang["errMC"]);
                return;
            }
            if (ctx.Arguments[0] == "all")
            {
                foreach (PhotonPlayer player in PhotonNetwork.playerList)
                {
                    ResetProps(player);
                }
                ctx.SendLocalMessage(Lang["resetkdAll"]);
                //if (PhotonNetwork.IsMasterClient)
                //{
                //    SendLocalizedText("resetkdAll", null);
                //}
                return;
            }
            int[] IDs = new int[ctx.Arguments.Length];
            for (int i = 0; i < ctx.Arguments.Length; i++)
            {
                int.TryParse(ctx.Arguments[i], out IDs[i]);
            }
            for (int i = 0; i < IDs.Length; i++)
            {
                PhotonPlayer player = PhotonPlayer.Find(IDs[i]);
                if (player != null)
                {
                    ResetProps(player);
                    ctx.SendLocalMessage(Lang.Format("resetkdPlayer", player.ID.ToString(), player.UIName.ToHTMLFormat()));
                }
            }
        }

        private readonly Hashtable _ResetHash = new Hashtable() { { PhotonPlayerProperty.kills, 0 }, { PhotonPlayerProperty.max_dmg, 0 }, { PhotonPlayerProperty.total_dmg, 0 }, { PhotonPlayerProperty.deaths, 0 } };

        private void ResetProps(PhotonPlayer target)
        {
            if (target != null)
            {
                target.SetCustomProperties(_ResetHash);
            }
        }

        [ChatCommand("restart")]
        [MasterClientOnly]
        public void Restart(ChatCommandContext ctx)
        {
            if (ctx.Arguments.Length > 0 && ctx.Arguments[0] == "-r")
            {
                foreach (PhotonPlayer player in PhotonNetwork.playerList)
                {
                    ResetProps(player);
                }
            }
            FengGameManagerMKII.FGM.RestartGame(false, true);
            ctx.SendLocalMessage(Lang["restartMessage"]);
            Extras.SendLocalizedText("restartMessage", null);
        }

        [ChatCommand("revive")]
        [ChatCommand("reviveall")]
        [MasterClientOnly]
        public void Revive(ChatCommandContext ctx)
        {
            Hashtable HashRevive = new Hashtable() { [(byte)0] = 2, [(byte)2] = PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds, [(byte)3] = "respawnHeroInNewRound" };
            if (ctx.Arguments.Length <= 0)
            {
                ctx.SendLocalMessage(Lang["revived"]);
                PhotonNetwork.networkingPeer.OpRaiseEvent(200, HashRevive, true, new RaiseEventOptions { TargetActors = new int[] { PhotonNetwork.player.ID } });
                return;
            }
            if (ctx.CommandName.ToLower() == "reviveall" || ctx.Arguments[0].ToLower() == "all")
            {
                FengGameManagerMKII.FGM.BasePV.RPC("respawnHeroInNewRound", PhotonTargets.All, new object[0]);
                Extras.SendLocalizedText("revivedAll", null);
                ctx.SendLocalMessage(Lang["revivedAll"]);
                return;
            }
            HashRevive[(byte)2] = PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds;
            int[] IDs = new int[ctx.Arguments.Length];
            for (int i = 0; i < ctx.Arguments.Length; i++)
            {
                int.TryParse(ctx.Arguments[i], out IDs[i]);
            }
            for (int i = 0; i < IDs.Length; i++)
            {
                PhotonPlayer target = PhotonPlayer.Find(IDs[i]);
                if (target == null)
                {
                    continue;
                }
                PhotonNetwork.networkingPeer.OpRaiseEvent(200, HashRevive, true, new RaiseEventOptions { TargetActors = new int[] { target.ID } });

                Extras.SendLocalizedText(target, "revived", null);

                ctx.SendLocalMessage(Lang.Format("playerRevived", target.ID.ToString()));
            }
        }

        [ChatCommand("room")]
        [MasterClientOnly]
        public void RoomSettings(ChatCommandContext ctx)
        {
            if (ctx.Arguments.Length < 2)
            {
                ctx.SendLocalMessage(Lang.Format("errRoom", ctx.Arguments[0].ToLower()));
                return;
            }
            switch (ctx.Arguments[0].ToLower())
            {
                case "hide":
                    {
                        PhotonNetwork.room.Visible = false;
                    }
                    break;

                case "show":
                    {
                        PhotonNetwork.room.Visible = true;
                    }
                    break;

                case "close":
                    {
                        PhotonNetwork.room.Open = false;
                    }
                    break;

                case "open":
                    {
                        PhotonNetwork.room.Open = true;
                    }
                    break;

                case "max":
                    if (!int.TryParse(ctx.Arguments[1], out int max))
                    {
                        ctx.SendLocalMessage(Lang.Format("errArg", ctx.CommandName + " max"));
                        return;
                    }
                    PhotonNetwork.room.MaxPlayers = max;
                    ctx.SendLocalMessage(Lang.Format("roomMax", max.ToString()));
                    Extras.SendLocalizedText("roomMax", new string[] { max.ToString() });
                    break;

                case "time":
                    int time;
                    if (ctx.Arguments[1].ToLower() == "set")
                    {
                        if (ctx.Arguments.Length < 3)
                        {
                            ctx.SendLocalMessage(Lang.Format("errRoom", ctx.Arguments[0].ToLower()));
                            return;
                        }
                        if (!int.TryParse(ctx.Arguments[2], out time))
                        {
                            ctx.SendLocalMessage(Lang.Format("errArg", ctx.CommandName + " time"));
                            return;
                        }
                        FengGameManagerMKII.FGM.logic.ServerTime = time;
                        FengGameManagerMKII.FGM.logic.OnRequireStatus();
                        ctx.SendLocalMessage(Lang.Format("roomTimeSet", time.ToString()));
                        Extras.SendLocalizedText("roomTimeSet", new string[] { time.ToString() });
                        break;
                    }
                    if (!int.TryParse(ctx.Arguments[1], out time))
                    {
                        ctx.SendLocalMessage(Lang.Format("errArg", ctx.CommandName + " time"));
                        return;
                    }
                    FengGameManagerMKII.FGM.logic.ServerTime += time;
                    FengGameManagerMKII.FGM.logic.OnRequireStatus();
                    ctx.SendLocalMessage(Lang.Format("roomTime", time.ToString()));
                    Extras.SendLocalizedText("roomTime", new string[] { time.ToString() });
                    break;

                default:
                    ctx.SendLocalMessage(Lang.Format("errRoom", ctx.Arguments[0].ToLower()));
                    return;
            }
        }

        [ChatCommand("rules")]
        public void Rules(ChatCommandContext ctx)
        {
            string toAdd = GameModes.GetGameModesInfo();
            if (toAdd.Length > 0)
            {
                ctx.SendLocalMessage(Lang["activatedGameModes"] + "\n" + toAdd);
            }
        }

        [ChatCommand("unban")]
        public void Unban(ChatCommandContext ctx)
        {
            if (ctx.Arguments.Length < 1)
            {
                ctx.SendLocalMessage(Lang["errArg"]);
                return;
            }
            bool unban;
            if (int.TryParse(ctx.Arguments[0], out int ID))
            {
                unban = BanList.Unban(ID);
            }
            else
            {
                unban = BanList.Unban(ctx.Arguments[0]);
            }
            ctx.SendLocalMessage(Lang["unban" + (unban ? "Succeed" : "Failed")]);
        }

        #region Spectate
        //[ChatCommand("spectate")]
        //public void Spectate(ChatCommandContext ctx)
        //{
        //    if (ctx.Arguments.Length > 0)
        //    {
        //        if (int.TryParse(ctx.Arguments[0], out int id))
        //        {
        //            var player = PhotonPlayer.Find(id);
        //            if (player == null || player.Dead)
        //            {
        //                return;
        //            }
        //            var hero = player.GameObject?.GetComponent<HERO>();
        //            if (hero == null)
        //            {
        //                return;
        //            }

        //            IN_GAME_MAIN_CAMERA.MainCamera.SetMainObject(hero, true, false);
        //            IN_GAME_MAIN_CAMERA.MainCamera.setSpectorMode(false);
        //            return;
        //        }
        //        return;
        //    }
        //    IsInSpecMode = !IsInSpecMode;
        //    EnterSpecMode(IsInSpecMode);
        //    ctx.SendLocalMessage(Lang["specMode" + (IsInSpecMode ? "Enter" : "Quit")]);
        //}
        //internal static bool IsInSpecMode = false;
        //public void EnterSpecMode(bool enter)
        //{
        //    List<GameObject> spectateSprites = new List<GameObject>();
        //    if (enter)
        //    {
        //        UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
        //        for (int i = 0; i < array.Length; i++)
        //        {
        //            GameObject gameObject = (GameObject)array[i];
        //            if (!(gameObject.GetComponent<UISprite>() != null) || !gameObject.activeInHierarchy)
        //            {
        //                continue;
        //            }
        //            string text = gameObject.name;
        //            if (text.Contains("blade") || text.Contains("bullet") || text.Contains("gas") || text.Contains("flare") || text.Contains("skill_cd"))
        //            {
        //                if (!spectateSprites.Contains(gameObject))
        //                {
        //                    spectateSprites.Add(gameObject);
        //                }
        //                gameObject.SetActive(value: false);
        //            }
        //        }
        //        string[] array2 = new string[2]
        //        {
        //        "Flare",
        //        "LabelInfoBottomRight"
        //        };
        //        string[] array3 = array2;
        //        foreach (string text2 in array3)
        //        {
        //            GameObject gameObject2 = GameObject.Find(text2);
        //            if (gameObject2 != null)
        //            {
        //                if (!spectateSprites.Contains(gameObject2))
        //                {
        //                    spectateSprites.Add(gameObject2);
        //                }
        //                gameObject2.SetActive(value: false);
        //            }
        //        }
        //        foreach (HERO player in FengGameManagerMKII.Heroes)
        //        {
        //            if (player.BasePV.IsMine)
        //            {
        //                PhotonNetwork.Destroy(player.BasePV);
        //            }
        //        }
        //        if (PhotonNetwork.player.IsTitan && !PhotonNetwork.player.Dead)
        //        {
        //            foreach (TITAN titan in FengGameManagerMKII.Titans)
        //            {
        //                if (titan.BasePV.IsMine)
        //                {
        //                    PhotonNetwork.Destroy(titan.BasePV);
        //                }
        //            }
        //        }
        //        NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[1], state: false);
        //        NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[2], state: false);
        //        NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[3], state: false);
        //        FengGameManagerMKII.FGM.needChooseSide = false;
        //        Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().enabled = true;
        //        if (IN_GAME_MAIN_CAMERA.CameraMode == CameraType.ORIGINAL)
        //        {
        //            Screen.lockCursor = false;
        //            Screen.showCursor = false;
        //        }
        //        GameObject gameObject3 = GameObject.FindGameObjectWithTag("Player");
        //        if (gameObject3 != null && gameObject3.GetComponent<HERO>() != null)
        //        {
        //            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetMainObject(gameObject3.GetComponent<HERO>());
        //        }
        //        else
        //        {
        //            Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetMainObject(null);
        //        }
        //        Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(val: false);
        //        Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
        //    }
        //    else
        //    {
        //        if (GameObject.Find("cross1") != null)
        //        {
        //            GameObject.Find("cross1").transform.localPosition = Vector3.up * 5000f;
        //        }
        //        if (spectateSprites != null)
        //        {
        //            foreach (GameObject spectateSprite in spectateSprites)
        //            {
        //                if (spectateSprite != null)
        //                {
        //                    spectateSprite.SetActive(value: true);
        //                }
        //            }
        //        }
        //        spectateSprites = new List<GameObject>();
        //        NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[1], state: false);
        //        NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[2], state: false);
        //        NGUITools.SetActive(FengGameManagerMKII.UIRefer.panels[3], state: false);
        //        FengGameManagerMKII.FGM.needChooseSide = true;
        //        Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().SetMainObject(null);
        //        Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(val: true);
        //        Camera.main.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
        //    }
        //}
        #endregion
    }
}

public static class Extras
{
    public static void SendLocalMessage(this ChatCommandContext ctx, string Message)
    {
        Anarchy.UI.Chat.Add(Message);
    }

    public static void AddLogMessage(this ChatCommandContext ctx, string Message)
    {
        Anarchy.UI.Log.AddLine(Message);
    }

    public static void SendLocalizedText(string key, string[] Arguments)
    {
        Anarchy.UI.Chat.SendLocalizedText("ChatCommands", key, Arguments);
    }

    public static void SendLocalizedText(PhotonPlayer target, string key, string[] Arguments)
    {
        if (target != null)
        {
            Anarchy.UI.Chat.SendLocalizedText(target, "ChatCommands", key, Arguments);
        }
    }
}