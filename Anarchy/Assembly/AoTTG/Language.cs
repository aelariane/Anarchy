using UnityEngine;

public class Language
{
    public static string[] abnormal = new string[25];
    public static string[] btn_back = new string[25];
    public static string[] btn_continue = new string[25];
    public static string[] btn_create_game = new string[25];
    public static string[] btn_credits = new string[25];
    public static string[] btn_default = new string[25];
    public static string[] btn_join = new string[25];
    public static string[] btn_LAN = new string[25];
    public static string[] btn_multiplayer = new string[25];
    public static string[] btn_option = new string[25];
    public static string[] btn_QUICK_MATCH = new string[25];
    public static string[] btn_quit = new string[25];
    public static string[] btn_ready = new string[25];
    public static string[] btn_refresh = new string[25];
    public static string[] btn_server_ASIA = new string[25];
    public static string[] btn_server_EU = new string[25];
    public static string[] btn_server_JAPAN = new string[25];
    public static string[] btn_server_US = new string[25];
    public static string[] btn_single = new string[25];
    public static string[] btn_start = new string[25];
    public static string[] camera_info = new string[25];
    public static string[] camera_original = new string[25];
    public static string[] camera_tilt = new string[25];
    public static string[] camera_tps = new string[25];
    public static string[] camera_type = new string[25];
    public static string[] camera_wow = new string[25];
    public static string[] change_quality = new string[25];
    public static string[] choose_character = new string[25];
    public static string[] choose_map = new string[25];
    public static string[] choose_region_server = new string[25];
    public static string[] difficulty = new string[25];
    public static string[] game_time = new string[25];
    public static string[] hard = new string[25];
    public static string[] invert_mouse = new string[25];
    public static string[] key_set_info_1 = new string[25];
    public static string[] key_set_info_2 = new string[25];
    public static string[] max_player = new string[25];
    public static string[] max_Time = new string[25];
    public static string[] mouse_sensitivity = new string[25];
    public static string[] normal = new string[25];
    public static string[] port = new string[25];
    public static string[] select_titan = new string[25];
    public static string[] server_ip = new string[25];
    public static string[] server_name = new string[25];
    public static string[] soldier = new string[25];
    public static string[] titan = new string[25];
    public static int type = -1;
    public static string[] waiting_for_input = new string[25];

    public static string GetLang(int id)
    {
        if (id == 0)
        {
            return "ENGLISH";
        }
        if (id == 1)
        {
            return "简体中文";
        }
        if (id == 2)
        {
            return "SPANISH";
        }
        if (id == 3)
        {
            return "POLSKI";
        }
        if (id == 4)
        {
            return "ITALIANO";
        }
        if (id == 5)
        {
            return "NORWEGIAN";
        }
        if (id == 6)
        {
            return "PORTUGUESE";
        }
        if (id == 7)
        {
            return "PORTUGUESE_BR";
        }
        if (id == 8)
        {
            return "繁體中文_台";
        }
        if (id == 9)
        {
            return "繁體中文_港";
        }
        if (id == 10)
        {
            return "SLOVAK";
        }
        if (id == 11)
        {
            return "GERMAN";
        }
        if (id == 12)
        {
            return "FRANCAIS";
        }
        if (id == 13)
        {
            return "TÜRKÇE";
        }
        if (id == 14)
        {
            return "ARABIC";
        }
        if (id == 15)
        {
            return "Thai";
        }
        if (id == 16)
        {
            return "Русский";
        }
        if (id == 17)
        {
            return "NEDERLANDS";
        }
        if (id == 18)
        {
            return "Hebrew";
        }
        if (id == 19)
        {
            return "DANSK";
        }
        return "ENGLISH";
    }

    public static int GetLangIndex(string txt)
    {
        if (txt == "ENGLISH")
        {
            return 0;
        }
        if (txt == "SPANISH")
        {
            return 2;
        }
        if (txt == "POLSKI")
        {
            return 3;
        }
        if (txt == "ITALIANO")
        {
            return 4;
        }
        if (txt == "NORWEGIAN")
        {
            return 5;
        }
        if (txt == "PORTUGUESE")
        {
            return 6;
        }
        if (txt == "PORTUGUESE_BR")
        {
            return 7;
        }
        if (txt == "SLOVAK")
        {
            return 10;
        }
        if (txt == "GERMAN")
        {
            return 11;
        }
        if (txt == "FRANCAIS")
        {
            return 12;
        }
        if (txt == "TÜRKÇE")
        {
            return 13;
        }
        if (txt == "ARABIC")
        {
            return 14;
        }
        if (txt == "Thai")
        {
            return 15;
        }
        if (txt == "Русский")
        {
            return 16;
        }
        if (txt == "NEDERLANDS")
        {
            return 17;
        }
        if (txt == "Hebrew")
        {
            return 18;
        }
        if (txt == "DANSK")
        {
            return 19;
        }
        if (txt == "简体中文")
        {
            return 1;
        }
        if (txt == "繁體中文_台")
        {
            return 8;
        }
        if (txt == "繁體中文_港")
        {
            return 9;
        }
        return 0;
    }

