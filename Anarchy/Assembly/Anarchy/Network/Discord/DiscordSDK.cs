using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Anarchy.Network.Discord
{
    internal class DiscordSDK : MonoBehaviour
    {
        public const long DiscordClientID = 571227991145971732;
        public static readonly long StartTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

        private SDK.Discord _Discord;

        private SDK.Activity _Activity;
        private SDK.ActivityManager _ActivityManager;

        private float _time;

        private void DiscordLogCallback(SDK.LogLevel level, string message)
        {
            Debug.Log($"Discord:{level.ToString()} : {message}");
        }
        
        private void Start()
        {
            _Discord = new SDK.Discord(DiscordClientID, (UInt64)SDK.CreateFlags.NoRequireDiscord);
            if (_Discord == null)
            {
                this.enabled = false;
                return;
            }

            _Discord.SetLogHook(SDK.LogLevel.Debug, DiscordLogCallback);

            _ActivityManager = _Discord.GetActivityManager();

            _Activity = new SDK.Activity
            {
                State = "",
                Details = "",
                Timestamps =
                {
                    Start = StartTime,
                },
                Assets =
                {
                    LargeImage = "anarchyicon",
                    LargeText = "",
                    SmallImage = "",
                    SmallText = "",
                },
                /*
                Party =
                {
                    Id = "foo partyID",
                    Size = {
                        CurrentSize = 1,
                        MaxSize = 4,
                    },
                },
                Secrets =
                {
                    Match = "foo matchSecret",
                    Join = "foo joinSecret",
                    Spectate = "foo spectateSecret",
                },
                */
                Instance = true
            };
        }

        private void Update()
        {
            _time += Time.deltaTime;
            if (!(_time > 1f))
            {
                return;
            }

            try
            {
                _Discord.RunCallbacks();
                UpdateStatus();
            }
            catch
            {
            }
            _time = 0f;
        }

        private void OnApplicationQuit()
        {
            _Discord.Dispose();
        }

        public void UpdateStatus()
        {
            if (!PhotonNetwork.inRoom)
            {
                if (PhotonNetwork.InsideLobby)
                {
                    _Activity.State = "In lobby: " + Regex.Replace(PhotonNetwork.ServerAddress, "app\\-|\\.exitgamescloud\\.com|\\:\\d+", "").ToUpper().Replace("WS://", "").Replace("WSS://", "");
                    _Activity.Party.Size.CurrentSize = 0;
                    _Activity.Party.Size.MaxSize = 0;
                    _Activity.Assets.LargeImage = "anarchyicon";
                }
                else if (IN_GAME_MAIN_CAMERA.GameType != GameType.Stop)
                {
                    _Activity.State = "Solo: " + FengGameManagerMKII.Level.Name;
                    _Activity.Party.Size.CurrentSize = 0;
                    _Activity.Party.Size.MaxSize = 0;
                    _Activity.Assets.LargeImage = FengGameManagerMKII.Level.DiscordName;
                    _Activity.Assets.LargeText = FengGameManagerMKII.Level.Name;
                }
                else
                {
                    _Activity.State = "In menu";
                    _Activity.Party.Size.CurrentSize = 0;
                    _Activity.Party.Size.MaxSize = 0;
                    _Activity.Assets.LargeImage = "anarchyicon";
                }
            }
            else
            {
                var text = PhotonNetwork.room.Name.Split('`')[0].RemoveHex();
                _Activity.State = "Multiplayer: " + ((text.Length > 30) ? (text.Remove(27) + "...") : text);
                _Activity.Party.Size.CurrentSize = PhotonNetwork.room.PlayerCount;
                _Activity.Party.Size.MaxSize = PhotonNetwork.room.MaxPlayers;
                _Activity.Assets.LargeImage = FengGameManagerMKII.Level.DiscordName;
                _Activity.Assets.LargeText = FengGameManagerMKII.Level.Name;
            }

            _ActivityManager.UpdateActivity(_Activity, (result) =>
            {
                if (result != SDK.Result.Ok)
                {
                    Debug.Log("Failed to update Activity.");
                }
            });
        }
    }
}
