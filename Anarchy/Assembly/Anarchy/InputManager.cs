using System;
using System.Collections.Generic;
using Anarchy.Configuration;
using UnityEngine;

namespace Anarchy
{
    public class InputManager : MonoBehaviour
    {
        public static List<KeySetting> AllKeys;

        private static readonly KeyCode[] defaults =
        {
            KeyCode.W,
            KeyCode.S,
            KeyCode.A, 
            KeyCode.D, 
            KeyCode.LeftShift,
            KeyCode.LeftControl, 
            KeyCode.Q,
            KeyCode.E,
            KeyCode.Space,
            KeyCode.F, 
            KeyCode.Mouse0,
            KeyCode.Mouse1,
            KeyCode.N,
            KeyCode.C,
            KeyCode.T,
            KeyCode.P,
            KeyCode.X,
            KeyCode.Backspace,
            KeyCode.R, 
            KeyCode.Alpha1, 
            KeyCode.Alpha2,
            KeyCode.Alpha3
        };
        
        public static bool[] IsInput = new bool[22];
        public static bool[] IsInputDown = new bool[22];
        public static bool MenuOn;
        private static Action onAwake;
        public static KeySetting[] Settings;
        public static IntSetting GasBurstType = new IntSetting(nameof(GasBurstType), 0);

        //TODO: Remove and put property to make field private
        public static readonly KeySetting[] RebindKeyCodes = 
        {
            new KeySetting("ReelIn", KeyCode.Space),
            new KeySetting("ReelOut", KeyCode.LeftAlt), 
            new KeySetting("GasBurst", KeyCode.Z),
            new KeySetting("Minimap Max", KeyCode.Tab),
            new KeySetting("Minimap Toggle", KeyCode.M), 
            new KeySetting("Minimap Reset", KeyCode.K),
            new KeySetting("Open Chat", KeyCode.None),
            new KeySetting("Live Spectate", KeyCode.Y)
        };

        private static readonly KeySetting[] cannonKeyCodes = 
        {
            new KeySetting("Cannon Up", KeyCode.W),
            new KeySetting("Cannon Down", KeyCode.S),
            new KeySetting("Cannon Left", KeyCode.A), 
            new KeySetting("Cannon Right", KeyCode.D),
            new KeySetting("Cannon Fire", KeyCode.Q),
            new KeySetting("Cannon Mount", KeyCode.G),
            new KeySetting("Cannon Slow", KeyCode.LeftShift)
        };

        private static readonly KeySetting[] titanKeyCodes = 
        {
            new KeySetting("Titan Forward", KeyCode.W),
            new KeySetting("Titan Backward", KeyCode.S), 
            new KeySetting("Titan Left", KeyCode.A),
            new KeySetting("Titan Right", KeyCode.D),
            new KeySetting("Titan Jump", KeyCode.LeftShift),
            new KeySetting("Titan Kill", KeyCode.T), 
            new KeySetting("Titan Walk", KeyCode.LeftAlt), 
            new KeySetting("Titan Punch", KeyCode.Q),
            new KeySetting("Titan Slam", KeyCode.E),
            new KeySetting("Titan Grab (front)", KeyCode.Alpha1),
            new KeySetting("Titan Grab (back)", KeyCode.Alpha3), 
            new KeySetting("Titan Grab (nape)", KeyCode.Mouse1),
            new KeySetting("Titan Slap", KeyCode.Mouse0),
            new KeySetting("Titan Bite", KeyCode.Alpha2), 
            new KeySetting("Titan Cover nape",KeyCode.O),
            new KeySetting("Titan Sit", KeyCode.X)
        };

        public static int RebindKeyCodesLength => RebindKeyCodes.Length;

        public static int CannonKeyCodesLength => cannonKeyCodes.Length;

        public static int TitanKeyCodesLength => titanKeyCodes.Length;

        private static readonly KeySetting[] horseKeyCodes = 
        {
            new KeySetting("Horse Forward", KeyCode.W),
            new KeySetting("Horse Backward", KeyCode.S), 
            new KeySetting("Horse Left", KeyCode.A),
            new KeySetting("Horse Right", KeyCode.D),
            new KeySetting("Horse Walk", KeyCode.LeftShift),
            new KeySetting("Horse Jump", KeyCode.Q),
            new KeySetting("Horse Mount", KeyCode.LeftControl)
        };
        
        private static InputManager Instance { get; set; }
        

        public static void AddToList(KeySetting set)
        {
            if (AllKeys == null)
            {
                if (onAwake == null)
                {
                    onAwake = () => { };
                }
                onAwake += () =>
                {
                    AllKeys.Add(set);
                    OnAdd(); //Was basically Instance.OnAdd();
                };
                return;
            }
            AllKeys.Add(set);
            OnAdd();
        }
        
        private static void OnAdd()
        {
            
        }

        private void Awake()
        {
            if (Instance != null)
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

        private static void InitList()
        {
            AllKeys = new List<KeySetting>();
            for (var i = 0; i < (int)InputCodes.DefaultsCount; i++)
            {
                new KeySetting(((InputCodes)i).ToString(), defaults[i]);
            }
            IsInput = new bool[(int)InputCodes.DefaultsCount];
            IsInputDown = new bool[(int)InputCodes.DefaultsCount];
            Settings = AllKeys.ToArray();
        }

        public static bool IsInputRebind(int code)
        {
            return RebindKeyCodes[code].IsKeyDown();
        }

        public static bool IsInputRebindHolding(int code)
        {
            return RebindKeyCodes[code].IsKeyHolding();
        }

        public static bool IsInputCannonHolding(int code)
        {
            return cannonKeyCodes[code].IsKeyHolding();
        }

        public static bool IsInputCannonDown(int code)
        {
            return cannonKeyCodes[code].IsKeyDown();
        }

        public static bool IsInputTitan(int code)
        {
            return titanKeyCodes[code].IsKeyDown();
        }

        public static bool IsInputTitanHolding(int code)
        {
            return titanKeyCodes[code].IsKeyHolding();
        }

        public static bool IsInputHorse(int code)
        {
            return horseKeyCodes[code].IsKeyDown();
        }

        public static bool IsInputHorseHolding(int code)
        {
            return horseKeyCodes[code].IsKeyHolding();
        }

        private void Update()
        {
            const int count = (int)InputCodes.DefaultsCount;
            for (var i = 0; i <  count; i++)
            {
                var set = Settings[i];
                IsInput[i] = set.IsKeyHolding();
                IsInputDown[i] = set.IsKeyDown();
            }
        }
    }
}
