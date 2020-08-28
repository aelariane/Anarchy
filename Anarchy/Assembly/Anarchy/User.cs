using System.Linq;
using Anarchy.Configuration;
using Anarchy.IO;
using UnityEngine;

namespace Anarchy
{
    public static class User
    {
        private const string Example = "DefaultExample";
        private const string Extension = ".profile";
        
        private static readonly string directory = Application.dataPath + "/Profiles/";

        public static readonly UserSetting Name = new UserSetting("name");
        public static readonly UserSetting[] GuildNames =
        { 
            new UserSetting("guildName"),
            new UserSetting("guildName2"), 
            new UserSetting("guildName3") 
        };
        
        public static readonly UserSetting ChatName = new UserSetting("chatName");
        public static readonly UserSetting MainColor = new UserSetting("mainColor");
        public static readonly UserSetting SubColor = new UserSetting("subColor");
        
        public static readonly UserSetting Suicide = new UserSetting("suicide");
        public static readonly UserSetting AkinaKillTrigger = new UserSetting("akinaKillTrigger");
        public static readonly UserSetting ForestLavaKillTrigger = new UserSetting("forestKillTrigger");
        public static readonly UserSetting RacingKillTrigger = new UserSetting("racingKillTrigger");

        public static readonly UserSetting[] TitanNames = 
        { 
            new UserSetting("titan"),
            new UserSetting("aberrant"), new UserSetting("jumper"),
            new UserSetting("crawler"), new UserSetting("punk") 
        };
        
        public static readonly UserSetting DieName = new UserSetting("diename");
        public static readonly UserSetting RaceFinish = new UserSetting("raceFinish");
        public static readonly UserSetting DieNameFormat = new UserSetting("dieStyle");
        public static readonly UserSetting WaveFormat = new UserSetting("wave");
        
        public static readonly UserSetting ChatFormat = new UserSetting("chatFormat");
        public static readonly UserSetting ChatFormatSend = new UserSetting("chatFormatSend");
        public static readonly UserSetting ChatPmFormat = new UserSetting("chatPMFormat");
        public static readonly UserSetting ChatPmFormatSend = new UserSetting("chatPMFormatSend");
        
        public static readonly UserSetting McSwitch = new UserSetting("mcSwitch");
        public static readonly UserSetting RestartMessage = new UserSetting("restartMsg");
        
        public static string[] AllProfiles { get; private set; }

        public static string AllGuildNames => 
            string.Join(
                "\n",
                GuildNames.Where(x => x.Value.TrimStart().Length > 0).Select(x => x.Value).Reverse().ToArray()
            );
        
        public static string ProfileName { get; private set; } = string.Empty;

        public static string DeathName => DieName.PickRandomString()
            .Replace("$name$", Name.Value)
            .Replace("$maincolor$", MainColor.Value)
            .Replace("$subcolor$", SubColor.Value);

        public static string DeathNameFull => DieName.Value.Replace("$name$", Name.Value)
            .Replace("$maincolor$", MainColor.Value)
            .Replace("$subcolor$", SubColor.Value);

        public static string MasterClientSwitch => McSwitch.Value.Replace("$name$", Name.Value.ToHTMLFormat())
            .Replace("$chatName$", ChatName.Value)
            .Replace("$maincolor$", MainColor.Value)
            .Replace("$subcolor$", SubColor.Value);

        public static string MsgRestart => RestartMessage.Value.Length <= 0 ? string.Empty : 
            RestartMessage.Value.Replace("$name$", Name.Value.ToHTMLFormat())
            .Replace("$chatName$", ChatName.Value)
            .Replace("$maincolor$", MainColor.Value)
            .Replace("$subcolor$", SubColor.Value);

        public static string RaceName => RaceFinish.Value.Replace("$name$", Name.Value)
            .Replace("$maincolor$", MainColor.Value)
            .Replace("$subcolor$", SubColor.Value);

