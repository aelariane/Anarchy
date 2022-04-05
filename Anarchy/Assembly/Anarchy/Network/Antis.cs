using Anarchy.UI;
using ExitGames.Client.Photon;
using System;
using System.Linq;

namespace Anarchy.Network
{
    internal static class Antis
    {
        private static string[] ValidLinks = new string[]
        {
            "i.imgur.com", "imgur.com", "discordapp.com", "discordapp.net", "postimg", "aotcorehome.files", "deviantart", "wmpics.pics", "puu.sh", "pp.userapi.com",
            "sun9-25.userapi.com", "sun9-35.userapi.com", "sun9-65.userapi.com", "sun9-6.userapi.com", "images.ourclipart.com", "pictureshack.ru", "savepice.ru"
        };

        public static void Kick(PhotonPlayer player, bool ban, string reason = "")
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            player.RCIgnored = true;
            if (ban)
            {
                BanList.Ban(player, reason);
            }
            if (reason != string.Empty)
            {
                //TODO: Make banlist and localize
                UI.Chat.Add($"Player {player.ID} autobanned. Reason: {reason}");
            }
            PhotonNetwork.networkingPeer.OpRaiseEvent(203, null, true, player.ToOption());
            FengGameManagerMKII.FGM.BasePV.RPC("ignorePlayer", PhotonTargets.Others, new object[] { player.ID });
        }

        internal static bool IsValidURL(string url, out Uri uri)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out uri) && (uri != null && uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        internal static bool IsValidSkinURL(ref string all_parts, int length, int ID)
        {
            if (all_parts == null)
            {
                return false;
            }
            var urls = all_parts.Split(',');
            //if (urls.Length != length)
            //{
            //    return false;
            //}
            Uri uri = null;
            for (int i = 0; i < urls.Length; i++)
            {
                var url = urls[i];
                if (url.IsNullOrWhiteSpace() || url.ToLower().Contains("transparent") || url.ToLower().StartsWith("file:///"))
                {
                    continue;
                }
                url = url.TrimStart();
                if (!url.StartsWith("https://") && !url.StartsWith("http://"))
                {
                    url = "http://" + url;
                }
                if (!IsValidURL(url, out uri))
                {
                    urls[i] = string.Empty;
                }
                else if (!ValidLinks.Any(uri.Host.Contains))
                {
                    urls[i] = string.Empty;
                }
            }
            all_parts = string.Join(",", urls);
            return true;
        }

        public static void SuperKick(PhotonPlayer player, bool ban, string reason = "")
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            player.RCIgnored = true;
            if (ban)
            {
                BanList.Ban(player, reason);
            }
            if (reason != string.Empty)
            {
                //TODO: Make banlist and localize
                UI.Chat.Add($"Player {player.ID} autobanned. Reason: {reason}");
            }
            var data = new Hashtable();
            data[(byte)0] = "hook";
            data[(byte)6] = PhotonNetwork.networkingPeer.ServerTimeInMilliSeconds;
            data[(byte)7] = 2;
            PhotonNetwork.networkingPeer.OpRaiseEvent(202, data, true, player.ToOption());
            PhotonNetwork.networkingPeer.OpRaiseEvent(203, null, true, player.ToOption());
        }
    }
}
