public class PhotonMessageInfo
{
    private readonly int timeInt;
    public readonly PhotonView PhotonView;
    public readonly PhotonPlayer Sender;

    public PhotonMessageInfo()
    {
        Sender = PhotonNetwork.player;
        timeInt = (int)(PhotonNetwork.time * 1000.0);
        PhotonView = null;
    }

    public PhotonMessageInfo(PhotonPlayer player, int timestamp, PhotonView view)
    {
        Sender = player;
        timeInt = timestamp;
        PhotonView = view;
    }

    public int TimeInt => timeInt;

    public double Timestamp
    {
        get
        {
            return timeInt / 1000.0;
        }
    }

    public override string ToString()
    {
        return string.Format("[PhotonMessageInfo: player='{1}' timestamp={0}]", Timestamp, Sender);
    }
}