        public static string Wave(int wave)
        {
            return WaveFormat.Value.Replace("$wave$", wave.ToString())
                .Replace("$maincolor$", MainColor.Value)
                .Replace("$subcolor$", SubColor.Value);
        }

        public static string Chat(int id, string content)
        {
            return ChatFormat.Value.Replace("$ID$", id.ToString())
                .Replace("$content$", content)
                .Replace("$maincolor$", MainColor.Value)
                .Replace("$subcolor$", SubColor.Value);
        }

        public static string ChatPm(int id, string content)
        {
            return ChatPmFormat.Value.Replace("$ID$", id.ToString())
                .Replace("$name$", Name.Value.ToHTMLFormat())
                .Replace("$content$", content)
                .Replace("$chatname$", ChatName.Value)
                .Replace("$maincolor$", MainColor.Value)
                .Replace("$subcolor$", SubColor.Value);
        }

        public static string ChatPmSend(int id, string content)
        {
            return ChatPmFormatSend.Value.Replace("$ID$", id.ToString())
                .Replace("$name$", Name.Value.ToHTMLFormat())
                .Replace("$chatName$", ChatName.Value)
                .Replace("$content$", content)
                .Replace("$maincolor$", MainColor.Value)
                .Replace("$subcolor$", SubColor.Value);
        }

        public static string ChatSend(string content)
        {
            return ChatFormatSend.Value.Replace("$content$", content)
                .Replace("$chatName$", ChatName.Value)
                .Replace("$name$", Name.Value.ToHTMLFormat())
                .Replace("$maincolor$", MainColor.Value)
                .Replace("$subcolor$", SubColor.Value);
        }
        
        public static string DeathFormat(int id, string killer)
        {
            return DieNameFormat.Value.Replace("$ID$", id.ToString())
                .Replace("$killer$", killer)
                .Replace("$maincolor$", MainColor.Value)
                .Replace("$subcolor$", SubColor.Value);
        }
        
        public static string FormatColors(string src)
        {
            return src.Replace("$maincolor$", MainColor.Value)
                .Replace("$subcolor$", SubColor.Value);
        }

        public static void CopyProfile(string newProfile, string copyFrom)
        {
            System.IO.File.Copy(GetPath(copyFrom), GetPath(newProfile));
            LoadProfile(newProfile);
        }

        private static void CreateProfile(string profileName)
        {
            if (System.IO.File.Exists(GetPath(profileName)))
            {
                System.IO.File.Delete(GetPath(profileName));
            }
            System.IO.File.Copy(GetPath(Example), GetPath(profileName));
            RefreshProfilesList();
        }

        public static void DeleteProfile(string nameToDelete)
        {
            if (AllProfiles.Length == 1)
            {
                //Put log
                return;
            }
            foreach (var str in AllProfiles)
            {
                if (str != nameToDelete)
                {
                    LoadProfile(str);
                    System.IO.File.Delete(GetPath(nameToDelete));
                    break;
                }
            }
            RefreshProfilesList();
        }

        private static string GetPath(string name)
        {
            return directory + name + Extension;
        }

        public static void Load()
        {
            UserSetting.LoadSettings();
        }

        public static void LoadProfile(string name)
        {
            if (!System.IO.File.Exists(GetPath(name)))
            {
                CreateProfile(name);
            }
            UserSetting.Change(GetPath(name));
            ProfileName = name;
            RefreshProfilesList();
        }

        private static void RefreshProfilesList()
        {
            var temp = System.IO.Directory.GetFiles(directory);
            var tmp = 
                (from t in temp 
                where !t.Contains(Example) 
                select t.Remove(0, directory.Length)
                    .Replace(Extension, string.Empty))
                .ToList();
            AllProfiles = tmp.ToArray();
        }

        public static void Save()
        {
            UserSetting.SaveSettings();
            using (var file = new ConfigFile(Application.dataPath + "/Configuration/Settings.ini", ' ', false))
            {
                file.Load();
                file.SetString("profile", ProfileName);
            }
        }
    }
}
