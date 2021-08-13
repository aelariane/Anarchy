using System;
using Anarchy.Configuration.Storage;
using Anarchy.Configuration;

namespace RC.Bombs
{
    public class BombSettings
    {
        public static BombStats Load(Type calculatorType)
        {
            IDataStorage storage = Settings.Storage;
            var calculator = Activator.CreateInstance(calculatorType) as BombStatsCalculator;

            string key = calculatorType.Name;

            var stats = new BombStats();
            BombStats defaultStats = calculator.GetDefaultStats();
            stats.Radius = storage.GetFloat(key + "Radius", defaultStats.Radius);
            stats.Range = storage.GetFloat(key + "Range", defaultStats.Range);
            stats.Speed = storage.GetFloat(key + "Speed", defaultStats.Speed);
            stats.Cooldown = storage.GetFloat(key + "Cooldown", defaultStats.Cooldown);

            stats = calculator.ValidateStats(stats);
            WriteToStorage(stats, calculatorType);
            return stats;
        }

        public static void WriteToStorage(BombStats statsToWrite, Type calculatorType)
        {
            IDataStorage storage = Settings.Storage;
            string key = calculatorType.Name;

            storage.SetFloat(key + "Radius", statsToWrite.Radius);
            storage.SetFloat(key + "Range", statsToWrite.Range);
            storage.SetFloat(key + "Speed", statsToWrite.Speed);
            storage.SetFloat(key + "Cooldown", statsToWrite.Cooldown);
        }
    }
}
