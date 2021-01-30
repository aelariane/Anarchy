using System.Collections.Generic;

public class HeroStat
{
    private static bool init;
    public static HeroStat ARMIN;
    public static HeroStat EREN;
    public static HeroStat[] heroStats;
    public static HeroStat JEAN;
    public static HeroStat LEVI;
    public static HeroStat MARCO;
    public static HeroStat MIKASA;
    public static HeroStat PETRA;
    public static HeroStat SASHA;
    public static Dictionary<string, HeroStat> stats;
    public int Acl;
    public int Bla;
    public int Gas;
    public string name;

    public string skillID = "petra";
    public int Spd;

    private static void initDATA()
    {
        if (!HeroStat.init)
        {
            HeroStat.init = true;
            HeroStat.MIKASA = new HeroStat();
            HeroStat.LEVI = new HeroStat();
            HeroStat.ARMIN = new HeroStat();
            HeroStat.MARCO = new HeroStat();
            HeroStat.JEAN = new HeroStat();
            HeroStat.EREN = new HeroStat();
            HeroStat.PETRA = new HeroStat();
            HeroStat.SASHA = new HeroStat();
            HeroStat.MIKASA.name = "MIKASA";
            HeroStat.MIKASA.skillID = "mikasa";
            HeroStat.MIKASA.Spd = 125;
            HeroStat.MIKASA.Gas = 75;
            HeroStat.MIKASA.Bla = 75;
            HeroStat.MIKASA.Acl = 135;
            HeroStat.LEVI.name = "LEVI";
            HeroStat.LEVI.skillID = "levi";
            HeroStat.LEVI.Spd = 95;
            HeroStat.LEVI.Gas = 100;
            HeroStat.LEVI.Bla = 100;
            HeroStat.LEVI.Acl = 150;
            HeroStat.ARMIN.name = "ARMIN";
            HeroStat.ARMIN.skillID = "armin";
            HeroStat.ARMIN.Spd = 75;
            HeroStat.ARMIN.Gas = 150;
            HeroStat.ARMIN.Bla = 125;
            HeroStat.ARMIN.Acl = 85;
            HeroStat.MARCO.name = "MARCO";
            HeroStat.MARCO.skillID = "marco";
            HeroStat.MARCO.Spd = 110;
            HeroStat.MARCO.Gas = 100;
            HeroStat.MARCO.Bla = 115;
            HeroStat.MARCO.Acl = 95;
            HeroStat.JEAN.name = "JEAN";
            HeroStat.JEAN.skillID = "jean";
            HeroStat.JEAN.Spd = 100;
            HeroStat.JEAN.Gas = 150;
            HeroStat.JEAN.Bla = 80;
            HeroStat.JEAN.Acl = 100;
            HeroStat.EREN.name = "EREN";
            HeroStat.EREN.skillID = "eren";
            HeroStat.EREN.Spd = 100;
            HeroStat.EREN.Gas = 90;
            HeroStat.EREN.Bla = 90;
            HeroStat.EREN.Acl = 100;
            HeroStat.PETRA.name = "PETRA";
            HeroStat.PETRA.skillID = "petra";
            HeroStat.PETRA.Spd = 80;
            HeroStat.PETRA.Gas = 110;
            HeroStat.PETRA.Bla = 100;
            HeroStat.PETRA.Acl = 140;
            HeroStat.SASHA.name = "SASHA";
            HeroStat.SASHA.skillID = "sasha";
            HeroStat.SASHA.Spd = 140;
            HeroStat.SASHA.Gas = 100;
            HeroStat.SASHA.Bla = 100;
            HeroStat.SASHA.Acl = 115;
            HeroStat heroStat = new HeroStat();
            heroStat.skillID = "petra";
            heroStat.Spd = 100;
            heroStat.Gas = 100;
            heroStat.Bla = 100;
            heroStat.Acl = 100;
            HeroStat heroStat2 = new HeroStat();
            HeroStat.SASHA.name = "AHSS";
            heroStat2.skillID = "sasha";
            heroStat2.Spd = 100;
            heroStat2.Gas = 100;
            heroStat2.Bla = 100;
            heroStat2.Acl = 100;
            HeroStat.stats = new Dictionary<string, HeroStat>();
            HeroStat.stats.Add("MIKASA", HeroStat.MIKASA);
            HeroStat.stats.Add("LEVI", HeroStat.LEVI);
            HeroStat.stats.Add("ARMIN", HeroStat.ARMIN);
            HeroStat.stats.Add("MARCO", HeroStat.MARCO);
            HeroStat.stats.Add("JEAN", HeroStat.JEAN);
            HeroStat.stats.Add("EREN", HeroStat.EREN);
            HeroStat.stats.Add("PETRA", HeroStat.PETRA);
            HeroStat.stats.Add("SASHA", HeroStat.SASHA);
            HeroStat.stats.Add("CUSTOM_DEFAULT", heroStat);
            HeroStat.stats.Add("AHSS", heroStat2);
            return;
        }
    }

    public static HeroStat getInfo(string name)
    {
        HeroStat.initDATA();
        return HeroStat.stats[name];
    }
}