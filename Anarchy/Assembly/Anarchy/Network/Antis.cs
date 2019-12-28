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
            "i.imgur", "imgur", "discordapp", "postimg", "aotcorehome.files.wordpress", "deviantart", "wmpics.pics"
        };

        internal static bool IsValidURL(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && (uriResult != null && uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        internal static bool IsValidSkinURL(string all_parts, int length, int ID)
        {
            if(all_parts == null)
            {
                return false;
            }
            var urls = all_parts.Split(',');
            if (urls == null)
            {
                return false;
            }
            if (urls.Length != length)
            {
                return false;   
            }
            for (int i = 0; i < urls.Length; i++)
            {
                var url = urls[i];
                if (url.IsNullOrWhiteSpace() || url.ToLower().Contains("transparent"))
                {
                    continue;
                }
                if (!IsValidURL(url) || !ValidLinks.Any(url.Contains))
                {
                    Log.AddLine("HeroSkinRPC", MsgType.Error, ID.ToString(), url);
                    return false;
                }
            }
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

