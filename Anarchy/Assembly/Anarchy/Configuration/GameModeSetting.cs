using Anarchy.Configuration.Storage;
using Anarchy.Localization;
using Anarchy.UI;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using static Anarchy.UI.GUI;

namespace Anarchy.Configuration
{
    public class GameModeSetting : ISetting
    {
        private const char Separator = ',';

        public delegate void ChangedCallback(GameModeSetting sender, bool state, bool received);

        public delegate void ApplyCallback(GameModeSetting sender, bool state, int selection, float[] floats, int[] integers);

        private delegate void GameModeSettingDraw(SmartRect rect, Locale locale);

        private static readonly Locale english;
        internal static readonly Locale Lang;

        private float[] appliedFloats = null;
        private int[] appliedIntegers = null;
        private int appliedSelection = -1;
        private bool appliedState;
        private readonly GameModeSettingDraw draw;
        private string[] floatStrings = null;
        private bool forcedChange;
        public readonly string[] HashKeys;
        private string[] integerStrings = null;
        private readonly string key;
        private readonly float[] nextFloats = null;
        private readonly int[] nextIntegers = null;
        private int nextSelection = -1;
        private ApplyCallback onApplyCallback = delegate (GameModeSetting set, bool state, int sel, float[] floats, int[] ints) { };
        private ChangedCallback onStateChanged = (set, val, received) => { };
        private readonly string saveKey;
        private bool state;

        public bool Enabled
        {
            get
            {
                return appliedState && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer;
            }
        }

