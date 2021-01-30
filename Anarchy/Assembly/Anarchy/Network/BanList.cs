using Anarchy.IO;
using System;
using System.Collections.Generic;

namespace Anarchy.Network
{
    internal static class BanList
    {
        public static List<BanInfo> BannedPlayers = new List<BanInfo>();
        public static LogFile BanFile = new LogFile("BannedPlayers");
        public static ConfigFile PermaBanned = new ConfigFile(UnityEngine.Application.dataPath + "/Configuration/PermaBannedPlayers.cfg", '`', true);

        public static string[] BannedNames
        {
            get
            {
                string[] result = new string[BannedPlayers.Count];
                for (int i = 0; i < BannedPlayers.Count; i++)
                {
                    result[i] = BannedPlayers[i].Name;
                }
                return result;
            }
        }

        public static bool Banned(string name)
        {
            if (name.Length == 0)
            {
                return false;
            }
            string hexRemoved = name.RemoveHex();
            for (int i = 0; i < BannedPlayers.Count; i++)
            {
                if (BannedPlayers[i].Name.Equals(hexRemoved))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Ban(PhotonPlayer player, string reason = "")
        {
            if (player.UIName == "Unknown" || Banned(player.UIName.RemoveHex()))
            {
                return false;
            }
            BannedPlayers.Add(new BanInfo(player, reason == string.Empty ? "Manual ban" : reason));
            InfoLine($"Player [{player.ID}] {player.UIName.RemoveHex()} banned. Reason: {(reason == string.Empty ? "Manual ban" : reason)}");
            return true;
        }

        public static bool PermaBan(PhotonPlayer player, string reason = "")
        {
            if (player.UIName == "Unknown" || Banned(player.UIName.RemoveHex()))
            {
                return false;
            }
            BannedPlayers.Add(new BanInfo(player, reason == string.Empty ? "Manual ban" : reason, true));
            InfoLine($"Player [{player.ID}] {player.UIName.RemoveHex()} permabanned. Reason: {(reason == string.Empty ? "Manual ban" : reason)}");
            return true;
        }

        private static void InfoLine(string str)
        {
            BanFile.WriteLine($"[{DateTime.Now.ToLongTimeString()}] " + str);
        }

        public static void Load()
        {
            BannedPlayers.Clear();
            string[] names = PermaBanned.GetString("names").Split(',');
            if (names.Length > 0)
            {
                foreach (string name in names)
                {
                    BannedPlayers.Add(new BanInfo(name, PermaBanned.GetString(name), true));
                }
            }
        }

        public static void Save()
        {
            PermaBanned.Load();
            List<string> names = new List<string>();
            foreach (var info in BannedPlayers)
            {
                if (info.Perma)
                {
                    names.Add(info.Name);
                }
            }
            if (names.Count > 0)
            {
                if (names.Count > 1)
                {
                    PermaBanned.SetString("names", string.Join(",", names.ToArray()));
                }
                else
                {
                    PermaBanned.SetString("names", names[0]);
                }
                foreach (var info in BannedPlayers)
                {
                    if (info.Perma)
                    {
                        PermaBanned.SetString(info.Name, info.Reason);
                    }
                }
            }
            else
            {
                PermaBanned.SetString("names", string.Empty);
            }
            PermaBanned.Save();
            BannedPlayers.Clear();
            Load();
        }

        public static bool Unban(string name)
        {
            name = name.ToLower().RemoveHex();
            for (int i = 0; i < BannedPlayers.Count; i++)
            {
                if (BannedPlayers[i].Name.ToLower() == name)
                {
                    InfoLine($"Player [{BannedPlayers[i].ID}] {BannedPlayers[i].Name} unbanned. Ban reason was: {BannedPlayers[i].Reason}");
                    BannedPlayers.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public static bool Unban(int id)
        {
            for (int i = 0; i < BannedPlayers.Count; i++)
            {
                if (BannedPlayers[i].ID == id)
                {
                    InfoLine($"Player [{id}] {BannedPlayers[i].Name} unbanned. Ban reason was: {BannedPlayers[i].Reason}");
                    BannedPlayers.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public class BanInfo
        {
            public readonly int ID = -1;
            public string Name = string.Empty;
            public bool Perma = false;
            public string Reason = string.Empty;

            public BanInfo(PhotonPlayer player, string reason, bool perma = false) : this(player.UIName.RemoveHex(), reason, perma)
            {
                ID = player.ID;
            }

            public BanInfo(string player, string reason, bool perma = false)
            {
                Name = player;
                Reason = reason;
                Perma = perma;
            }
        }
    }
}