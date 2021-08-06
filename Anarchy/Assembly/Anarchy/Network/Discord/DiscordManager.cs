using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Anarchy.Network.Discord
{
    //TODO: Delete?
    internal class DiscordManager : MonoBehaviour
    {
        public static string DiscordClientID = "571227991145971732";
        public static readonly long StartTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

        private static bool _ignoreStart;
        private static DiscordRPC.RichPresence _presence;
        private static DiscordRPC.EventHandlers _handlers;
        private static float _time;
        private static bool canUpdate = true;

        public void Start()
        {
            if (_ignoreStart)
            {
                return;
            }

            _ignoreStart = true;
            _presence = new DiscordRPC.RichPresence
            {
                details = "",
                startTimestamp = StartTime,
                largeImageKey = "anarchyicon",
                largeImageText = "",
                smallImageKey = "",
                smallImageText = ""
            };
            _handlers = default;
            DiscordRPC.Initialize(DiscordClientID, ref _handlers, true, null);
            DiscordRPC.UpdatePresence(_presence);
            UpdateStatus();
        }

        private void OnApplicationQuit()
        {
            canUpdate = false;
        }

        public void Update()
        {
            _time += Time.deltaTime;
            if (!(_time > 1f))
            {
                return;
            }

            try
            {
                UpdateStatus();
            }
            catch
            {
            }
            _time = 0f;
        }

        public static void UpdateStatus()
        {
            if (!canUpdate)
            {
                return;
            }
            if (!PhotonNetwork.inRoom)
            {
                if (PhotonNetwork.InsideLobby)
                {
                    _presence.state = "In lobby: " + Regex.Replace(PhotonNetwork.ServerAddress, "app\\-|\\.exitgamescloud\\.com|\\:\\d+", "").ToUpper().Replace("WS://", "").Replace("WSS://", "");

                    switch (PhotonNetwork.ServerAddress.Split(':')[0])
                    {
                        case "142.44.242.29":
                            _presence.state = "In lobby: USA";
                            break;

                        case "135.125.239.180":
                            _presence.state = "In lobby: Europe";
                            break;

                        case "51.79.164.137":
                            _presence.state = "In lobby: Asia";
                            break;

                        case "172.107.193.233":
                            _presence.state = "In lobby: South America";
                            break;

                        default:
                            break;
                    }

                    _presence.partySize = 0;
                    _presence.partyMax = 0;
                    _presence.largeImageKey = "anarchyicon";
                }
                else if (IN_GAME_MAIN_CAMERA.GameType != GameType.Stop)
                {
                    _presence.state = "Solo: " + FengGameManagerMKII.Level.Name;
                    _presence.partySize = 0;
                    _presence.partyMax = 0;
                    _presence.largeImageKey = FengGameManagerMKII.Level.DiscordName;
                    _presence.largeImageText = FengGameManagerMKII.Level.Name;
                }
                else
                {
                    _presence.state = "In menu";
                    _presence.partySize = 0;
                    _presence.partyMax = 0;
                    _presence.largeImageKey = "anarchyicon";
                }
            }
            else
            {
                var text = PhotonNetwork.room.Name.Split('`')[0].RemoveHex();
                _presence.state = "Multiplayer: " + ((text.Length > 30) ? (text.Remove(27) + "...") : text);
                _presence.partySize = PhotonNetwork.room.PlayerCount;
                _presence.partyMax = PhotonNetwork.room.MaxPlayers;
                _presence.largeImageKey = FengGameManagerMKII.Level.DiscordName;
                _presence.largeImageText = FengGameManagerMKII.Level.Name;
            }
            DiscordRPC.UpdatePresence(_presence);
        }
    }
}