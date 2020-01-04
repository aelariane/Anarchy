using System;
using System.Linq;
using Anarchy.UI;
using ExitGames.Client.Photon;

namespace Anarchy.Network
{
    internal static class Antis
    {
        private static readonly string[] AllowedProperties = new string[] { "kills", "deaths", "total_dmg", "max_dmg", "isTitan", "dead", "RCteam", "team", "guildName" };

        private static string[] ValidLinks = new string[]
        {
            "i.imgur.com", "imgur.com", "discordapp", "postimg", "aotcorehome.files.wordpress", "deviantart", "wmpics.pics", "puu.sh"
        };

        internal static bool IsValidURL(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && (uriResult != null && uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        internal static bool IsValidSkinURL(ref string all_parts, int length, int ID)
        {
            if(all_parts == null)
            {
                return false;
            }
            var urls = all_parts.Split(',');
            if (urls.Length != length)
            {
                return false;   
            }
            for (int i = 0; i < urls.Length; i++)
            {
                var url = urls[i];
                if (url.IsNullOrWhiteSpace() || url.Trim().ToLower().Contains("transparent") || url.ToLower().StartsWith("file:///"))
                {
                    continue;
                }
                if(!url.StartsWith("https://") && !url.StartsWith("http://"))
                {
                    url = "https://" + url;
                }
                if (!IsValidURL(url))
                {
                    Log.AddLine("invalidSkinUrl", MsgType.Error, ID.ToString());
                    urls[i] = string.Empty;
                }
                else if (!ValidLinks.Any(url.Contains))
                {
                    Log.AddLine("invalidSkinLink", MsgType.Error, ID.ToString());
                    urls[i] = string.Empty;
                }
            }
            all_parts = string.Join(",", urls);
            return true;
        }

        //public static bool CheckEventProperties(Hashtable hash, PhotonPlayer sender, int key)
        //{
        //    NetworkingPeer peer = PhotonNetwork.networkingPeer;
        //    if (hash == null || hash.Count <= 0)
        //    {
        //        return false;
        //    }
        //    CheckUnusualProperties(hash, sender);
        //    PhotonPlayer player = PhotonPlayer.Find(key);
        //    if (player == null)
        //    {
        //        return false;
        //    }
        //    if (player.IsLocal)
        //    {
        //    }
        //    return true;
        //}

        //private static void CheckUnusualProperties(Hashtable hash, PhotonPlayer sender)
        //{

        //}
    }
}

