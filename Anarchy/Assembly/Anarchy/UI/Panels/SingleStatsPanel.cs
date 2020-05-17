using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.UI
{
    internal class SingleStatsPanel : GUIPanel
    {
        private SmartRect rect;
        private SingleRunStats stats;
        string statString;

        
        //MADE JUST FOR TESTING IDEA. NOT IMPLEMENTED YET
        public SingleStatsPanel() : base(nameof(SingleStatsPanel), GUILayers.SingleStatsPanel)
        {
        }

        protected override void DrawMainPart()
        {
            rect.Reset();
            GUI.Label(rect, $"Time: {stats.TimeStamp.ToString("F4")}, last kill at {stats.LastKillTime.ToString("F3")}", true);
            GUI.Label(rect, $"Name: {stats.Name}", true);
            GUI.Label(rect, $"Statistics", true);
            GUI.Label(rect, $"Kills: {stats.Kills}. Time for 1 kill: {stats.KillTimeAverage.ToString("F3")}", true);
            GUI.Label(rect, $"Total damage: {stats.Total_Dmg}. Average total: {stats.TotalPerKill.ToString("F3")}", true);
            GUI.Label(rect, $"Max danmage: {stats.Max_Dmg}", true);

            GUI.Label(rect, $"Misc statistics", true);
            GUI.Label(rect, $"Physics update: {UnityEngine.Mathf.RoundToInt(1f / stats.FixedDeltaTime)}/sec. ({stats.FixedDeltaTime.ToString("F4")} ms)", true);
            GUI.Label(rect, $"Refills. Refills count: {stats.GasRefillsCount}. Last refill at: {stats.LastRefill.ToString("F3")}", true);
            GUI.Label(rect, $"Reloads. Reloads count: {stats.Reloads}. Last reload at: {stats.LastReload.ToString("F3")}", true);
            GUI.Label(rect, $"Hero stats. Name: {stats.Stats.name}", true);
            GUI.Label(rect, $"Spd: {stats.Stats.Spd}, Bla: {stats.Stats.Bla}, Acl {stats.Stats.Acl}, Gas: {stats.Stats.Acl}, Skill: {stats.Stats.skillID}", true);
            GUI.Label(rect, $"Anarchy version: {stats.Version}");
        }

        protected override void OnPanelDisable()
        {
            UnityEngine.Time.timeScale = 1f;
        }

        protected override void OnPanelEnable()
        {
            rect = Helper.GetSmartRects(BoxPosition, 1)[0];
            stats = SingleRunStats.Generate();
            statString = stats.ToString();
            UnityEngine.Time.timeScale = 0f;
        }

        public override void Update()
        {
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Escape))
            {
                Disable();
                return;
            }
        }
    }
}
