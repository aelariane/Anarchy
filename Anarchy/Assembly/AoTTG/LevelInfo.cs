using System;
using UnityEngine;

public class LevelInfo
{
    private static bool initialized;
    public string Description;
    public string DiscordName;
    public int EnemyNumber;
    public bool Hint;
    public bool HorsesEnabled;
    public bool LavaMode;
    public Type LogicType = typeof(GameLogic.GameLogic);
    public string MapName;
    public Minimap.Preset minimapPreset;
    public GameMode Mode;
    public string Name;
    public bool NoCrawler;
    public bool PunksEnabled = true;
    public bool PVPEnabled;
    public RespawnMode RespawnMode;
    public bool Supply = true;
    public bool TeamTitan;
    public bool HasFog = false;

    public static LevelInfo[] Levels { get; private set; }

    public static LevelInfo GetInfo(string name)
    {
        return GetInfo(name, true);
    }

    public static LevelInfo GetInfo(string name, bool initLogic)
    {
        InitData();
        foreach (LevelInfo levelInfo in Levels)
        {
            if (levelInfo.Name == name)
            {
                if (initLogic)
                {
                    FengGameManagerMKII.FGM.logic = (GameLogic.GameLogic)System.Activator.CreateInstance(levelInfo.LogicType);
                }
                return levelInfo;
            }
        }
        return null;
    }

