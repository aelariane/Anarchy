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
            "i.imgur.com", "imgur.com", "discordapp.com", "postimg", "aotcorehome.files", "deviantart", "wmpics.pics", "puu.sh", "pp.userapi.com",
            "sun9-25.userapi.com", "sun9-35.userapi.com", "sun9-65.userapi.com", "sun9-.userapi.com", "images.ourclipart.com", "pictureshack.ru"
        };

        internal static bool IsValidURL(string url, out Uri uri)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out uri) && (uri != null && uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
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
            Uri uri = null;
            for (int i = 0; i < urls.Length; i++)
            {
                var url = urls[i];
                if (url.IsNullOrWhiteSpace() || url.ToLower().Contains("transparent") || url.ToLower().StartsWith("file:///"))
                {
                    continue;
                }
                url = url.Trim();
                if(!url.StartsWith("https://") && !url.StartsWith("http://"))
                {
                    url = "http://" + url;
                }
                if (!IsValidURL(url, out uri))
                {
                    Log.AddLine("invalidSkinUrl", MsgType.Error, ID.ToString(), url);
                    urls[i] = string.Empty;
                }
                else if (!ValidLinks.Any(uri.Host.Contains))
                {
                    Log.AddLine("invalidSkinLink", MsgType.Error, ID.ToString(), url);
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

