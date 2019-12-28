using Anarchy;
using ExitGames.Client.Photon;

public class RoomInfo
{
    private Hashtable customPropertiesField = new Hashtable();

    protected bool autoCleanUpField = PhotonNetwork.autoCleanUpPlayerObjects;

    protected byte maxPlayersField;

    protected string nameField;

    protected bool openField = true;

    protected bool visibleField = true;

    private string cachedname;

    public bool HasPassword
    {
        get
        {
            string[] array = name.Split('`');
            if(array.Length != 7)
            {
                return false;
            }
            return array[5].Length > 0;
        }
    }

    protected internal RoomInfo(string roomName, Hashtable properties)
    {
        this.CacheProperties(properties);
        this.nameField = roomName;
    }

    public Hashtable customProperties
    {
        get
        {
            return this.customPropertiesField;
        }
    }

    public bool isLocalClientInside { get; set; }

    public byte maxPlayers
    {
        get
        {
            return this.maxPlayersField;
        }
    }

    public string name
    {
        get
        {
            return this.nameField;
        }
    }

    public bool open
    {
        get
        {
            return this.openField;
        }
    }

    public int playerCount { get; private set; }
    public bool removedFromList { get; internal set; }

    public bool visible
    {
        get
        {
            return this.visibleField;
        }
    }

    protected internal void CacheProperties(Hashtable propertiesToCache)
    {
        if (((propertiesToCache != null) && (propertiesToCache.Count != 0)) && !this.customPropertiesField.Equals(propertiesToCache))
        {
            if (propertiesToCache.ContainsKey((byte)0xfb) && propertiesToCache[(byte)0xfb] is bool)
            {
                this.removedFromList = (bool)propertiesToCache[(byte)0xfb];
            }
            else if (propertiesToCache.ContainsKey((byte)0xfb))
            {
                propertiesToCache.Remove((byte)0xfb);
            }
            if (propertiesToCache.ContainsKey((byte)0xff) && propertiesToCache[(byte)0xff] is byte)
            {
                this.maxPlayersField = (byte)propertiesToCache[(byte)0xff];
            }
            else if (propertiesToCache.ContainsKey((byte)0xff))
            {
                propertiesToCache.Remove((byte)0xff);
            }
            if (propertiesToCache.ContainsKey((byte)0xfd) && propertiesToCache[(byte)0xfd] is bool)
            {
                this.openField = (bool)propertiesToCache[(byte)0xfd];
            }
            else if (propertiesToCache.ContainsKey((byte)0xfd))
            {
                propertiesToCache.Remove((byte)0xfd);
            }
            if (propertiesToCache.ContainsKey((byte)0xfe) && propertiesToCache[(byte)0xfe] is bool)
            {
                this.visibleField = (bool)propertiesToCache[(byte)0xfe];
            }
            else if (propertiesToCache.ContainsKey((byte)0xfe))
            {
                propertiesToCache.Remove((byte)0xfe);
            }
            if (propertiesToCache.ContainsKey((byte)0xfc) && propertiesToCache[(byte)0xfc] is byte)
            {
                this.playerCount = (byte)propertiesToCache[(byte)0xfc];
            }
            else if (propertiesToCache.ContainsKey((byte)0xfc))
            {
                propertiesToCache.Remove((byte)0xfc);
            }
            if (propertiesToCache.ContainsKey((byte)0xf9) && propertiesToCache[(byte)0xf9] is bool)
            {
                this.autoCleanUpField = (bool)propertiesToCache[(byte)0xf9];
            }
            else if (propertiesToCache.ContainsKey((byte)0xf9))
            {
                propertiesToCache.Remove((byte)0xf9);
            }
            this.customPropertiesField.MergeStringKeys(propertiesToCache);
        }
    }

    public override bool Equals(object p)
    {
        Room room = p as Room;
        return ((room != null) && this.nameField.Equals(room.nameField));
    }

    public override int GetHashCode()
    {
        return this.nameField.GetHashCode();
    }

    public bool IsCorrectPassword(string pass)
    {
        if (!HasPassword)
        {
            return true;
        }
        string[] array = name.Split('`');
        if (array.Length != 7)
        {
            return false;
        }
        string decrypted = new SimpleAES().Decrypt(array[5]);
        UnityEngine.Debug.Log($"decrypted: {decrypted}, pass {pass}, decrypted.Equals(pass) {decrypted.Equals(pass)}");
        return decrypted.Equals(pass);
    }



    public override string ToString()
    {
        object[] args = new object[] { this.nameField, !this.visibleField ? "hidden" : "visible", !this.openField ? "closed" : "open", this.maxPlayersField, this.playerCount };
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", args);
    }

    public string ToStringFull()
    {
        object[] args = new object[] { this.nameField, !this.visibleField ? "hidden" : "visible", !this.openField ? "closed" : "open", this.maxPlayersField, this.playerCount, this.customPropertiesField.ToStringFull() };
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", args);
    }

    public string UIName
    {
        get
        {
            if (cachedname == null)
            {
                var strArray = name.Split('`');
                if (strArray.Length != 7)
                {
                    cachedname = $"Invalid server name[{strArray.Length}]";
                    return $"{cachedname} {playerCount}/{maxPlayers}";
                }
                //if (Astora.Antis.CheckRoomData(strArray))
                //{
                //    visibleField = false;
                //    string log = "";
                //    foreach (string str in strArray)
                //    {
                //        log += str + System.Environment.NewLine;
                //    }
                //    Astora.FileLogger.AddLine("Invalid room name: " + log);
                //    return string.Empty;
                //}
                string open = openField ? "" : "[FF0000][Closed][-] ";
                string pass = strArray[5].Length > 0 ? "[PWD] " : string.Empty;
                int length = strArray[0].RemoveHex().Length;
                string rname = strArray[0];
                if (length > 46 && length < 100)
                {
                    rname = rname.RemoveHex().Substring(0, 43) + "...";
                }
                else if (length > 100)
                {
                    rname = "[FF0000]Big map name[" + length + "][-]";
                }
                cachedname = $"{open}{pass}{rname}/{strArray[1]}/{strArray[2]}/{strArray[4]}";
                if (!int.TryParse(strArray[6], out int roomID))
                {
                    cachedname += "/[FF0000]RoomID: " + strArray[6] + "[-]/";
                }
            }
            string max = playerCount == maxPlayers ? $"[FF0000][{playerCount}/{maxPlayers}][-]" : $"[{playerCount}/{maxPlayers}]";
            return $"{cachedname} {max}";
        }
    }
}