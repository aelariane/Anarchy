using Anarchy.Configuration;

namespace Anarchy
{
    internal class GameFeed
    {
        public static readonly BoolSetting Enabled = new BoolSetting("GameFeedEnabled", false);
        public static readonly BoolSetting ConsoleMode = new BoolSetting("GameFeedConsole", true);

        private readonly Localization.Locale lang;

        public GameFeed()
        {
            lang = new Localization.Locale("GameFeed", true, ',');
            lang.Load();
        }

        private static void AddLine(string line)
        {
            line = GetTimeString() + " <color=#" + User.MainColor + ">" + line + "</color>";
            if (ConsoleMode)
            {
                UI.Log.AddLineRaw(line);
            }
            else
            {
                UI.Chat.Add(line);
            }
        }

        private static string GetTimeString() => $"<color=#{User.MainColor}>(<color=#{User.SubColor}>"
                                                 + FengGameManagerMKII.FGM.logic.RoundTime.ToString("F2")
                                                 + "</color>)</color>";

        public void Kill(string killer, string target, int dmg)
        {
            if (!Enabled)
            {
                return;
            }

            AddLine(lang.Format("killed", killer.ToHTMLFormat(), target.ToHTMLFormat())
                    + (dmg > 0 ? (" " + lang.Format("forDamage", dmg.ToString())) : string.Empty));
        }

        public void RoundLoose()
        {
            if (!Enabled)
            {
                return;
            }

            AddLine(lang.Format("gameLoose", FengGameManagerMKII.FGM.logic.CurrentRound.ToString()));
        }

        public void RoundWin()
        {
            if (!Enabled)
            {
                return;
            }

            AddLine(lang.Format("gameWin", FengGameManagerMKII.FGM.logic.CurrentRound.ToString()));
        }

        public void RoundStart()
        {
            if (!Enabled)
            {
                return;
            }

            AddLine(lang.Format("roundStart", FengGameManagerMKII.FGM.logic.CurrentRound.ToString()));
        }
    }
}