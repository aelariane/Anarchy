using System;

namespace RC.Bombs
{
    public class DefaultBombStatsCalculator : BombStatsCalculator
    {
        private BombStatSetting _setting = new BombStatSetting(1f, 0f, 10f);

        public override BombStatSetting CooldownSetting => _setting;
        public override float MaxTotalStats => 20f;
        public override BombStatSetting RadiusSetting => _setting;
        public override BombStatSetting RangeSetting => _setting;
        public override BombStatSetting SpeedSetting => _setting;

        public DefaultBombStatsCalculator()
        {
        }

        public override BombStats GetDefaultStats()
        {
            return new BombStats()
            {
                Cooldown = 5f,
                Radius = 5f,
                Range = 5f,
                Speed = 5f
            };
        }
    }
}
