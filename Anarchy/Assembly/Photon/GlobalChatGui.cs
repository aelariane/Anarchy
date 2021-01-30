//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;

//public class GlobalChatGui : MonoBehaviour
//{
//    private static GlobalChatGui _instance;

//    public static GlobalChatGui Instance
//    {
//        get
//        {
//            if(_instance == null)
//            {
//                _instance = new GameObject(nameof(GlobalChatGui)).AddComponent<GlobalChatGui>();
//                DontDestroyOnLoad(_instance);
//            }
//            return _instance;
//        }
//    }

//    public static KeyCode OpenGlobalChatCode { get; set; }

//    static GlobalChatGui()
//    {
//        OpenGlobalChatCode = KeyCode.F8;
//    }

//    private void OnGUI()
//    {
//    }

//    public void AddMessage()
//    {
//    }
//}