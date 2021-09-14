using Anarchy.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Anarchy
{
    public class InputManager : MonoBehaviour
    {
        //Default AoTTG layot
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

        private float restartTimer = 0f;
        private float pauseTimer = 0f;

        #region Rebinds

        private static readonly KeySetting[] rebindKeyCodes =
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

        #endregion Rebinds

        #region Cannons

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

        #endregion Cannons

        #region Titans

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
            new KeySetting("Titan Cover nape", KeyCode.O),
            new KeySetting("Titan Sit", KeyCode.X),
            new KeySetting("Titan Stomp", KeyCode.None),
            new KeySetting("Titan Kick", KeyCode.None),
            new KeySetting("Titan FaceSlap", KeyCode.None),
            new KeySetting("Titan NeckSlap", KeyCode.None),
            new KeySetting("Titan LeftSlap", KeyCode.None),
            new KeySetting("Titan RightSlap", KeyCode.None),
            new KeySetting("Titan SlapLow", KeyCode.None),
            new KeySetting("Titan LeftSlapLow", KeyCode.None),
            new KeySetting("Titan RightSlapLow", KeyCode.None),
            new KeySetting("Titan LeftBite", KeyCode.None),
            new KeySetting("Titan RightBite", KeyCode.None)
        };

        public static BoolSetting InvertTitanBiteInput = new BoolSetting(nameof(InvertTitanBiteInput), true);
        public static BoolSetting InvertTitanSlapInput = new BoolSetting(nameof(InvertTitanSlapInput), true);
        public static BoolSetting DisableDirectionalBite = new BoolSetting(nameof(DisableDirectionalBite), false);
        public static BoolSetting DisableDirectionalSlap = new BoolSetting(nameof(DisableDirectionalSlap), false);
        #endregion Titans

        #region Horse

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

        #endregion Horse

        #region Anarchy

        //Special Anarchy-specified keycodes
        private static readonly KeySetting[] anarchyKeyCodes =
        {
            new KeySetting("RestartHotkey", KeyCode.F1), //Restarts game on press
            new KeySetting("PauseHotkey", KeyCode.F2), //Pause/Unpause game
            new KeySetting("DebugPanel", KeyCode.None), //Enables/Disables debug panel
            new KeySetting("ChatHistory", KeyCode.F8), //Chat history panel
            new KeySetting("SingleStats", KeyCode.Tab), //Singleplayer stats panel
            new KeySetting("Rejoin", KeyCode.F12) //Fast rejoin
        };

        #endregion Anarchy

        private static InputManager instance;
        private static Action onAwake;

        public static bool[] IsInput = new bool[22];
        public static bool[] IsInputDown = new bool[22];
        public static KeySetting[] Settings;
        public static IntSetting GasBurstType = new IntSetting(nameof(GasBurstType), 0);
        public static BoolSetting LegacyGasRebind = new BoolSetting(nameof(LegacyGasRebind), false);
        public static BoolSetting DisableMouseReeling = new BoolSetting(nameof(DisableMouseReeling), false);
        public static BoolSetting DisableBurstCooldown = new BoolSetting(nameof(DisableBurstCooldown), false);

        public static List<KeySetting> AllKeys { get; private set; }
        public static int AnarchyKeyCodesLength => anarchyKeyCodes.Length;
        public static int CannonKeyCodesLength => cannonKeyCodes.Length;
        public static int HorseKeyCodesLength => horseKeyCodes.Length;
        public static bool MenuOn { get; set; }
        public static int RebindKeyCodesLength => rebindKeyCodes.Length;
        public static int TitanKeyCodesLength => titanKeyCodes.Length;

        //Sadly this is needed specifically for this one keybind
        public static KeyCode OpenChatCode => rebindKeyCodes[(int)Inputs.InputRebinds.OpenChat].Value;

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
            if (instance != null)
            {
                DestroyImmediate(this);
                Debug.LogError("Attemption to create more then one Anarchy.InputManager, please avoid this");
                return;
            }
            DontDestroyOnLoad(this);
            instance = this;
            if (AllKeys == null)
            {
                InitList();
            }

            onAwake?.Invoke();
            onAwake = null;
        }

        //Initializes basic aottg layout, using defaults first time
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

        public static bool IsInputAnarchy(int code)
        {
            return anarchyKeyCodes[code].IsKeyDown();
        }

        public static bool IsInputAnarchyHolding(int code)
        {
            return anarchyKeyCodes[code].IsKeyHolding();
        }

        public static bool IsInputRebind(int code)
        {
            return rebindKeyCodes[code].IsKeyDown();
        }

        public static bool IsInputRebindHolding(int code)
        {
            return rebindKeyCodes[code].IsKeyHolding();
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
            for (var i = 0; i < count; i++)
            {
                var set = Settings[i];
                IsInput[i] = set.IsKeyHolding();
                IsInputDown[i] = set.IsKeyDown();
            }

            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer && PhotonNetwork.IsMasterClient)
            {
                float delta = Time.deltaTime;
                if (AnarchyManager.PauseWindow.IsActive)
                {
                    delta *= 100000f;
                }
                if (anarchyKeyCodes[(int)Inputs.InputAnarchy.Restart].IsKeyDown())
                {
                    if (restartTimer == 0f)
                    {
                        FengGameManagerMKII.FGM.RestartGame(false, true);
                        restartTimer += 0.001f;
                    }
                }
                if (anarchyKeyCodes[(int)Inputs.InputAnarchy.Pause].IsKeyDown())
                {
                    if (pauseTimer == 0f)
                    {
                        bool val = AnarchyManager.PauseWindow.IsActive;
                        Commands.ICommand cmd = new Commands.Chat.PauseCommand(!val);
                        cmd.Execute(new string[0]);
                        pauseTimer += 0.001f;
                    }
                }
                if (restartTimer > 0f)
                {
                    restartTimer += delta;
                    if (restartTimer > 2f)
                    {
                        restartTimer = 0f;
                    }
                }
                if (pauseTimer > 0f)
                {
                    pauseTimer += delta;
                    if (pauseTimer > 4f)
                    {
                        pauseTimer = 0f;
                    }
                }
            }
        }
    }
}