using Anarchy.Configuration;
using Anarchy.IO;
using UnityEngine;

namespace Anarchy
{
    public static class User
    {
        private const string DefaultProfile = "Default";
        private const string Example = "DefaultExample";
        private const string Extension = ".profile";
        private static readonly string Directory = Application.dataPath + "/Profiles/";

        public static UserSetting AkinaKillTrigger = new UserSetting("akinaKillTrigger");
        public static UserSetting ChatFormat = new UserSetting("chatFormat");
        public static UserSetting ChatFormatSend = new UserSetting("chatFormatSend");
        public static UserSetting ChatPMFormat = new UserSetting("chatPMFormat");
        public static UserSetting ChatPMFormatSend = new UserSetting("chatPMFormatSend");
        public static UserSetting ChatName = new UserSetting("chatName");
        public static UserSetting DieName = new UserSetting("diename");
        public static UserSetting DieNameFormat = new UserSetting("dieStyle");
        public static UserSetting ForestLava = new UserSetting("forestKillTrigger");
        public static UserSetting[] GuildNames = new UserSetting[3] { new UserSetting("guildName"), new UserSetting("guildName2"), new UserSetting("guildName3") };
        public static UserSetting MainColor = new UserSetting("mainColor");
        public static UserSetting MCSwitch = new UserSetting("mcSwitch");
        public static UserSetting Name = new UserSetting("name");
        public static UserSetting RaceFinish = new UserSetting("raceFinish");
        public static UserSetting RacingKillTrigger = new UserSetting("racingKillTrigger");
        public static UserSetting RestartMessage = new UserSetting("restartMsg");
        public static UserSetting SubColor = new UserSetting("subColor");
        public static UserSetting Suicide = new UserSetting("suicide");
        public static UserSetting[] TitanNames = new UserSetting[5] { new UserSetting("titan"), new UserSetting("aberrant"), new UserSetting("jumper"), new UserSetting("crawler"), new UserSetting("punk") };
        public static UserSetting WaveFormat = new UserSetting("wave");

        public static string AllGuildNames
        {
            get
            {
                string guild = string.Empty;
                if(GuildNames[2].Value.Length > 0)
                {
                    guild += GuildNames[2].Value;
                }
                if (GuildNames[1].Value.Length > 0)
                {
                    if (guild != string.Empty)
                        guild += "\n";
                    guild += GuildNames[1].Value;
                }
                if (GuildNames[0].Value.Length > 0)
                {
                    if (guild != string.Empty)
                        guild += "\n";
                    guild += GuildNames[0].Value;
                }
                return guild;
            }
        }

        public static string[] AllProfiles { get; private set; }

        public static string DeathName
        {
            get
            {
                return DieName.PickRandomString().Replace("$name$", Name.Value).Replace("$maincolor$", MainColor.Value).Replace("$subcolor$", SubColor.Value);
            }
        }

        public static string DeathNameFull
        {
            get
            {
                return DieName.Value.Replace("$name$", Name.Value).Replace("$maincolor$", MainColor.Value).Replace("$subcolor$", SubColor.Value);
            }
        }

        public static string MasterClientSwitch
        {
            get
            {
                return MCSwitch.Value.Replace("$name$", Name.Value.ToHTMLFormat()).Replace("$chatName$", ChatName.Value).Replace("$maincolor$", MainColor.Value).Replace("$subcolor$", SubColor.Value);
            }
        }

        public static string MsgRestart
        {
            get
            {
                return RestartMessage.Value.Replace("$name$", Name.Value.ToHTMLFormat()).Replace("$chatName$", ChatName.Value).Replace("$maincolor$", MainColor.Value).Replace("$subcolor$", SubColor.Value);
            }
        }

        public static string ProfileName { get; private set; } = string.Empty;

        public static string RaceName
        {
            get
            {
                return RaceFinish.Value.Replace("$name$", Name.Value).Replace("$maincolor$", MainColor.Value).Replace("$subcolor$", SubColor.Value);
            }
        }

        public static string Chat(int id, string content)
        {
            return ChatFormat.Value.Replace("$ID$", id.ToString()).Replace("$content$", content).Replace("$maincolor$", MainColor.Value).Replace("$subcolor$", SubColor.Value);
        }

        public static string ChatPM(int id, string content)
        {
            return ChatPMFormat.Value.Replace("$ID$", id.ToString()).Replace("$name$", Name.Value.ToHTMLFormat()).Replace("$content$", content).Replace("$chatname$", ChatName.Value).Replace("$maincolor$", MainColor.Value).Replace("$subcolor$", SubColor.Value);
        }

        public static string ChatPMSend(int id, string content)
        {
            return ChatPMFormatSend.Value.Replace("$ID$", id.ToString()).Replace("$name$", Name.Value.ToHTMLFormat()).Replace("$chatName$", ChatName.Value).Replace("$content$", content).Replace("$maincolor$", MainColor.Value).Replace("$subcolor$", SubColor.Value);

        }

        public static string ChatSend(string content)
        {
            return ChatFormatSend.Value.Replace("$content$", content).Replace("$chatName$", ChatName.Value).Replace("$name$", Name.Value.ToHTMLFormat()).Replace("$maincolor$", MainColor.Value).Replace("$subcolor$", SubColor.Value);
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

        public static string DeathFormat(int id, string killer)
        {
            return DieNameFormat.Value.Replace("$ID$", id.ToString()).Replace("$killer$", killer).Replace("$maincolor$", MainColor.Value).Replace("$subcolor$", SubColor.Value);
        }

        public static void DeleteProfile(string nameToDelete)
        {
            if (AllProfiles.Length == 1)
            {
                //Put log
                return;
            }
            foreach(string str in AllProfiles)
            {
                if(str != nameToDelete)
                {
                    LoadProfile(str);
                    System.IO.File.Delete(GetPath(nameToDelete));
                    break;
                }
            }
            RefreshProfilesList();
        }

        public static string FormatColors(string src)
        {
            return src.Replace("$maincolor$", MainColor.Value).Replace("$subcolor$", SubColor.Value);
        }

        private static string GetPath(string name)
        {
            return Directory + name + Extension;
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
            string[] temp = System.IO.Directory.GetFiles(Directory);
            System.Collections.Generic.List<string> tmp = new System.Collections.Generic.List<string>();
            for(int i = 0; i < temp.Length; i++)
            {
                if (temp[i].Contains(Example))
                    continue;
                tmp.Add(temp[i].Remove(0, Directory.Length).Replace(Extension, string.Empty));
            }
            AllProfiles = tmp.ToArray();
        }

        public static void Save()
        {
            UserSetting.SaveSettings();
            using (ConfigFile file = new ConfigFile(Application.dataPath + "/Configuration/Settings.ini", ' ', false))
            {
                file.Load();
                file.SetString("profile", ProfileName);
            }
        }

        public static string Wave(int wave)
        {
            return WaveFormat.Value.Replace("$wave$", wave.ToString()).Replace("$maincolor$", MainColor.Value).Replace("$subcolor$", SubColor.Value);
        }
    }
}
