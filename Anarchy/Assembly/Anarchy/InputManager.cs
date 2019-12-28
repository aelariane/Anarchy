using System;
using System.Collections.Generic;
using Anarchy.Configuration;
using UnityEngine;

namespace Anarchy
{
    public class InputManager : MonoBehaviour
    {
        public static List<KeySetting> AllKeys;
        public static readonly KeyCode[] Defaults = new KeyCode[22] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.LeftShift, KeyCode.LeftControl, KeyCode.Q, KeyCode.E, KeyCode.Space, KeyCode.F, KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.N, KeyCode.C, KeyCode.T, KeyCode.P, KeyCode.X, KeyCode.Backspace, KeyCode.R, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };
        public static bool[] IsInput = new bool[22];
        public static bool[] IsInputDown = new bool[22];
        public static bool MenuOn;
        private static Action onAwake;
        public static KeySetting[] Settings;

        public static KeySetting[] RebindKeyCodes = new KeySetting[]
        {
            new KeySetting("ReelIn", KeyCode.Space),new KeySetting("ReelOut", KeyCode.LeftAlt), new KeySetting("GasBurst", KeyCode.Z), new KeySetting("Minimap Max", KeyCode.Tab),
            new KeySetting("Minimap Toggle", KeyCode.M), new KeySetting("Minimap Reset", KeyCode.K), new KeySetting("Open Chat", KeyCode.None), new KeySetting("Live Spectate", KeyCode.Y)
        };

        public static KeySetting[] CannonKeyCodes = new KeySetting[]
        {
            new KeySetting("Cannon Up", KeyCode.W), new KeySetting("Cannon Down", KeyCode.S), new KeySetting("Cannon Left", KeyCode.A), new KeySetting("Cannon Right", KeyCode.D),
            new KeySetting("Cannon Fire", KeyCode.Q), new KeySetting("Cannon Mount", KeyCode.G), new KeySetting("Cannon Slow", KeyCode.LeftShift)
        };

        public static KeySetting[] TitanKeyCodes = new KeySetting[]
        {
            new KeySetting("Titan Forward", KeyCode.W), new KeySetting("Titan Backward", KeyCode.S), new KeySetting("Titan Left", KeyCode.A), new KeySetting("Titan Right", KeyCode.D),
            new KeySetting("Titan Jump", KeyCode.LeftShift) ,new KeySetting("Titan Kill", KeyCode.T), new KeySetting("Titan Walk", KeyCode.LeftAlt), new KeySetting("Titan Punch", KeyCode.Q),
            new KeySetting("Titan Slam", KeyCode.E), new KeySetting("Titan Grab (front)", KeyCode.Alpha1), new KeySetting("Titan Grab (back)", KeyCode.Alpha3), new KeySetting("Titan Grab (nape)", KeyCode.Mouse1),
            new KeySetting("Titan Slap", KeyCode.Mouse0), new KeySetting("Titan Bite", KeyCode.Alpha2), new KeySetting("Titan Cover nape", KeyCode.O),  new KeySetting("Titan Sit", KeyCode.X)
        };

        public static KeySetting[] HorseKeyCodes = new KeySetting[]
        {
            new KeySetting("Horse Forward", KeyCode.W), new KeySetting("Horse Backward", KeyCode.S), new KeySetting("Horse Left", KeyCode.A), new KeySetting("Horse Right", KeyCode.D), new KeySetting("Horse Walk", KeyCode.LeftShift),
            new KeySetting("Horse Jump", KeyCode.Q), new KeySetting("Horse Mount", KeyCode.LeftControl)
        };

        public static InputManager Instance { get; private set; }

        public static void AddToList(KeySetting set)
        {
            if (AllKeys == null)
            {
                if(onAwake == null)
                {
                    onAwake = () => { };
                }
                onAwake += () =>
                {
                    AllKeys.Add(set);
                    Instance.OnAdd();
                };
                return;
            }
            AllKeys.Add(set);
            Instance.OnAdd();
        }

        private void Awake()
        {
            if(Instance != null)
            {
                DestroyImmediate(this);
                Debug.LogError("Attemption to create more then one Anarchy.InputManager, please avoid this");
                return;
            }
            DontDestroyOnLoad(this);
            Instance = this;
            if (AllKeys == null)
                InitList();
            onAwake?.Invoke();
            onAwake = null;
        }

        private void InitList()
        {
            AllKeys = new List<KeySetting>();
            for(int i = 0; i < (int)InputCodes.DefaultsCount; i++)
            {
                new KeySetting(((InputCodes)i).ToString(), Defaults[i]);
            }
            IsInput = new bool[(int)InputCodes.DefaultsCount];
            IsInputDown = new bool[(int)InputCodes.DefaultsCount];
            Settings = AllKeys.ToArray();
        }

        public static bool IsInputRebind(int code)
        {
            return RebindKeyCodes[code].IsPressed();
        }

        public static bool IsInputRebindHolding(int code)
        {
            return RebindKeyCodes[code].IsHolding();
        }

        public static bool IsInputCannon(int code)
        {
            return CannonKeyCodes[code].IsPressed();
        }

        public static bool IsInputCannonHolding(int code)
        {
            return CannonKeyCodes[code].IsHolding();
        }

        public static bool IsInputTitan(int code)
        {
            return TitanKeyCodes[code].IsPressed();
        }

        public static bool IsInputTitanHolding(int code)
        {
            return TitanKeyCodes[code].IsHolding();
        }

        public static bool IsInputHorse(int code)
        {
            return HorseKeyCodes[code].IsPressed();
        }

        public static bool IsInputHorseHolding(int code)
        {
            return HorseKeyCodes[code].IsHolding();
        }

        private void OnAdd()
        {
            
        }

        private void OnLevelWasLoaded(int level)
        {
        }

        private void Update()
        {
            int count = (int)InputCodes.DefaultsCount;
            for(int i = 0; i <  count; i++)
            {
                KeySetting set = Settings[i];
                IsInput[i] = set.IsHolding();
                IsInputDown[i] = set.IsPressed();
            }
        }
    }
}
