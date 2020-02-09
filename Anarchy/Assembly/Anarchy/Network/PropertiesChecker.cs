using ExitGames.Client.Photon;

namespace Anarchy.Network
{
    internal static class PropertiesChecker
    {
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
