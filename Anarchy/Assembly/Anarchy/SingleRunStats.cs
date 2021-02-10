using System;
using System.Text;

namespace Anarchy
{
    internal class SingleRunStats
    {
        private static float lastKillTime = 0f;
        private static float lastRefill = 0;
        private static float lastReload = 0;
        private static HeroStat lastStats;
        private static int refills;
        private static int reloads;

        public float FixedDeltaTime { get; private set; }
        public int GasRefillsCount { get; private set; }
        public int Kills { get; private set; }
        public float KillTimeAverage => (float)Math.Round(TimeStamp / Kills, 2);
        public float LastKillTime { get; private set; }
        public float LastRefill { get; private set; }
        public float LastReload { get; private set; }
        public int MaxDamage { get; private set; }
        public string Name { get; private set; }
        public int Reloads { get; private set; }
        public HeroStat Stats { get; private set; }
        public float TimeStamp { get; private set; }
        public int TotalDamage { get; private set; }
        public float TotalPerKill => (float)Math.Round((float)TotalDamage / (float)Kills, 2);
        public string Version => AnarchyManager.AnarchyVersion.ToString();

        public static void OnKill()
        {
            lastKillTime = FengGameManagerMKII.FGM.logic.RoundTime;
        }

        public static void Refill()
        {
            lastRefill = FengGameManagerMKII.FGM.logic.RoundTime;
            refills++;
        }

        public static void Reload()
        {
            lastReload = FengGameManagerMKII.FGM.logic.RoundTime;
            reloads++;
        }

        public static void Reset()
        {
            refills = 0;
            reloads = 0;
            lastKillTime = 0;
            lastRefill = 0;
            lastReload = 0;
        }

        public static SingleRunStats Generate()
        {
            SingleRunStats result = new SingleRunStats();
            result.Kills = FengGameManagerMKII.FGM.singleKills;
            result.TotalDamage = FengGameManagerMKII.FGM.singleTotal;
            result.MaxDamage = FengGameManagerMKII.FGM.singleMax;
            result.TimeStamp = (float)Math.Round(FengGameManagerMKII.FGM.logic.RoundTime, 4);
            result.FixedDeltaTime = UnityEngine.Time.fixedDeltaTime;
            result.GasRefillsCount = refills;
            result.Reloads = reloads;
            result.LastKillTime = lastKillTime;
            result.LastRefill = lastRefill;
            result.LastReload = lastReload;
            result.Stats = lastStats;
            result.Name = User.Name.Value.ToString().ToHTMLFormat();
            return result;
        }

        public static void SetHERO(HERO hero)
        {
            lastStats = hero.Setup.myCostume.stat;
        }

        public override string ToString()
        {
            var bld = new StringBuilder();
            bld.AppendLine($"Time: {TimeStamp.ToString("F3")}, last kill at {LastKillTime.ToString("F2")}");
            bld.AppendLine($"Name: {Name}");
            bld.AppendLine($"Statistics");
            bld.AppendLine($"Kills: {Kills}. Average kill time: {KillTimeAverage.ToString("F2")}");
            bld.AppendLine($"Total damage: {TotalDamage}. Average total damage: {TotalPerKill.ToString("F2")}");
            bld.AppendLine($"Max damage: {MaxDamage}");

            bld.AppendLine($"Misc statistics");
            bld.AppendLine($"Physics update: {UnityEngine.Mathf.RoundToInt(1f / FixedDeltaTime)}/sec. ({FixedDeltaTime.ToString("F3")} ms)");
            bld.AppendLine($"Refills. Refills count: {GasRefillsCount}. Last refill at: {LastRefill.ToString("F2")}");
            bld.AppendLine($"Reloads. Reloads count: {Reloads}. Last reload at: {LastReload.ToString("F2")}");
            bld.AppendLine($"Hero stats. Name: {Stats.name}");
            bld.AppendLine($"Spd: {Stats.Spd}, Bla: {Stats.Bla}, Acl {Stats.Acl}, Gas: {Stats.Acl}, Skill: {Stats.skillID}");
            bld.AppendLine($"Anarchy version: {Version}");
            return bld.ToString();
        }
    }
}