        public bool HasChanged
        {
            get
            {
                if (state == appliedState && appliedState == false)
                {
                    return false;
                }

                if (forcedChange || state != appliedState)
                {
                    return true;
                }

                if (nextSelection >= 0)
                {
                    if (appliedSelection != nextSelection + 1)
                    {
                        return true;
                    }
                }
                if (nextFloats != null)
                {
                    for (int i = 0; i < nextFloats.Length; i++)
                    {
                        if (float.TryParse(floatStrings[i], out float val))
                        {
                            if (!appliedFloats[i].Equals(val))
                            {
                                return true;
                            }
                        }
                    }
                }
                if (nextIntegers != null)
                {
                    for (int i = 0; i < nextIntegers.Length; i++)
                    {
                        if (int.TryParse(integerStrings[i], out int val))
                        {
                            if (!appliedIntegers[i].Equals(val))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        public bool HasChangedReceived
        {
            get
            {
                if (state == appliedState && appliedState == false)
                {
                    return false;
                }

                if (forcedChange || state != appliedState)
                {
                    return true;
                }

                if (nextSelection >= 0)
                {
                    if (appliedSelection != nextSelection + 1)
                    {
                        return true;
                    }
                }
                if (nextFloats != null)
                {
                    for (int i = 0; i < nextFloats.Length; i++)
                    {
                        if (!appliedFloats[i].Equals(nextFloats[i]))
                        {
                            return true;
                        }
                    }
                }
                if (nextIntegers != null)
                {
                    for (int i = 0; i < nextIntegers.Length; i++)
                    {
                        if (!appliedIntegers[i].Equals(nextIntegers[i]))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public int Selection
        {
            get
            {
                if (appliedSelection <= 0)
                {
                    return -1;
                }

                return appliedSelection;
            }
        }

        public bool State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        public GameModeSetting(string key) : this(key, -1, null, null)
        {
        }

        public GameModeSetting(string key, int sel) : this(key, sel, null, null)
        {
        }

        public GameModeSetting(string key, float[] floats) : this(key, -1, floats, null)
        {
        }

        public GameModeSetting(string key, int[] ints) : this(key, -1, null, ints)
        {
        }

        public GameModeSetting(string key, int sel, float[] floats) : this(key, sel, floats, null)
        {
        }

        public GameModeSetting(string key, int sel, int[] ints) : this(key, sel, null, ints)
        {
        }

        public GameModeSetting(string key, int selection, float[] floats, int[] ints)
        {
            if (Settings.Storage == null)
            {
                Settings.CreateStorage();
            }

            Settings.AddSetting(this);
            GameModes.AddSetting(this);
            string[] keys = key.Split(Separator);
            HashKeys = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                HashKeys[i] = keys[i];
            }
            this.key = keys[0];
            saveKey = "GameMode/" + keys[0];
            this.nextSelection = selection;
            this.nextFloats = floats;
            nextIntegers = ints;

            draw = (rect, loc) => { };
            if (selection >= 0)
            {
                draw += DrawSelection;
            }
            if (floats != null)
            {
                draw += DrawFloats;
            }
            if (ints != null)
            {
                draw += DrawIntegers;
            }
            Load();
            Apply();
            AddChangedCallback(RCSettingCallback);
        }

        static GameModeSetting()
        {
            english = new Locale(Localization.Language.DefaultLanguage, "GameModes", true, ',');
            english.Reload();
            Lang = new Locale("GameModes", true);
            Lang.Reload();
        }

        public GameModeSetting AddApplyCallback(ApplyCallback callback)
        {
            if (onApplyCallback == null)
            {
                onApplyCallback = callback;
                return this;
            }
            onApplyCallback += callback;
            return this;
        }

        public GameModeSetting AddChangedCallback(ChangedCallback callback)
        {
            if (onStateChanged == null)
            {
                onStateChanged = callback;
                return this;
            }
            onStateChanged += callback;
            return this;
        }

        public static void RCSettingCallback(GameModeSetting set, bool state, bool received)
        {
            if (PhotonNetwork.IsMasterClient && !received)
            {
                PhotonPlayer[] targets = PhotonPlayer.GetVanillaUsers();
                if (targets.Length <= 0)
                {
                    return;
                }
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", targets, new object[] { set.ToString(), string.Empty });
            }
        }

        public void Apply()
        {
            forcedChange = false;
            if (nextFloats != null)
            {
                for (int i = 0; i < nextFloats.Length; i++)
                {
                    if (float.TryParse(floatStrings[i], out float val))
                    {
                        nextFloats[i] = val;
                    }
                }
            }
            if (nextIntegers != null)
            {
                for (int i = 0; i < nextIntegers.Length; i++)
                {
                    if (int.TryParse(integerStrings[i], out int val))
                    {
                        nextIntegers[i] = val;
                    }
                }
            }
            onApplyCallback(this, state, nextSelection, nextFloats, nextIntegers);
            appliedState = state;
            if (nextSelection >= 0)
            {
                appliedSelection = nextSelection + 1;
            }
            if (nextFloats != null)
            {
                for (int i = 0; i < nextFloats.Length; i++)
                {
                    appliedFloats[i] = nextFloats[i];
                    floatStrings[i] = nextFloats[i].ToString("F2");
                }
            }
            if (nextIntegers != null)
            {
                for (int i = 0; i < nextIntegers.Length; i++)
                {
                    appliedIntegers[i] = nextIntegers[i];
                    integerStrings[i] = nextIntegers[i].ToString();
                }
            }
            onStateChanged(this, state, false);
        }

        public void ApplyReceived()
        {
            forcedChange = false;
            appliedState = state;
            if (nextSelection >= 0)
            {
                appliedSelection = nextSelection + 1;
            }
            if (nextFloats != null)
            {
                for (int i = 0; i < nextFloats.Length; i++)
                {
                    appliedFloats[i] = nextFloats[i];
                }
            }
            if (nextIntegers != null)
            {
                for (int i = 0; i < nextIntegers.Length; i++)
                {
                    appliedIntegers[i] = nextIntegers[i];
                }
            }
            onStateChanged(this, state, true);
        }

        public void Draw(SmartRect rect, Locale locale)
        {
            state = ToggleButton(rect, state, locale[key + "State"], true);
            draw(rect, locale);
        }

        private void DrawFloats(SmartRect rect, Locale loc)
        {
            for (int i = 0; i < floatStrings.Length; i++)
            {
                floatStrings[i] = TextField(rect, floatStrings[i], Style.LabelSpace + loc[key + "Float" + i.ToString()], Style.BigLabelOffset, true);
            }
        }

        private void DrawIntegers(SmartRect rect, Locale loc)
        {
            for (int i = 0; i < integerStrings.Length; i++)
            {
                integerStrings[i] = TextField(rect, integerStrings[i], Style.LabelSpace + loc[key + "Int" + i.ToString()], Style.BigLabelOffset, true);
            }
        }

        private void DrawSelection(SmartRect rect, Locale loc)
        {
            string[] labels = loc.GetArray(key + "Selection");
            nextSelection = SelectionGrid(rect, nextSelection, labels, labels.Length, true);
        }

        public void ForceChange()
        {
            forcedChange = true;
        }

        public void ForceDisable()
        {
            state = false;
            appliedState = false;
        }

        public float GetFloat(int index)
        {
            if (appliedFloats == null || appliedFloats.Length <= index)
            {
                return 0f;
            }

            return appliedFloats[index];
        }

        public int GetInt(int index)
        {
            if (appliedIntegers == null || appliedIntegers.Length <= index)
            {
                return 0;
            }

            return appliedIntegers[index];
        }

        public void Load()
        {
            IDataStorage storage = Settings.Storage;
            state = storage.GetBool(saveKey + "State", false);
            if (nextSelection >= 0)
            {
                nextSelection = storage.GetInt(saveKey + "Selection", nextSelection);
            }
            if (nextFloats != null)
            {
                floatStrings = new string[nextFloats.Length];
                appliedFloats = new float[nextFloats.Length];
                for (int i = 0; i < nextFloats.Length; i++)
                {
                    floatStrings[i] = storage.GetFloat(saveKey + "Float" + i.ToString(), nextFloats[i]).ToString();
                    if (!float.TryParse(floatStrings[i], out float val))
                    {
                        floatStrings[i] = nextFloats[i].ToString("F2");
                    }
                    else
                    {
                        nextFloats[i] = val;
                    }
                    appliedFloats[i] = nextFloats[i];
                }
            }
            if (nextIntegers != null)
            {
                integerStrings = new string[nextIntegers.Length];
                appliedIntegers = new int[nextIntegers.Length];
                for (int i = 0; i < nextIntegers.Length; i++)
                {
                    integerStrings[i] = storage.GetInt(saveKey + "Int" + i.ToString(), nextIntegers[i]).ToString();
                    if (!int.TryParse(integerStrings[i], out int val))
                    {
                        integerStrings[i] = nextIntegers[i].ToString();
                    }
                    else
                    {
                        nextIntegers[i] = val;
                    }
                    appliedIntegers[i] = nextIntegers[i];
                }
            }
        }

        public void Save()
        {
            IDataStorage storage = Settings.Storage;
            storage.SetBool(saveKey + "State", state);
            if (nextSelection >= 0)
            {
                storage.SetInt(saveKey + "Selection", nextSelection);
            }
            if (nextFloats != null)
            {
                for (int i = 0; i < nextFloats.Length; i++)
                {
                    if (float.TryParse(floatStrings[i], out float val))
                    {
                        nextFloats[i] = val;
                    }
                }
                for (int i = 0; i < nextFloats.Length; i++)
                {
                    storage.SetFloat(saveKey + "Float" + i.ToString(), nextFloats[i]);
                }
            }
            if (nextIntegers != null)
            {
                for (int i = 0; i < nextIntegers.Length; i++)
                {
                    if (int.TryParse(integerStrings[i], out int val))
                    {
                        nextIntegers[i] = val;
                    }
                }
                for (int i = 0; i < nextIntegers.Length; i++)
                {
                    storage.SetInt(saveKey + "Int" + i.ToString(), nextIntegers[i]);
                }
            }
        }

        public void ReadFromHashtable(Hashtable hash)
        {
            if (!hash.ContainsKey(HashKeys[0]) || hash[HashKeys[0]] is int stateVal && stateVal <= 0)
            {
                state = false;
                return;
            }
            if (HashKeys.Length == 1)
            {
                if (hash.ContainsKey(HashKeys[0]))
                {
                    if (hash[HashKeys[0]] is int state)
                    {
                        this.state = state > 0;
                        if (nextSelection >= 0)
                        {
                            nextSelection = state - 1;
                        }

                        if (nextIntegers != null && nextIntegers.Length == 1)
                        {
                            nextIntegers[0] = state;
                        }
                    }
                    else if (hash[HashKeys[0]] is float val)
                    {
                        this.state = val > 0f;
                        if (nextFloats != null && nextFloats.Length == 1)
                        {
                            nextFloats[0] = val;
                        }
                    }
                }
                return;
            }
            int i = 1;
            state = hash[HashKeys[0]] is int ? (int)hash[HashKeys[0]] != 0 : false;
            state |= hash[HashKeys[0]] is bool ? (bool)hash[HashKeys[0]] : false;
            if (nextSelection >= 0)
            {
                if (hash[HashKeys[0]] is int val)
                {
                    nextSelection = val - 1;
                }
            }
            if (nextFloats != null)
            {
                for (int j = 0; j < nextFloats.Length; j++)
                {
                    if (hash[HashKeys[i++]] is float val)
                    {
                        nextFloats[j] = val;
                    }
                }
            }
            if (nextIntegers != null)
            {
                for (int j = 0; j < nextIntegers.Length; j++)
                {
                    if (hash[HashKeys[i++]] is int val)
                    {
                        nextIntegers[j] = val;
                    }
                }
            }
        }

        public GameModeSetting RemoveApplyCallback(ApplyCallback callback)
        {
            if (onApplyCallback == null)
            {
                return this;
            }
            onApplyCallback -= callback;
            return this;
        }

        public GameModeSetting RemoveChangedCallback(ChangedCallback callback)
        {
            if (onStateChanged == null)
            {
                return this;
            }
            onStateChanged -= callback;
            return this;
        }

        public void SetFloat(int index, float value)
        {
            if (appliedFloats == null || appliedFloats.Length <= index)
            {
                return;
            }

            nextFloats[index] = value;
        }

        public void SetInt(int index, int value)
        {
            if (appliedIntegers == null || appliedIntegers.Length <= index)
            {
                return;
            }

            nextIntegers[index] = value;
        }

        public override string ToString()
        {
            List<object> args = new List<object>();
            if (nextSelection >= 0)
            {
                args.Add(english.GetArray(key + "Selection")[nextSelection]);
            }
            if (nextFloats != null)
            {
                for (int i = 0; i < nextFloats.Length; i++)
                {
                    args.Add(GetFloat(i).ToString("F2"));
                }
            }
            if (nextIntegers != null)
            {
                for (int i = 0; i < nextIntegers.Length; i++)
                {
                    args.Add(GetInt(i).ToString());
                }
            }
            string format = english.Get(key + "Info" + (state ? "Enabled" : "Disabled")).Replace(@"\n", System.Environment.NewLine);
            if (state)
            {
                if (GameModes.EnabledColor.Value.Length != 6)
                {
                    GameModes.EnabledColor.Value = "CCFFCC";
                }
                format = format.Replace("$eColor$", GameModes.EnabledColor.Value);
            }
            else
            {
                if (GameModes.DisabledColor.Value.Length != 6)
                {
                    GameModes.DisabledColor.Value = "FFAACC";
                }
                format = format.Replace("$dColor$", GameModes.DisabledColor.Value);
            }
            return string.Format(format, args.ToArray());
        }

        public string ToStringLocal()
        {
            List<object> args = new List<object>();
            if (nextSelection >= 0)
            {
                args.Add(Lang.GetArray(key + "Selection")[nextSelection]);
            }
            if (nextFloats != null)
            {
                for (int i = 0; i < nextFloats.Length; i++)
                {
                    args.Add(GetFloat(i).ToString("F2"));
                }
            }
            if (nextIntegers != null)
            {
                for (int i = 0; i < nextIntegers.Length; i++)
                {
                    args.Add(GetInt(i).ToString());
                }
            }
            string format = Lang.Get(key + "Info" + (state ? "Enabled" : "Disabled")).Replace(@"\n", System.Environment.NewLine);
            if (state)
            {
                if (GameModes.EnabledColor.Value.Length != 6)
                {
                    GameModes.EnabledColor.Value = "CCFFCC";
                }
                format = format.Replace("$eColor$", GameModes.EnabledColor.Value);
            }
            else
            {
                if (GameModes.DisabledColor.Value.Length != 6)
                {
                    GameModes.DisabledColor.Value = "FFAACC";
                }
                format = format.Replace("$dColor$", GameModes.DisabledColor.Value);
            }
            return string.Format(format, args.ToArray());
        }

        public void WriteToHashtable(Hashtable hash)
        {
            if (!appliedState)
            {
                return;
            }
            hash.Add(HashKeys[0], 1);
            if (HashKeys.Length == 1)
            {
                if (appliedSelection >= 0)
                {
                    hash[HashKeys[0]] = appliedSelection;
                }

                if (nextIntegers != null && nextIntegers.Length == 1)
                {
                    hash[HashKeys[0]] = appliedIntegers[0];
                }

                return;
            }
            int i = 1;
            if (appliedSelection >= 0)
            {
                hash[HashKeys[0]] = appliedSelection;
            }
            if (appliedFloats != null)
            {
                for (int j = 0; j < appliedFloats.Length; j++)
                {
                    hash.Add(HashKeys[i++], appliedFloats[j]);
                }
            }
            if (appliedIntegers != null)
            {
                for (int j = 0; j < appliedIntegers.Length; j++)
                {
                    hash.Add(HashKeys[i++], appliedIntegers[j]);
                }
            }
        }
    }
}