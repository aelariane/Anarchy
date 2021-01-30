namespace Antis.Protections
{
    public class HeroAnimationChecker : IProtection<string>
    {
        internal HeroAnimationChecker()
        {
        }

        bool IProtection.Check(object data)
        {
            string str = data as string;
            if (str != null)
            {
                return Check(str);
            }
            return false;
        }

        public bool Check(string data)
        {
            switch (data)
            {
                case "air_circle":
                case "air_fall":
                case "air_rise":
                case "air2":
                case "air2_backward":
                case "air2_left":
                case "air2_right":
                case "attack1":
                case "attack2":
                case "attack3_1":
                case "attack3_2":
                case "attack4":
                case "attack5":
                case "changeBlade":
                case "dash":
                case "dash_land":
                case "dodge":
                case "grabbed":
                case "grabbed_jean":
                case "horse_getoff":
                case "horse_geton":
                case "horse_idle":
                case "horse_run":
                case "jump":
                case "onWall":
                case "run":
                case "run_levi":
                case "run_sasha":
                case "salute":
                case "slide":
                case "special_marco_0":
                case "special_marco_1":
                case "special_petra":
                case "special_sasha":
                case "special_armin":
                case "stand":
                case "stand_levi":
                case "supply":
                case "T-pose":
                case "toRoof":
                case "wallrun":
                case "AHSS_gun_reload_both":
                case "AHSS_gun_reload_both_air":
                case "AHSS_gun_reload_l":
                case "AHSS_gun_reload_l_air":
                case "AHSS_gun_reload_r":
                case "AHSS_gun_reload_r_air":
                case "AHSS_hook_forward_both":
                case "AHSS_hook_forward_l":
                case "AHSS_hook_forward_r":
                case "AHSS_shoot_both":
                case "AHSS_shoot_both_air":
                case "AHSS_shoot_l":
                case "AHSS_shoot_l_air":
                case "AHSS_shoot_r":
                case "AHSS_shoot_r_air":
                case "AHSS_stand_gun":
                case "air_hook":
                case "air_hook_l":
                case "air_hook_l_just":
                case "air_hook_r":
                case "air_hook_r_just":
                case "air_release":
                case "attack1_hook_l1":
                case "attack1_hook_r1":
                case "attack1_hook_l2":
                case "attack1_hook_r2":
                case "changeBlade_air":
                case "ThrowBlades":
                case "ThrowBladesDown":
                case "ThrowBlades1":
                case "ThrowBlades2":
                case "SlideReload":
                case "Airload":
                    return true;

                default:
                    return false;
            }
        }
    }
}