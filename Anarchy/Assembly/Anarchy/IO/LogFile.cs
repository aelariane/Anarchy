using UnityEngine;

namespace Anarchy.IO
{
    public class LogFile : File
    {
        private static bool created = false;

        public LogFile() : base(Application.dataPath + "/AnarchyLog.log", true, true)
        {
            if (created)
            {
                Debug.LogError($"Created second instance of LogFile.");
                return;
            }
            created = true;
        }

        public override string ReadLine()
        {
            throw new System.Exception($"You are trying to read something from LogFile. This file should be used only for writing!");
        }

        public override void WriteLine(string line)
        {
            base.WriteLine(line);
        }
    }
}
