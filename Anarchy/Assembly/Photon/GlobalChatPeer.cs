//using System.Collections.Generic;
//using System.Threading;
//using ExitGames.Client.Photon;

//public class GlobalChatPeer : PhotonPeer
//{
//    private readonly Dictionary<byte, object> _parameters = new Dictionary<byte, object>();
//    private readonly object _syncer = new object();
//    private Thread _mainThread;

//    public static GlobalChatPeer Peer;

//    public string ApplicationId { get; set; }
//    public int DelayMs { get; set; }
//    public string Identifier { get; private set; } //Used for user identifying
//    public static bool IsRunning { get; private set; }

//    public string Name { get; set; } //Name

//    public GlobalChatPeer(IPhotonPeerListener listener, ConnectionProtocol connectionProtocol)
//        : base(listener, connectionProtocol)
//    {
//        ApplicationId = FengGameManagerMKII.ApplicationId;
//        DelayMs = 100;
//        Peer = this;
//        Identifier = GetIdentifier(); //Prolly can be used instead of name to identify person? As long as should be unique per user
//        Name = "<color=#9999FF>Chat_Guest</color>";
//        base.Listener = new GlobalChatListener();
//    }

//    public void AddFriend()
//    {
//        lock (_syncer)
//        {
//            _parameters.Clear();
//        }
//    }

//    public bool Authenticate(string userId, string token)
//    {
//        lock (_syncer)
//        {
//            bool result;
//            if (token != null)
//            {
//                _parameters.Clear();
//                _parameters[Parameter.Token] = token;
//                result = OpCustom(Operation.Authenticate, _parameters, true);
//            }
//            else
//            {
//                _parameters[Parameter.UserId] = userId;
//                _parameters[Parameter.AppID] = "5578b046-8264-438c-99c5-fb15c71b6744";
//                _parameters[Parameter.AppVersion] = "01042015_1.28";
//                result = OpCustom(Operation.Authenticate, _parameters, true, 0, true);
//            }
//            return result;
//        }
//    }

//    public void EstablishConnection(string serverAddress)
//    {
//        base.Connect(serverAddress, string.Empty);
//    }

//    private string GetIdentifier()
//    {
//        byte[] bytes = new byte[8];
//        new System.Random().NextBytes(bytes);
//        string result = string.Empty;
//        for(int i = 0; i < bytes.Length; i++)
//        {
//            result = bytes[i].ToString("X2").ToUpper();
//        }
//        return result;
//    }

//    private void Process()
//    {
//        while (true)
//        {
//            lock (_syncer)
//            {
//                while (SendOutgoingCommands()) { }
//                while (DispatchIncomingCommands()) { }
//            }
//            Thread.Sleep(DelayMs);
//        }
//    }

//    public bool SendMessage(string content)
//    {
//        lock (_syncer)
//        {
//            _parameters.Clear();
//            _parameters[Parameter.Channel] = "main";
//            _parameters[Parameter.Message] = content;
//            return OpCustom(Operation.Publish, _parameters, true);
//        }
//    }

//    public void SetStatus()
//    {
//        _parameters.Clear();
//    }

//    public void Start()
//    {
//        if (IsRunning)
//        {
//            throw new System.InvalidOperationException(nameof(GlobalChatPeer) + " is already running. Avoid create another instance of it.");
//        }
//        _mainThread = new Thread(Process);
//        _mainThread.Name = nameof(GlobalChatPeer) + "Thread";
//        _mainThread.IsBackground = true;
//        _mainThread.Priority = ThreadPriority.BelowNormal;
//        _mainThread.Start();
//    }

//    public bool Subscribe(string[] chs, int messageHistory)
//    {
//        _parameters.Clear();
//        _parameters[Parameter.Channels] = chs; //channels to subscribe
//        _parameters[Parameter.HistoryLength] = messageHistory; //-1 all history, 0 no history
//        _parameters[Parameter.Properties] = new Dictionary<object, object> { { (byte)254, true } }; //todo: addMaxSubscribers
//        return OpCustom(Operation.Subscribe, _parameters, true);
//    }

//    public void Unsubscribe(string[] ch)
//    {
//        _parameters.Clear();
//    }

//    public class Status
//    {
//        public const int Offline = 0;
//        public const int Invisible = 1;
//        public const int Online = 2;
//        public const int Away = 3;
//        public const int DND = 4;
//        public const int LFG = 5;
//        public const int Playing = 6;
//    }

//    public class Operation
//    {
//        public const byte Subscribe = 0;
//        public const byte Unsubscribe = 1;
//        public const byte Publish = 2;
//        public const byte SendPrivate = 3;
//        public const byte UpdateStatus = 5;
//        public const byte AddFriends = 6;
//        public const byte RemoveFriends = 7;
//        public const byte UpdateChannelProperties = 8;
//        public const byte Authenticate = 230;
//    }

//    public class Event
//    {
//        public const byte UserUnsubscribed = 9;
//        public const byte UserSubscribed = 8;
//        public const byte Unsubscribe = 6;
//        public const byte Subscribe = 5;
//        public const byte Status = 4;
//        public const byte PrivateMessage = 2;
//        public const byte Message = 0;
//    }

//    public class Parameter
//    {
//        public const byte ServerAddress = 230;
//        public const byte UserId = 225;
//        public const byte AppID = 224;
//        public const byte Token = 221;
//        public const byte AppVersion = 220;
//        public const byte RegionCode = 210;
//        public const byte ChannelSubscribers = 23;
//        public const byte Properties = 22;
//        public const byte SubscriberResults = 15;
//        public const byte HistoryLength = 14;
//        public const byte SkipMessage = 12;
//        public const byte Friends = 11;
//        public const byte Status = 10;
//        public const byte MsgIds = 9;
//        public const byte MsgId = 8;
//        public const byte Sender = 5;
//        public const byte Senders = 4;
//        public const byte Message = 3;
//        public const byte Messages = 2;
//        public const byte Channel = 1;
//        public const byte Channels = 0;
//    }
//}