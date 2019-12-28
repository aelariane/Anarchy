using System;

public class EventCode
{
    public const byte AppStats = 226;

    [Obsolete("TCP routing was removed after becoming obsolete.")]
    public const byte AzureNodeInfo = 210;

    public const byte GameList = 230;

    public const byte GameListUpdate = 229;

    public const byte Join = 255;
    public const byte Leave = 254;
    public const byte Match = 227;
    public const byte PropertiesChanged = 253;
    public const byte QueueState = 228;

    [Obsolete("Use PropertiesChanged now.")]
    public const byte SetProperties = 253;

    public const byte TypedLobbyStats = 224;
}