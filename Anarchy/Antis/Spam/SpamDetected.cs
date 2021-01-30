namespace Antis.Spam
{
    public delegate void SpamDetected(object sender, SpamDetectedArgs args);

    public delegate void SpamDetected<T>(object sender, SpamDetectedArgs<T> args);
}