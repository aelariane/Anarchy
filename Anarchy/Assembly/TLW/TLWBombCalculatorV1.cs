using System;
using RC.Bombs;

namespace TLW
{
    public class TLWBombCalculatorV1 : BombStatsCalculator
    {
        private BombStatSetting _coldownSetting = new BombStatSetting(1f, 4f, 10f);
        private BombStatSetting _radSetting = new BombStatSetting(0.5f, 4f, 10f);
        private BombStatSetting _rangeSetting = new BombStatSetting(0.5f, 0f, 3f);
        private BombStatSetting _speedSetting = new BombStatSetting(1f, 0f, 10f);

        public override BombStatSetting CooldownSetting => _coldownSetting;
        public override float MaxTotalStats => 20f;
        public override BombStatSetting RadiusSetting => _radSetting;

        public override BombStatSetting RangeSetting => _rangeSetting;

        public override BombStatSetting SpeedSetting => _speedSetting;

        public TLWBombCalculatorV1()
        {
        }

        public override BombStats GetDefaultStats()
        {
            return new BombStats()
            {
                Radius = 6f,
                Range = 2f,
                Speed = 6,
                Cooldown = 6f
            };
        }
    }
}
