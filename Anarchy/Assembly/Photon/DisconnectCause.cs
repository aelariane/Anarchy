using System;

public enum DisconnectCause
{
    ExceptionOnConnect = 1023,
    SecurityExceptionOnConnect = 1022,

    [Obsolete("Replaced by clearer: DisconnectByClientTimeout")]
    TimeoutDisconnect = 1040,

    DisconnectByClientTimeout = 1040,
    InternalReceiveException = 1039,

    [Obsolete("Replaced by clearer: DisconnectByServerTimeout")]
    DisconnectByServer = 1041,

    DisconnectByServerTimeout = 1041,
    DisconnectByServerLogic = 1043,
    DisconnectByServerUserLimit = 1042,
    Exception = 1026,
    InvalidRegion = 32756,
    MaxCcuReached,
    InvalidAuthentication = 32767,
    AuthenticationTicketExpired = 32753
}