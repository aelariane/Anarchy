using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RC.Bombs
{
    public abstract class BombStatsCalculator
    {
        public abstract BombStatSetting CooldownSetting { get; }
        public abstract float MaxTotalStats { get; }
        public abstract BombStatSetting RadiusSetting { get; }
        public abstract BombStatSetting RangeSetting { get; }
        public abstract BombStatSetting SpeedSetting { get; }

        public BombStats ValidateStats(BombStats stats)
        {
            if (CheckValidStats(stats) == false)
            {
                return GetDefaultStats();
            }
            return stats;
        }

        public bool CheckValidStats(BombStats stats)
        {

            bool reset = false;
            float statsCount = 0f;

            statsCount += stats.Radius;
            reset |= stats.Radius < RadiusSetting.MinimumLimit || stats.Radius > RadiusSetting.MaximumLimit;


            statsCount += stats.Radius;
            reset |= stats.Radius < RadiusSetting.MinimumLimit || stats.Radius > RadiusSetting.MaximumLimit;


            statsCount += stats.Radius;
            reset |= stats.Radius < RadiusSetting.MinimumLimit || stats.Radius > RadiusSetting.MaximumLimit;


            statsCount += stats.Radius;
            reset |= stats.Radius < RadiusSetting.MinimumLimit || stats.Radius > RadiusSetting.MaximumLimit;

            reset |= statsCount > MaxTotalStats;

            return !reset;
        }

        public abstract BombStats GetDefaultStats();
    }
}
