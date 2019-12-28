public class FriendInfo
{
    public bool IsInRoom
    {
        get
        {
            return this.IsOnline && !string.IsNullOrEmpty(this.Room);
        }
    }

    public bool IsOnline { get; protected internal set; }
    public string Name { get; protected internal set; }
    public string Room { get; protected internal set; }

    public override string ToString()
    {
        return string.Format("{0}\t is: {1}", this.Name, this.IsOnline ? ((!this.IsInRoom) ? "on master" : "playing") : "offline");
    }
}