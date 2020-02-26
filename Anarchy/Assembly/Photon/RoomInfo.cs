using Anarchy;
using Antis;
using ExitGames.Client.Photon;

public class RoomInfo
{
    protected bool autoCleanUpField = PhotonNetwork.autoCleanUpPlayerObjects;
    private string cachedname;
    protected byte maxPlayersField;
    protected string nameField;
    protected bool openField = true;
    protected bool visibleField = true;


    public bool HasPassword
    {
        get
        {
            string[] array = Name.Split('`');
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

    public Hashtable CustomProperties { get; } = new Hashtable();

    public bool IsLocalClientInside { get; set; }

    public byte MaxPlayers
    {
        get
        {
            return this.maxPlayersField;
        }
    }

    public string Name
    {
        get
        {
            return this.nameField;
        }
    }

    public bool Open
    {
        get
        {
            return this.openField;
        }
    }

    public int PlayerCount { get; private set; }
    public bool RemovedFromList { get; internal set; }

    public bool Visible
    {
        get
        {
            return this.visibleField;
        }
    }

    protected internal void CacheProperties(Hashtable propertiesToCache)
    {
        if (((propertiesToCache != null) && (propertiesToCache.Count != 0)) && !CustomProperties.Equals(propertiesToCache))
        {
            if(propertiesToCache.CheckKey(RoomProperty.RemovedFromList, out bool removed, true))
            {
                RemovedFromList = removed;
            }
            if(propertiesToCache.CheckKey(RoomProperty.MaxPlayers, out byte max, true))
            {
                maxPlayersField = max;
            }
            if (propertiesToCache.CheckKey(RoomProperty.Visibility, out bool visible, true))
            {
                visibleField = visible;
            }
            if (propertiesToCache.CheckKey(RoomProperty.Joinable, out bool joinable, true))
            {
                openField = joinable;
            }
            if (propertiesToCache.CheckKey(RoomProperty.PlayerCount, out byte players, true))
            {
                PlayerCount = players;
            }
            if(propertiesToCache.CheckKey(RoomProperty.CleanUpCacheOnLeave, out bool cleanup, true))
            {
                autoCleanUpField = cleanup;
            }
            CustomProperties.MergeStringKeys(propertiesToCache);
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
        string[] array = Name.Split('`');
        if (array.Length != 7)
        {
            return false;
        }
        string decrypted = new SimpleAES().Decrypt(array[5]);
        return decrypted.Equals(pass);
    }



    public override string ToString()
    {
        object[] args = new object[] { this.nameField, !this.visibleField ? "hidden" : "visible", !this.openField ? "closed" : "open", this.maxPlayersField, this.PlayerCount };
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.", args);
    }

    public string ToStringFull()
    {
        object[] args = new object[] { this.nameField, !this.visibleField ? "hidden" : "visible", !this.openField ? "closed" : "open", this.maxPlayersField, this.PlayerCount, this.CustomProperties.ToStringFull() };
        return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", args);
    }

    public string UIName
    {
        get
        {
            if (cachedname == null)
            {
                var strArray = Name.Split('`');
                if (strArray.Length != 7)
                {
                    cachedname = $"Invalid server name[{strArray.Length}]";
                    return $"{cachedname} {PlayerCount}/{MaxPlayers}";
                }
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
            string max = PlayerCount == MaxPlayers ? $"[FF0000][{PlayerCount}/{MaxPlayers}][-]" : $"[{PlayerCount}/{MaxPlayers}]";
            return $"{cachedname} {max}";
        }
    }
}
