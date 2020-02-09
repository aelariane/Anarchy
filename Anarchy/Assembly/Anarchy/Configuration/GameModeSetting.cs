using System.Collections.Generic;
using Anarchy.Localization;
using Anarchy.Configuration.Storage;
using Anarchy.UI;
using ExitGames.Client.Photon;
using static Anarchy.UI.GUI;

namespace Anarchy.Configuration
{
    public class GameModeSetting : ISetting
    {
        private const char Separator = ',';

        public delegate void StateChanged(GameModeSetting sender, bool state, bool received);
        private delegate void GameModeSettingDraw(SmartRect rect, Locale locale);

        private static readonly Locale english;
        private static readonly Locale lang;

        private readonly GameModeSettingDraw draw;
        private readonly float[] floats = null;
        private string[] floatStrings = null;
        private bool forcedChange;
        public readonly string[] HashKeys;
        private readonly int[] integers = null;
        private string[] integerStrings = null;
        private readonly string key;
        private float[] oldFloats = null;
        private int[] oldIntegers = null;
        private int oldSelection = -1;
        private bool oldState;
        private readonly string saveKey;
        private int selection = -1;
        private bool state;
        private StateChanged onStateChanged  = (set, val, received) => { };

        public bool Enabled
        {
            get
            {
                return oldState && IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer;
            }
        }