    private static void InitData()
    {
        if (!initialized)
        {
            initialized = true;
            Levels = new LevelInfo[33];
            Levels[0] = new LevelInfo();
            Levels[1] = new LevelInfo();
            Levels[2] = new LevelInfo();
            Levels[3] = new LevelInfo();
            Levels[4] = new LevelInfo();
            Levels[5] = new LevelInfo();
            Levels[6] = new LevelInfo();
            Levels[7] = new LevelInfo();
            Levels[8] = new LevelInfo();
            Levels[9] = new LevelInfo();
            Levels[10] = new LevelInfo();
            Levels[11] = new LevelInfo();
            Levels[12] = new LevelInfo();
            Levels[13] = new LevelInfo();
            Levels[14] = new LevelInfo();
            Levels[15] = new LevelInfo();
            Levels[16] = new LevelInfo();
            Levels[17] = new LevelInfo();
            Levels[18] = new LevelInfo();
            Levels[19] = new LevelInfo();
            Levels[20] = new LevelInfo();
            Levels[21] = new LevelInfo();
            Levels[22] = new LevelInfo();
            Levels[23] = new LevelInfo();
            Levels[24] = new LevelInfo();
            Levels[25] = new LevelInfo();
            Levels[26] = new LevelInfo();

            Levels[0].Name = "The City";
            Levels[0].MapName = "The City I";
            Levels[0].Description = "kill all the titans with your friends.(No RESPAWN/SUPPLY/PLAY AS TITAN)";
            Levels[0].EnemyNumber = 10;
            Levels[0].Mode = GameMode.KillTitan;
            Levels[0].RespawnMode = RespawnMode.NEVER;
            Levels[0].Supply = true;
            Levels[0].TeamTitan = true;
            Levels[0].PVPEnabled = true;
            Levels[0].DiscordName = "city";
            Levels[0].LogicType = typeof(GameLogic.KillTitanLogic);

            Levels[1].Name = "The City II";
            Levels[1].MapName = "The City I";
            Levels[1].Description = "Fight the titans with your friends.(RESPAWN AFTER 10 SECONDS/SUPPLY/TEAM TITAN)";
            Levels[1].EnemyNumber = 10;
            Levels[1].Mode = GameMode.KillTitan;
            Levels[1].RespawnMode = RespawnMode.DEATHMATCH;
            Levels[1].Supply = true;
            Levels[1].TeamTitan = true;
            Levels[1].PVPEnabled = true;
            Levels[1].DiscordName = "city";
            Levels[1].LogicType = typeof(GameLogic.KillTitanLogic);

            //Levels[2].Name = "Cage Fighting";
            //Levels[2].MapName = "Cage Fighting";
            //Levels[2].Description = "2 players in different cages. when you kill a titan,  one or more titan will spawn to your opponent's cage.";
            //Levels[2].EnemyNumber = 1;
            //Levels[2].Mode = GameMode.CAGE_FIGHT;
            //Levels[2].RespawnMode = RespawnMode.NEVER;
            //Levels[2].DiscordName = "cage-fight";

            Levels[3].Name = "The Forest";
            Levels[3].MapName = "The Forest";
            Levels[3].Description = "The Forest Of Giant Trees.(No RESPAWN/SUPPLY/PLAY AS TITAN)";
            Levels[3].EnemyNumber = 5;
            Levels[3].Mode = GameMode.KillTitan;
            Levels[3].RespawnMode = RespawnMode.NEVER;
            Levels[3].Supply = true;
            Levels[3].TeamTitan = true;
            Levels[3].PVPEnabled = true;
            Levels[3].DiscordName = "forest";
            Levels[3].LogicType = typeof(GameLogic.KillTitanLogic);
            Levels[3].HasFog = true;

            Levels[4].Name = "The Forest II";
            Levels[4].MapName = "The Forest";
            Levels[4].Description = "Survive for 20 waves.";
            Levels[4].EnemyNumber = 3;
            Levels[4].Mode = GameMode.SurviveMode;
            Levels[4].RespawnMode = RespawnMode.NEVER;
            Levels[4].Supply = true;
            Levels[4].DiscordName = "forest";
            Levels[4].LogicType = typeof(GameLogic.SurviveLogic);
            Levels[4].HasFog = true;

            Levels[5].Name = "The Forest III";
            Levels[5].MapName = "The Forest";
            Levels[5].Description = "Survive for 20 waves.player will respawn in every new wave";
            Levels[5].EnemyNumber = 3;
            Levels[5].Mode = GameMode.SurviveMode;
            Levels[5].RespawnMode = RespawnMode.NEWROUND;
            Levels[5].Supply = true;
            Levels[5].DiscordName = "forest";
            Levels[5].LogicType = typeof(GameLogic.SurviveLogic);
            Levels[5].HasFog = true;

            Levels[6].Name = "Annie";
            Levels[6].MapName = "The Forest";
            Levels[6].Description = "Nape Armor/ Ankle Armor:\nNormal:1000/50\nHard:2500/100\nAbnormal:4000/200\nYou only have 1 life.Don't do this alone.";
            Levels[6].EnemyNumber = 15;
            Levels[6].Mode = GameMode.KillTitan;
            Levels[6].RespawnMode = RespawnMode.NEVER;
            Levels[6].PunksEnabled = false;
            Levels[6].PVPEnabled = true;
            Levels[6].DiscordName = "annie";
            Levels[6].LogicType = typeof(GameLogic.KillTitanLogic);
            Levels[6].HasFog = true;

            Levels[7].Name = "Annie II";
            Levels[7].MapName = "The Forest";
            Levels[7].Description = "Nape Armor/ Ankle Armor:\nNormal:1000/50\nHard:3000/200\nAbnormal:6000/1000\n(RESPAWN AFTER 10 SECONDS)";
            Levels[7].EnemyNumber = 15;
            Levels[7].Mode = GameMode.KillTitan;
            Levels[7].RespawnMode = RespawnMode.DEATHMATCH;
            Levels[7].PunksEnabled = false;
            Levels[7].PVPEnabled = true;
            Levels[7].DiscordName = "annie";
            Levels[7].LogicType = typeof(GameLogic.KillTitanLogic);
            Levels[7].HasFog = true;

            Levels[8].Name = "Colossal Titan";
            Levels[8].MapName = "Colossal Titan";
            Levels[8].Description = "Defeat the Colossal Titan.\nPrevent the abnormal titan from running to the north gate.\n Nape Armor:\n Normal:2000\nHard:3500\nAbnormal:5000\n";
            Levels[8].EnemyNumber = 2;
            Levels[8].Mode = GameMode.BossFightCT;
            Levels[8].RespawnMode = RespawnMode.NEVER;
            Levels[8].DiscordName = "colossal";
            Levels[8].LogicType = typeof(GameLogic.CTFightLogic);

            Levels[9].Name = "Colossal Titan II";
            Levels[9].MapName = "Colossal Titan";
            Levels[9].Description = "Defeat the Colossal Titan.\nPrevent the abnormal titan from running to the north gate.\n Nape Armor:\n Normal:5000\nHard:8000\nAbnormal:12000\n(RESPAWN AFTER 10 SECONDS)";
            Levels[9].EnemyNumber = 2;
            Levels[9].Mode = GameMode.BossFightCT;
            Levels[9].RespawnMode = RespawnMode.DEATHMATCH;
            Levels[9].DiscordName = "colossal";
            Levels[9].LogicType = typeof(GameLogic.CTFightLogic);

            Levels[10].Name = "Trost";
            Levels[10].MapName = "Colossal Titan";
            Levels[10].Description = "Escort Titan Eren";
            Levels[10].EnemyNumber = 2;
            Levels[10].Mode = GameMode.Trost;
            Levels[10].RespawnMode = RespawnMode.NEVER;
            Levels[10].PunksEnabled = false;
            Levels[10].DiscordName = "city";
            Levels[10].LogicType = typeof(GameLogic.TrostLogic);

            Levels[11].Name = "Trost II";
            Levels[11].MapName = "Colossal Titan";
            Levels[11].Description = "Escort Titan Eren(RESPAWN AFTER 10 SECONDS)";
            Levels[11].EnemyNumber = 2;
            Levels[11].Mode = GameMode.Trost;
            Levels[11].RespawnMode = RespawnMode.DEATHMATCH;
            Levels[11].PunksEnabled = false;
            Levels[11].DiscordName = "city";
            Levels[11].LogicType = typeof(GameLogic.TrostLogic);

            Levels[12].Name = "[S]City";
            Levels[12].MapName = "The City I";
            Levels[12].Description = "Kill all 15 Titans";
            Levels[12].EnemyNumber = 15;
            Levels[12].Mode = GameMode.KillTitan;
            Levels[12].RespawnMode = RespawnMode.NEVER;
            Levels[12].Supply = true;
            Levels[12].DiscordName = "city";
            Levels[12].LogicType = typeof(GameLogic.KillTitanLogic);

            Levels[13].Name = "[S]Forest";
            Levels[13].MapName = "The Forest";
            Levels[13].Description = string.Empty;
            Levels[13].EnemyNumber = 15;
            Levels[13].Mode = GameMode.KillTitan;
            Levels[13].RespawnMode = RespawnMode.NEVER;
            Levels[13].Supply = true;
            Levels[13].DiscordName = "forest";
            Levels[13].LogicType = typeof(GameLogic.KillTitanLogic);
            Levels[13].HasFog = true;

            Levels[14].Name = "[S]Forest Survive(no crawler)";
            Levels[14].MapName = "The Forest";
            Levels[14].Description = string.Empty;
            Levels[14].EnemyNumber = 3;
            Levels[14].Mode = GameMode.SurviveMode;
            Levels[14].RespawnMode = RespawnMode.NEVER;
            Levels[14].Supply = true;
            Levels[14].NoCrawler = true;
            Levels[14].PunksEnabled = true;
            Levels[14].DiscordName = "forest";
            Levels[14].LogicType = typeof(GameLogic.SurviveLogic);
            Levels[14].HasFog = true;

            Levels[15].Name = "[S]Tutorial";
            Levels[15].MapName = "tutorial";
            Levels[15].Description = string.Empty;
            Levels[15].EnemyNumber = 1;
            Levels[15].Mode = GameMode.KillTitan;
            Levels[15].RespawnMode = RespawnMode.NEVER;
            Levels[15].Supply = true;
            Levels[15].Hint = true;
            Levels[15].PunksEnabled = false;
            Levels[15].DiscordName = "anarchyicon";
            Levels[15].LogicType = typeof(GameLogic.KillTitanLogic);

            Levels[16].Name = "[S]Battle training";
            Levels[16].MapName = "tutorial 1";
            Levels[16].Description = string.Empty;
            Levels[16].EnemyNumber = 7;
            Levels[16].Mode = GameMode.KillTitan;
            Levels[16].RespawnMode = RespawnMode.NEVER;
            Levels[16].Supply = true;
            Levels[16].PunksEnabled = false;
            Levels[16].DiscordName = "anarchyicon";
            Levels[16].LogicType = typeof(GameLogic.KillTitanLogic);

            Levels[17].Name = "The Forest IV  - LAVA";
            Levels[17].MapName = "The Forest";
            Levels[17].Description = "Survive for 20 waves.player will respawn in every new wave.\nNO CRAWLERS\n***YOU CAN'T TOUCH THE GROUND!***";
            Levels[17].EnemyNumber = 3;
            Levels[17].Mode = GameMode.SurviveMode;
            Levels[17].RespawnMode = RespawnMode.NEWROUND;
            Levels[17].Supply = true;
            Levels[17].NoCrawler = true;
            Levels[17].LavaMode = true;
            Levels[17].DiscordName = "forest";
            Levels[17].LogicType = typeof(GameLogic.SurviveLogic);
            Levels[17].HasFog = true;

            Levels[18].Name = "[S]Racing - Akina";
            Levels[18].MapName = "track - akina";
            Levels[18].Description = string.Empty;
            Levels[18].EnemyNumber = 0;
            Levels[18].Mode = GameMode.Racing;
            Levels[18].RespawnMode = RespawnMode.NEVER;
            Levels[18].Supply = false;
            Levels[18].DiscordName = "racing-akina";
            Levels[18].LogicType = typeof(GameLogic.RacingLogic);

            Levels[19].Name = "Racing - Akina";
            Levels[19].MapName = "track - akina";
            Levels[19].Description = string.Empty;
            Levels[19].EnemyNumber = 0;
            Levels[19].Mode = GameMode.Racing;
            Levels[19].RespawnMode = RespawnMode.NEVER;
            Levels[19].Supply = false;
            Levels[19].PVPEnabled = true;
            Levels[19].DiscordName = "racing-akina";
            Levels[19].LogicType = typeof(GameLogic.RacingLogic);

            Levels[20].Name = "Outside The Walls";
            Levels[20].MapName = "OutSide";
            Levels[20].Description = "Capture Checkpoint mode.";
            Levels[20].EnemyNumber = 0;
            Levels[20].Mode = GameMode.PVP_CAPTURE;
            Levels[20].RespawnMode = RespawnMode.DEATHMATCH;
            Levels[20].Supply = true;
            Levels[20].HorsesEnabled = true;
            Levels[20].TeamTitan = true;
            Levels[20].DiscordName = "outside-the-walls";
            Levels[20].LogicType = typeof(GameLogic.PVPCaptureLogic);

            Levels[21].Name = "The City III";
            Levels[21].MapName = "The City I";
            Levels[21].Description = "Capture Checkpoint mode.";
            Levels[21].EnemyNumber = 0;
            Levels[21].Mode = GameMode.PVP_CAPTURE;
            Levels[21].RespawnMode = RespawnMode.DEATHMATCH;
            Levels[21].Supply = true;
            Levels[21].HorsesEnabled = false;
            Levels[21].TeamTitan = true;
            Levels[21].DiscordName = "city";
            Levels[21].LogicType = typeof(GameLogic.PVPCaptureLogic);

            Levels[22].Name = "Cave Fight";
            Levels[22].MapName = "CaveFight";
            Levels[22].Description = "***Spoiler Alarm!***";
            Levels[22].EnemyNumber = -1;
            Levels[22].Mode = GameMode.PvpAhss;
            Levels[22].RespawnMode = RespawnMode.NEVER;
            Levels[22].Supply = true;
            Levels[22].HorsesEnabled = false;
            Levels[22].TeamTitan = true;
            Levels[22].PVPEnabled = true;
            Levels[22].DiscordName = "cage-fight";
            Levels[22].LogicType = typeof(GameLogic.PVPLogic);

            Levels[23].Name = "House Fight";
            Levels[23].MapName = "HouseFight";
            Levels[23].Description = "***Spoiler Alarm!***";
            Levels[23].EnemyNumber = -1;
            Levels[23].Mode = GameMode.PvpAhss;
            Levels[23].RespawnMode = RespawnMode.NEVER;
            Levels[23].Supply = true;
            Levels[23].HorsesEnabled = false;
            Levels[23].TeamTitan = true;
            Levels[23].PVPEnabled = true;
            Levels[23].DiscordName = "house-fight";
            Levels[23].LogicType = typeof(GameLogic.PVPLogic);

            Levels[24].Name = "[S]Forest Survive(no crawler no punk)";
            Levels[24].MapName = "The Forest";
            Levels[24].Description = string.Empty;
            Levels[24].EnemyNumber = 3;
            Levels[24].Mode = GameMode.SurviveMode;
            Levels[24].RespawnMode = RespawnMode.NEVER;
            Levels[24].Supply = true;
            Levels[24].NoCrawler = true;
            Levels[24].PunksEnabled = false;
            Levels[24].DiscordName = "forest";
            Levels[24].LogicType = typeof(GameLogic.SurviveLogic);
            Levels[24].HasFog = true;

            Levels[25].Name = "Custom";
            Levels[25].MapName = "The Forest";
            Levels[25].Description = "Custom Map.";
            Levels[25].EnemyNumber = 1;
            Levels[25].Mode = GameMode.KillTitan;
            Levels[25].RespawnMode = RespawnMode.NEVER;
            Levels[25].Supply = true;
            Levels[25].TeamTitan = true;
            Levels[25].PVPEnabled = true;
            Levels[25].PunksEnabled = true;
            Levels[25].DiscordName = "custom";
            Levels[25].LogicType = typeof(GameLogic.KillTitanLogic);
            Levels[25].HasFog = true;

            Levels[26].Name = "Custom (No PT)";
            Levels[26].MapName = "The Forest";
            Levels[26].Description = "Custom Map (No Player Titans).";
            Levels[26].EnemyNumber = 1;
            Levels[26].Mode = GameMode.KillTitan;
            Levels[26].RespawnMode = RespawnMode.NEVER;
            Levels[26].Supply = true;
            Levels[26].TeamTitan = false;
            Levels[26].PVPEnabled = true;
            Levels[26].PunksEnabled = true;
            Levels[26].DiscordName = "custom";
            Levels[16].LogicType = typeof(GameLogic.KillTitanLogic);
            Levels[16].HasFog = true;

            Levels[0].minimapPreset = new Minimap.Preset(new Vector3(22.6f, 0f, 13f), 731.9738f);
            Levels[8].minimapPreset = new Minimap.Preset(new Vector3(8.8f, 0f, 65f), 765.5751f);
            Levels[9].minimapPreset = new Minimap.Preset(new Vector3(8.8f, 0f, 65f), 765.5751f);
            Levels[18].minimapPreset = new Minimap.Preset(new Vector3(443.2f, 0f, 1912.6f), 1929.042f);
            Levels[19].minimapPreset = new Minimap.Preset(new Vector3(443.2f, 0f, 1912.6f), 1929.042f);
            Levels[20].minimapPreset = new Minimap.Preset(new Vector3(2549.4f, 0f, 3042.4f), 3697.16f);
            Levels[21].minimapPreset = new Minimap.Preset(new Vector3(22.6f, 0f, 13f), 734.9738f);

            //Guardian maps (27,28,29)
            Levels[27] = new LevelInfo();//TODO: Multi-Map
            Levels[28] = new LevelInfo
            {
                Name = "The City IV",
                MapName = "The City I",
                Description = "Survive all 20 waves. (No respawns)",
                EnemyNumber = 3,
                Mode = GameMode.SurviveMode,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                DiscordName = "city",
                LogicType = typeof(GameLogic.SurviveLogic)
            };
            Levels[29] = new LevelInfo
            {
                Name = "The City V",
                MapName = "The City I",
                Description = "Survive all 20 waves. (Respawn on each new wave)",
                EnemyNumber = 3,
                Mode = GameMode.SurviveMode,
                RespawnMode = RespawnMode.NEWROUND,
                Supply = true,
                DiscordName = "city",
                LogicType = typeof(GameLogic.SurviveLogic)
            };
            //Anarchy maps (30)
            Levels[30] = new LevelInfo
            {
                Name = "Custom-Anarchy (No PT)",
                MapName = "The Forest",
                Description = "Custom maps with Anarchy extension (No PT)",
                EnemyNumber = 1,
                Mode = GameMode.KillTitan,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                TeamTitan = false,
                PVPEnabled = true,
                PunksEnabled = true,
                DiscordName = "custom",
                LogicType = typeof(GameLogic.KillTitanLogic),
                HasFog = true
            };

            Levels[31] = new LevelInfo
            {
                Name = "Racing - Custom",
                MapName = "The Forest",
                Description = "Custom map dedicated to use for Racing gamemode",
                EnemyNumber = 0,
                Mode = GameMode.Racing,
                RespawnMode = RespawnMode.NEVER,
                Supply = false,
                TeamTitan = false,
                PVPEnabled = false,
                PunksEnabled = false,
                DiscordName = "custom",
                LogicType = typeof(GameLogic.CustomRacingLogic),
                HasFog = true
            }; 
            
            Levels[32] = new LevelInfo
            {
                Name = "Forest - Bomb",
                MapName = "The Forest",
                Description = "Custom map dedicated to use for Racing gamemode",
                EnemyNumber = 0,
                Mode = GameMode.Bomb,
                RespawnMode = RespawnMode.NEVER,
                Supply = true,
                TeamTitan = false,
                PVPEnabled = false,
                PunksEnabled = false,
                DiscordName = "forest",
                LogicType = typeof(GameLogic.CustomRacingLogic),
                HasFog = true
            };

        }
    }
}