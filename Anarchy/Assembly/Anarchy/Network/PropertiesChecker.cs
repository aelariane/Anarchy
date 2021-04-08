using ExitGames.Client.Photon;

namespace Anarchy.Network
{
    internal static class PropertiesChecker
    {
        //Here is placed detection of most public mods
        private static bool TryCheckMod(System.Collections.DictionaryEntry entry, out string modName)
        {
            modName = string.Empty;
            if (entry.Key is string keyString)
            {
                bool detected = true;
                switch (keyString)
                {
                    //Guardian mod by alerithe (Summer)
                    case "GuardianMod":
                        modName = ModNames.Guardian;
                        break;

                    //Photon mod by Fleur
                    case "guildName":
                        if (entry.Value is string gName)
                        {
                            if (gName.StartsWith("photonMod"))
                            {
                                modName = ModNames.Photon;
                                break;
                            }
                        }
                        detected = false;
                        break;

                    //PedoRC mod by Sadico
                    case "PBModRC":
                        modName = ModNames.PedoRC;
                        break;

                    //TLW mod by JustlPain
                    case "TLW":
                        modName = ModNames.TLW;
                        break;

                    //Exp mod by ???
                    case "ExpMod":
                    case "EMID":
                    case "Version":
                    case "Pref":
                        modName = ModNames.Exp;
                        break;

                    //Universe mod by ???
                    case "UPublica":
                    case "UPublica2":
                    case "UGrup":
                    case "Hats":
                    case "UYoutube":
                    case "UVip":
                    case "SUniverse":
                    case "UAdmin":
                    case "coins":
                        modName = ModNames.Universe;
                        break;

                    //Additional check for universe
                    case "":
                        if (entry.Value is string)
                        {
                            modName = ModNames.Universe;
                            break;
                        }
                        else if(entry.Value is int)
                        {
                            modName = ModNames.RRC;
                            break;
                        }
                        detected = false;
                        break;

                    //Ranked RC mod by ????
                    case "bronze":
                    case "silver":
                    case "gold":
                    case "platin":
                    case "diamond":
                    case "master":
                    case "grandmaster":
                    case "top5":
                        modName = string.Format(ModNames.RRC, keyString);
                        break;

                    case "RCteam":
                        if (entry.Value is string teamSting)
                        {
                            switch (teamSting)
                            {
                                //GucciGang mod by JustlPain
                                case "GGM":
                                case "GGM83":
                                case "GucciLab":
                                    modName = ModNames.GucciGang;
                                    break;

                                //Another DeadInside mod check
                                case "Dead Inside":
                                    modName = ModNames.DeadInside;
                                    break;

                                default:
                                    break;
                            }
                        }
                        detected = modName != string.Empty;
                        break;

                    case "Destroy":
                        modName = ModNames.Destroy;
                        break;

                    case "BRM":
                        modName = ModNames.Brm;
                        break;

                    case "DeadInside":
                        modName = ModNames.DeadInside;
                        break;

                    case "INSANE":
                    case "INS":
                    case "INSANE new mod":
                        modName = ModNames.Insane;
                        break;

                    default:
                        detected = false;
                        break;
                }
                return detected;
            }
            return false;
        }

        public static void CheckPlayersProperties(Hashtable hash, int targetActorNr, PhotonPlayer sender)
        {
            NetworkingPeer peer = PhotonNetwork.networkingPeer;
            if (peer == null || hash == null || hash.Count <= 0)
            {
                return;
            }
            if (targetActorNr > 0)
            {
                PhotonPlayer target = PhotonPlayer.Find(targetActorNr);
                if (target != null)
                {
                    Hashtable props = peer.GetActorPropertiesForActorNr(hash, targetActorNr);
                    if (target.IsLocal)
                    {
                        bool needSend = false;
                        if (sender == null || !sender.IsMasterClient)
                        {
                            if (props.ContainsKey(PhotonPlayerProperty.name))
                            {
                                string checkName = props[PhotonPlayerProperty.name] as string;
                                if (checkName == null || checkName != User.Name.Value)
                                {
                                    //TODO: Localize
                                    UI.Log.AddLineRaw($"{(sender == null ? "Someone" : $"[{sender.ID}]" + sender.UIName.ToHTMLFormat())} tried to change your name.");
                                    props[PhotonPlayerProperty.name] = User.Name.Value;
                                    needSend = true;
                                }
                            }
                            if (props.ContainsKey(PhotonPlayerProperty.guildName))
                            {
                                string checkName = props[PhotonPlayerProperty.guildName] as string;
                                if (checkName == null || checkName != User.AllGuildNames)
                                {
                                    //TODO: Localize
                                    UI.Log.AddLineRaw($"{(sender == null ? "Someone" : $"[{sender.ID}]" + sender.UIName.ToHTMLFormat())} tried to change your guildname.");
                                    props[PhotonPlayerProperty.guildName] = User.AllGuildNames;
                                    needSend = true;
                                }
                            }
                        }
                        if (needSend)
                        {
                            target.SetCustomProperties(props);
                        }
                    }
                    else
                    {
                        if (!target.ModLocked)
                        {
                            foreach (var entry in props)
                            {
                                if (TryCheckMod(entry, out string modName))
                                {
                                    target.ModName = modName;
                                    target.ModLocked = true;
                                    break;
                                }
                            }
                        }
                    }
                    target.InternalCacheProperties(props);
                    NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[] { target, props });
                }
            }
            else
            {
                foreach (object obj2 in hash.Keys)
                {
                    int number = (int)obj2;
                    Hashtable properties = (Hashtable)hash[obj2];
                    string name = (string)properties[(byte)0xff];
                    PhotonPlayer player = PhotonPlayer.Find(number);
                    if (player == null)
                    {
                        player = new PhotonPlayer(false, number, name);
                        peer.AddNewPlayer(number, player);
                    }
                    player.InternalCacheProperties(properties);
                    if (!player.ModLocked)
                    {
                        foreach (var entry in properties)
                        {
                            if(TryCheckMod(entry, out string modName))
                            {
                                player.ModName = modName;
                                player.ModLocked = true;
                                break;
                            }
                        }
                    }
                    NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, new object[] { player, properties });
                }
            }
        }

        public static void CheckRoomProperties(Hashtable hash, PhotonPlayer sender)
        {
            NetworkingPeer peer = PhotonNetwork.networkingPeer;
            if(peer == null)
            {
                return;
            }
            if (PhotonNetwork.IsMasterClient)
            {
                if(sender != null)
                {
                    //TODO: Localize
                    Antis.Kick(sender, true, "Atttemption to change room properties");
                    Room room = PhotonNetwork.room;
                    room.Visible = true;
                    room.Open = true;
                    int max = room.MaxPlayers;
                    room.MaxPlayers = max;
                    return;
                }
            }
            if(peer.mCurrentGame != null && hash != null)
            {
                peer.mCurrentGame.CacheProperties(hash);
                NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, new object[] { hash });
                if (PhotonNetwork.automaticallySyncScene)
                {
                    peer.LoadLevelIfSynced();
                }
            }
        }
    }
}