        public bool HasChanged
        {
            get
            {
                if (state == oldState && oldState == false)
                    return false;
                if (forcedChange || state != oldState)
                    return true;
                if (selection >= 0)
                {
                    if (oldSelection != selection + 1)
                        return true;
                }
                if (floats != null)
                {
                    for(int i = 0; i < floats.Length; i++)
                    {
                        if (float.TryParse(floatStrings[i], out float val))
                        {
                            
                            if (!oldFloats[i].Equals(val))
                                return true;
                        }
                    }
                }
                if (integers!= null)
                {
                    for (int i = 0; i < integers.Length; i++)
                    {
                        if (int.TryParse(integerStrings[i], out int val))
                        {
                            if (!oldIntegers[i].Equals(val))
                                return true;
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
                if (state == oldState && oldState == false)
                    return false;
                if (forcedChange || state != oldState)
                    return true;
                if (selection >= 0)
                {
                    if (oldSelection != selection + 1)
                        return true;
                }
                if (floats != null)
                {
                    for (int i = 0; i < floats.Length; i++)
                    {
                        if (!oldFloats[i].Equals(floats[i]))
                            return true;
                    }
                }
                if (integers != null)
                {
                    for (int i = 0; i < integers.Length; i++)
                    {
                        if (!oldIntegers[i].Equals(integers[i]))
                            return true;
                    }
                }
                return false;
            }
        }

        public int Selection
        {
            get
            {
                if (oldSelection < 0)
                    return -1;
                return oldSelection + 1;
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

        public GameModeSetting(string key) : this(key, -1, null, null) { }

        public GameModeSetting(string key, int sel) : this(key, sel, null, null) { }

        public GameModeSetting(string key, float[] floats) : this(key, -1, floats, null) { }

        public GameModeSetting(string key, int[] ints) : this(key, -1, null, ints) { }

        public GameModeSetting(string key, int sel, float[] floats) : this(key, sel, floats, null)  { }

        public GameModeSetting(string key, int sel, int[] ints) : this(key, sel, null, ints) { }

        public GameModeSetting(string key, int selection, float[] floats, int[] ints)
        {
            if (Settings.Storage == null)
                Settings.CreateStorage();
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
            this.selection = selection;
            this.floats = floats;
            integers = ints;

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
            AddCallback(RCSettingCallback);
        }

        static GameModeSetting()
        {

            english = new Locale("English", "GameModes", true, ',');
            english.Reload();
            lang = new Locale("GameModes", true);
            lang.Reload();
        }

        public GameModeSetting AddCallback(StateChanged callback)
        {
            if(onStateChanged == null)
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
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", targets, new object[] { set.ToString(false), string.Empty });
            }
        }

        public void Apply()
        {
            forcedChange = false;
            oldState = state;
            if(selection >= 0)
            {
                oldSelection = selection + 1;
            }
            if(floats != null)
            {
                for(int i = 0; i < floats.Length; i++)
                {
                    if(float.TryParse(floatStrings[i], out float val))
                    {
                        floats[i] = val;
                    }
                    else
                    {
                        floatStrings[i] = floats[i].ToString("F2");
                    }
                    oldFloats[i] = floats[i];
                }
            }
            if (integers!= null)
            {
                for (int i = 0; i < integers.Length; i++)
                {
                    if (int.TryParse(integerStrings[i], out int val))
                    {
                        integers[i] = val;
                    }
                    else
                    {
                        integerStrings[i] = integers[i].ToString();
                    }
                    oldIntegers[i] = integers[i];
                }
            }
            onStateChanged(this, state, false);
        }

        public void ApplyReceived()
        {
            forcedChange = false;
            oldState = state;
            if (selection >= 0)
            {
                oldSelection = selection + 1;
            }
            if (floats != null)
            {
                for (int i = 0; i < floats.Length; i++)
                {
                    oldFloats[i] = floats[i];
                }
            }
            if (integers != null)
            {
                for (int i = 0; i < integers.Length; i++)
                {
                    oldIntegers[i] = integers[i];
                }
            }
            onStateChanged(this, state, true);
        }

        public void Draw(SmartRect rect, Locale locale)
        {
            state = ToggleButton(rect, state, locale[key + "State"], true);
            //if (State)
            draw(rect, locale);
            
        }

        private void DrawFloats(SmartRect rect, Locale loc)
        {
            for(int i = 0; i < floatStrings.Length; i++)
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
            selection = SelectionGrid(rect, selection, labels, labels.Length, true);
        }

        public void ForceChange()
        {
            forcedChange = true;
        }

        public void ForceDisable()
        {
            state = false;
            oldState = false;
        }

        public float GetFloat(int index)
        {
            if (oldFloats == null || oldFloats.Length <= index)
                return 0f;
            return oldFloats[index];
        }

        public int GetInt(int index)
        {
            if (oldIntegers == null || oldIntegers.Length <= index)
                return 0;
            return oldIntegers[index];
        }

        public void Load()
        {
            IDataStorage storage = Settings.Storage;
            state = storage.GetBool(saveKey + "State", false);
            if (selection >= 0)
            {
                selection = storage.GetInt(saveKey + "Selection", selection);
            }
            if(floats != null)
            {
                floatStrings = new string[floats.Length];
                oldFloats = new float[floats.Length];
                for (int i = 0; i < floats.Length; i++)
                {
                    floatStrings[i] = storage.GetFloat(saveKey + "Float" + i.ToString(), floats[i]).ToString();
                    if(!float.TryParse(floatStrings[i], out float val))
                    {
                        floatStrings[i] = floats[i].ToString("F2");
                    }
                    else
                    {
                        floats[i] = val;
                    }
                    oldFloats[i] = floats[i];
                }
            }
            if (integers != null)
            {
                integerStrings = new string[integers.Length];
                oldIntegers = new int[integers.Length];
                for (int i = 0; i < integers.Length; i++)
                {
                    integerStrings[i] = storage.GetInt(saveKey + "Int" + i.ToString(), integers[i]).ToString();
                    if (!int.TryParse(integerStrings[i], out int val))
                    {
                        integerStrings[i] = integers[i].ToString();
                    }
                    else
                    {
                        integers[i] = val;
                    }
                    oldIntegers[i] = integers[i];
                }
            }
        }

        public void Save()
        {
            IDataStorage storage = Settings.Storage;
            storage.SetBool(saveKey + "State", state);
            if(selection >= 0)
            {
                storage.SetInt(saveKey + "Selection", selection);
            }
            if (floats != null)
            {
                for (int i = 0; i < floats.Length; i++)
                {
                    if (float.TryParse(floatStrings[i], out float val))
                    {
                        floats[i] = val;
                    }
                }
                for (int i = 0; i < floats.Length; i++)
                {
                    storage.SetFloat(saveKey + "Float" + i.ToString(), floats[i]);
                }
            }
            if (integers != null)
            {
                for (int i = 0; i < integers.Length; i++)
                {
                    if (int.TryParse(integerStrings[i], out int val))
                    {
                        integers[i] = val;
                    }
                }
                for (int i = 0; i < integers.Length; i++)
                {
                    storage.SetInt(saveKey + "Int" + i.ToString(), integers[i]);
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
            if(HashKeys.Length == 1)
            {
                if (hash.ContainsKey(HashKeys[0]))
                {
                    if(hash[HashKeys[0]] is int state)
                    {
                        this.state = state > 0;
                        if (selection >= 0)
                            selection = state - 1;
                        if (integers != null && integers.Length == 1)
                            integers[0] = state;
                    }
                    else if(hash[HashKeys[0]] is float val)
                    {
                        this.state = val > 0f;
                        if (floats != null && floats.Length == 1)
                            floats[0] = val;
                    }
                }
                return;
            }
            int i = 1;
            state = ((int)hash[HashKeys[0]] != 0);
            if(selection >= 0)
            {
                if (hash[HashKeys[0]] is int val)
                    selection = val - 1;
            }
            if(floats != null)
            {
                for(int j = 0; j < floats.Length; j++)
                {
                    if (hash[HashKeys[i++]] is float val)
                        floats[j] = val;
                }
            }
            if(integers != null)
            {
                for (int j = 0; j < integers.Length; j++)
                {
                    if (hash[HashKeys[i++]] is int val)
                        integers[j] = val;
                }
            }
        }

        public GameModeSetting RemoveCallback(StateChanged callback)
        {
            if (onStateChanged == null)
            {
                return this;
            }
            onStateChanged -= callback;
            return this;
        }

        public override string ToString()
        {
            List<object> args = new List<object>();
            if (Selection >= 0)
            {
                args.Add(english.GetArray(key + "Selection")[Selection]);
            }
            if (floats != null)
            {
                for(int i = 0; i < floats.Length; i++)
                {
                    args.Add(GetFloat(i).ToString("F2"));
                }
            }
            if (integers != null)
            {
                for(int i = 0; i < integers.Length; i++)
                {
                    args.Add(GetInt(i).ToString());
                }
            }
            string format = english.Get(key + "Info" + (state ? "Enabled" : "Disabled")).Replace(@"\n", System.Environment.NewLine);
            if (state)
            {
                if(GameModes.EnabledColor.Value.Length != 6)
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

        public string ToString(bool local)
        {
            Locale loc = local ? lang : english;
            List<object> args = new List<object>();
            if (selection >= 0)
            {
                args.Add(loc.GetArray(key + "Selection")[selection]);
            }
            if (floats != null)
            {
                for (int i = 0; i < floats.Length; i++)
                {
                    args.Add(GetFloat(i).ToString("F2"));
                }
            }
            if (integers != null)
            {
                for (int i = 0; i < integers.Length; i++)
                {
                    args.Add(GetInt(i).ToString());
                }
            }
            string format = loc.Get(key + "Info" + (state ? "Enabled" : "Disabled")).Replace(@"\n", System.Environment.NewLine);
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
            if (!oldState)
            {
                return;
            }
            hash.Add(HashKeys[0], 1);
            if (HashKeys.Length == 1)
            {
                if (oldSelection >= 0)
                    hash[HashKeys[0]] = oldSelection;
                if (integers != null && integers.Length == 1)
                    hash[HashKeys[0]] = oldIntegers[0];
                return;
            }
            int i = 1;
            if(oldSelection >= 0)
            {
                hash[HashKeys[0]] = oldSelection;
            }
            if (oldFloats != null)
            {
                for(int j = 0 ; j < oldFloats.Length; j++)
                {
                    hash.Add(HashKeys[i++], oldFloats[j]);
                }
            }
            if(oldIntegers != null)
            {
                for (int j = 0; j < oldIntegers.Length; j++)
                {
                    hash.Add(HashKeys[i++], oldIntegers[j]);
                }
            }
        }
    }
}
