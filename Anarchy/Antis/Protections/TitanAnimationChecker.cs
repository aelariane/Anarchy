namespace Antis.Protections
{
    public class TitanAnimationChecker : IProtection<string>
    {
        internal TitanAnimationChecker()
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
                case "attack_abnormal_getup":
                case "attack_abnormal_jump":
                case "attack_anti_AE_l":
                case "attack_anti_AE_low_l":
                case "attack_anti_AE_low_r":
                case "attack_anti_AE_r":
                case "attack_combo_1":
                case "attack_crawler_jump_0":
                case "attack_crawler_jump_1":
                case "attack_crawler_jump_2":
                case "attack_front_ground":
                case "attack_jumper_0":
                case "attack_jumper_1":
                case "attack_jumper_2":
                case "attack_kick":
                case "attack_slap_back":
                case "attack_slap_face":
                case "attack_stomp":
                case "attack_throw":
                case "attack_quick_turn_l":
                case "attack_quick_turn_r":
                case "attack_bite":
                case "attack_bite_l":
                case "attack_bite_r":
                case "crawler_die":
                case "crawler_idle":
                case "crawler_run":
                case "crawler_turnaround_L":
                case "crawler_turnaround_R":
                case "die_back":
                case "die_blow":
                case "die_front":
                case "die_ground":
                case "die_headOff":
                case "eat_l":
                case "eat_r":
                case "grab_ground_back_l":
                case "grab_ground_back_r":
                case "grab_ground_front_r":
                case "grab_ground_front_l":
                case "grab_head_back_l":
                case "grab_head_back_r":
                case "grab_head_front_l":
                case "grab_head_front_r":
                case "hit_eren_L":
                case "hit_eren_R":
                case "hit_eye":
                case "idle":
                case "idle_recovery":
                case "laugh":
                case "run_abnormal":
                case "run_abnormal_1":
                case "run_walk":
                case "sit_die":
                case "sit_down":
                case "sit_getup":
                case "sit_hit_eye":
                case "sit_hunt_down":
                case "sit_idle":
                case "T-pose":
                case "tired":
                case "turnaround1":
                case "turnaround2":
                case "attack_combo_2":
                case "attack_combo_3":
                    return true;

                default:
                    return false;
            }
        }
    }
}