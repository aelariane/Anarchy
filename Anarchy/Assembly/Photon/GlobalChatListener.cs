//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.RegularExpressions;
//using ExitGames.Client.Photon;

//public class GlobalChatListener : IPhotonPeerListener
//{
//    private string Token;

//    public void DebugReturn(DebugLevel level, string msg)
//    {
//        //IDK
//    }

//    public void OnEvent(EventData photonEvent)
//    {
//        switch (photonEvent.Code)
//        {
//            case GlobalChatPeer.Event.Message:
//                string channel = (string)photonEvent[GlobalChatPeer.Parameter.Channel];
//                object[] messages = (object[])photonEvent[GlobalChatPeer.Parameter.Messages];
//                string[] senders = (string[])photonEvent[GlobalChatPeer.Parameter.Senders];
//                //int lastMSGId = (int)photonEvent[NetworkingPeer2.Parameter.MsgId];
//                for (int i = 0; i < messages.Length; i++)
//                {
//                    MiniPhoton.messages.Add(senders[i] + ":" + messages[i]);
//                }
//                break;

//            case GlobalChatPeer.Event.Subscribe:
//                string[] channels = (string[])photonEvent[GlobalChatPeer.Parameter.Channels];
//                //bool[] subbedChannels = (bool[])photonEvent[NetworkingPeer2.Parameter.SubscriberResults];
//                //Dictionary<object, object> properties = (Dictionary<object, object>)photonEvent[NetworkingPeer2.Parameter.Properties];
//                //int num = 0;
//                MiniPhoton.messages.Add("Subscribed to channel (" + string.Join(", ", channels) + ").");
//                if (photonEvent.Parameters.ContainsKey(GlobalChatPeer.Parameter.ChannelSubscribers))
//                {
//                    string[] subcribers = (string[])photonEvent[GlobalChatPeer.Parameter.ChannelSubscribers];
//                    string s = string.Join(", ", subcribers);
//                    string pattern = @"\w{8}-\w{4}-\w{4}-\w{4}-\w{12}";
//                    if (Regex.IsMatch(s, pattern))
//                    {
//                        s = Regex.Replace(s, pattern, "Unnamed");
//                    }
//                    MiniPhoton.messages.Add("Currently online users: " + s);
//                }
//                break;

//            case GlobalChatPeer.Event.UserSubscribed:
//                channel = (string)photonEvent[GlobalChatPeer.Parameter.Channel];
//                string sender = (string)photonEvent[GlobalChatPeer.Parameter.UserId];
//                string pattern2 = @"\w{8}-\w{4}-\w{4}-\w{4}-\w{12}";
//                if (Regex.IsMatch(sender, pattern2))
//                {
//                    sender = Regex.Replace(sender, pattern2, "Unnamed");
//                }
//                MiniPhoton.messages.Add(sender + " has joined channel (" + channel + ").");
//                break;

//            case GlobalChatPeer.Event.UserUnsubscribed:
//                channel = (string)photonEvent[GlobalChatPeer.Parameter.Channel];
//                sender = (string)photonEvent[GlobalChatPeer.Parameter.UserId];
//                pattern2 = @"\w{8}-\w{4}-\w{4}-\w{4}-\w{12}";
//                if (Regex.IsMatch(sender, pattern2))
//                {
//                    sender = Regex.Replace(sender, pattern2, "Unnamed");
//                }
//                MiniPhoton.messages.Add(sender + " has left channel (" + channel + ").");
//                break;

//            case GlobalChatPeer.Event.PrivateMessage:

//                break;

//            case GlobalChatPeer.Event.Status:
//                break;
//        }
//    }

//    public void OnOperationResponse(OperationResponse operationResponse)
//    {
//        switch (operationResponse.OperationCode)
//        {
//            case GlobalChatPeer.Operation.Subscribe:
//                if (operationResponse.ReturnCode != 0)
//                {
//                }
//                break;

//            case GlobalChatPeer.Operation.SendPrivate:
//                if (operationResponse.ReturnCode != 0)
//                {
//                }
//                break;

//            case GlobalChatPeer.Operation.Unsubscribe:
//                if (operationResponse.ReturnCode != 0)
//                {
//                }
//                break;

//            case GlobalChatPeer.Operation.UpdateStatus:
//                if (operationResponse.ReturnCode != 0)
//                {
//                }
//                break;

//            case GlobalChatPeer.Operation.UpdateChannelProperties:
//                if (operationResponse.ReturnCode != 0)
//                {
//                }
//                break;

//            case GlobalChatPeer.Operation.Authenticate:
//                if (operationResponse.ReturnCode == 0)
//                {
//                    if (GlobalChatPeer.Peer.ServerAddress.Contains("app"))
//                    {
//                        Token = operationResponse[GlobalChatPeer.Parameter.Token].ToString();
//                        GlobalChatPeer.Peer.Disconnect();
//                    }
//                    else
//                    {
//                        GlobalChatPeer.Peer.Subscribe(new string[] { "main" }, 6);
//                    }
//                }
//                else
//                {
//                }
//                break;
//        }
//    }

//    public void OnStatusChanged(StatusCode stCode)
//    {
//        switch (stCode)
//        {
//            case StatusCode.Connect:
//                if (Token != null)
//                {
//                    GlobalChatPeer.Peer.Authenticate("<color=#ffffff>B</color><color=#000000>ahaa</color>", Token);
//                }
//                else
//                {
//                    GlobalChatPeer.Peer.EstablishEncryption();
//                }
//                break;

//            case StatusCode.EncryptionEstablished:
//                GlobalChatPeer.Peer.Authenticate("<color=#ffffff>B</color><color=#000000>ahaa</color>", Token);
//                break;

//            case StatusCode.EncryptionFailedToEstablish:
//                break;

//            case StatusCode.Disconnect:
//                GlobalChatPeer.Peer.EstablishConnection("chat-eu.exitgames.com:5057");
//                break;
//        }

//    }
//}