
public class RaiseEventOptions
{
    public static readonly RaiseEventOptions Default = new RaiseEventOptions();

    public int CacheSliceIndex;
    public EventCaching CachingOption;

    public bool ForwardToWebhook;
    public byte InterestGroup;

    public ReceiverGroup Receivers;
    public byte SequenceChannel;
    public int[] TargetActors;
}