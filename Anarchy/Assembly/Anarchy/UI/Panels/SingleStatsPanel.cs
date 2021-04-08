namespace Anarchy.UI
{
    internal class SingleStatsPanel : GUIPanel
    {
        private SmartRect rect;
        private SingleRunStats stats;
        private string statString;

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
            GUI.Label(rect, $"Total damage: {stats.TotalDamage}. Average total: {stats.TotalPerKill.ToString("F3")}", true);
            GUI.Label(rect, $"Max danmage: {stats.MaxDamage}", true);

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
            UnityEngine.Screen.lockCursor = IN_GAME_MAIN_CAMERA.CameraMode >= CameraType.TPS;
            UnityEngine.Screen.showCursor = false;
            IN_GAME_MAIN_CAMERA.isPausing = false;
            InputManager.MenuOn = false;
            if (IN_GAME_MAIN_CAMERA.MainCamera != null && !IN_GAME_MAIN_CAMERA.MainCamera.enabled)
            {
                IN_GAME_MAIN_CAMERA.SpecMov.disable = false;
                IN_GAME_MAIN_CAMERA.Look.disable = false;
            }
            UnityEngine.Time.timeScale = 1f;
        }

        protected override void OnPanelEnable()
        {
            rect = Helper.GetSmartRects(WindowPosition, 1)[0];
            stats = SingleRunStats.Generate();
            statString = stats.ToString();
            UnityEngine.Time.timeScale = 0f;
            IN_GAME_MAIN_CAMERA.isPausing = true;
        }

        public override void Update()
        {
            if(UnityEngine.Input.GetKey(UnityEngine.KeyCode.Tab) == false)
            {
                DisableImmediate();
            }
        }
    }
}