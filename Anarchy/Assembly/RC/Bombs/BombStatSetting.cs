namespace RC.Bombs
{
    public class BombStatSetting
    {
        public float MaximumLimit { get; }
        public float MinimumLimit { get; }
        public float Step { get; }

        public BombStatSetting(float step, float min, float max)
        {
            Step = step;
            MinimumLimit = min;
            MaximumLimit = max;
        }
    }
}