    public static void init()
    {
        string text = ((TextAsset)Resources.Load("lang")).text;
        string[] array = text.Split(new char[]
        {
            "\n"[0]
        });
        string text2 = string.Empty;
        int num = 0;
        string a = string.Empty;
        string text3 = string.Empty;
        foreach (string text4 in array)
        {
            if (!text4.Contains("//"))
            {
                if (text4.Contains("#START"))
                {
                    text2 = text4.Split(new char[]
                    {
                        "@"[0]
                    })[1];
                    num = Language.GetLangIndex(text2);
                }
                else if (text4.Contains("#END"))
                {
                    text2 = string.Empty;
                }
                else if (text2 != string.Empty && text4.Contains("@"))
                {
                    a = text4.Split(new char[]
                    {
                        "@"[0]
                    })[0];
                    text3 = text4.Split(new char[]
                    {
                        "@"[0]
                    })[1];
                    if (a == "btn_single")
                    {
                        Language.btn_single[num] = text3;
                    }
                    else if (a == "btn_multiplayer")
                    {
                        Language.btn_multiplayer[num] = text3;
                    }
                    else if (a == "btn_option")
                    {
                        Language.btn_option[num] = text3;
                    }
                    else if (a == "btn_credits")
                    {
                        Language.btn_credits[num] = text3;
                    }
                    else if (a == "btn_back")
                    {
                        Language.btn_back[num] = text3;
                    }
                    else if (a == "btn_refresh")
                    {
                        Language.btn_refresh[num] = text3;
                    }
                    else if (a == "btn_join")
                    {
                        Language.btn_join[num] = text3;
                    }
                    else if (a == "btn_start")
                    {
                        Language.btn_start[num] = text3;
                    }
                    else if (a == "btn_create_game")
                    {
                        Language.btn_create_game[num] = text3;
                    }
                    else if (a == "btn_LAN")
                    {
                        Language.btn_LAN[num] = text3;
                    }
                    else if (a == "btn_server_US")
                    {
                        Language.btn_server_US[num] = text3;
                    }
                    else if (a == "btn_server_EU")
                    {
                        Language.btn_server_EU[num] = text3;
                    }
                    else if (a == "btn_server_ASIA")
                    {
                        Language.btn_server_ASIA[num] = text3;
                    }
                    else if (a == "btn_server_JAPAN")
                    {
                        Language.btn_server_JAPAN[num] = text3;
                    }
                    else if (a == "btn_QUICK_MATCH")
                    {
                        Language.btn_QUICK_MATCH[num] = text3;
                    }
                    else if (a == "btn_default")
                    {
                        Language.btn_default[num] = text3;
                    }
                    else if (a == "btn_ready")
                    {
                        Language.btn_ready[num] = text3;
                    }
                    else if (a == "server_name")
                    {
                        Language.server_name[num] = text3;
                    }
                    else if (a == "server_ip")
                    {
                        Language.server_ip[num] = text3;
                    }
                    else if (a == "port")
                    {
                        Language.port[num] = text3;
                    }
                    else if (a == "choose_map")
                    {
                        Language.choose_map[num] = text3;
                    }
                    else if (a == "choose_character")
                    {
                        Language.choose_character[num] = text3;
                    }
                    else if (a == "camera_type")
                    {
                        Language.camera_type[num] = text3;
                    }
                    else if (a == "camera_original")
                    {
                        Language.camera_original[num] = text3;
                    }
                    else if (a == "camera_wow")
                    {
                        Language.camera_wow[num] = text3;
                    }
                    else if (a == "camera_tps")
                    {
                        Language.camera_tps[num] = text3;
                    }
                    else if (a == "max_player")
                    {
                        Language.max_player[num] = text3;
                    }
                    else if (a == "max_Time")
                    {
                        Language.max_Time[num] = text3;
                    }
                    else if (a == "game_time")
                    {
                        Language.game_time[num] = text3;
                    }
                    else if (a == "difficulty")
                    {
                        Language.difficulty[num] = text3;
                    }
                    else if (a == "normal")
                    {
                        Language.normal[num] = text3;
                    }
                    else if (a == "hard")
                    {
                        Language.hard[num] = text3;
                    }
                    else if (a == "abnormal")
                    {
                        Language.abnormal[num] = text3;
                    }
                    else if (a == "mouse_sensitivity")
                    {
                        Language.mouse_sensitivity[num] = text3;
                    }
                    else if (a == "change_quality")
                    {
                        Language.change_quality[num] = text3;
                    }
                    else if (a == "camera_tilt")
                    {
                        Language.camera_tilt[num] = text3;
                    }
                    else if (a == "invert_mouse")
                    {
                        Language.invert_mouse[num] = text3;
                    }
                    else if (a == "waiting_for_input")
                    {
                        Language.waiting_for_input[num] = text3;
                    }
                    else if (a == "key_set_info_1")
                    {
                        Language.key_set_info_1[num] = text3;
                    }
                    else if (a == "key_set_info_2")
                    {
                        Language.key_set_info_2[num] = text3;
                    }
                    else if (a == "soldier")
                    {
                        Language.soldier[num] = text3;
                    }
                    else if (a == "titan")
                    {
                        Language.titan[num] = text3;
                    }
                    else if (a == "select_titan")
                    {
                        Language.select_titan[num] = text3;
                    }
                    else if (a == "camera_info")
                    {
                        Language.camera_info[num] = text3;
                    }
                    else if (a == "btn_continue")
                    {
                        Language.btn_continue[num] = text3;
                    }
                    else if (a == "btn_quit")
                    {
                        Language.btn_quit[num] = text3;
                    }
                    else if (a == "choose_region_server")
                    {
                        Language.choose_region_server[num] = text3;
                    }
                }
            }
        }
    }
}