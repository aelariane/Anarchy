using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Anarchy.Network.Events
{
    public class EventOSR : INetworkEvent
    {
        private short correctPrefix = -1;
        private Queue<object[]> hashData = new Queue<object[]>();
        private Queue<int> hashViews = new Queue<int>();
        private string myReason = "";
        private PhotonPlayer sender;
        private int sentTime;

        public byte Code { get; }

        public EventOSR()
        {
            Code = 201;
            NetworkManager.RegisterEvent(this);
            Code = 206;
            NetworkManager.RegisterEvent(this);
        }

        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = "";
            this.sender = sender;
            Hashtable hash = data[245] as Hashtable;
            if (hash == null)
            {
                reason += UI.Log.GetString("notHashOrNull");
                return false;
            }
            if (hash.Count < 1)
            {
                reason += UI.Log.GetString("invalidParamsCount", hash.Count.ToString());
                return false;
            }
            if (!CheckKey(hash, 0, out sentTime))
            {
                reason += UI.Log.GetString("missOrInvalidKey", "0");
                return false;
            }
            short index = 2;
            if (!CheckKey(hash, 1, out correctPrefix, true))
            {
                reason += UI.Log.GetString("invalidKey", "1");
                return false;
            }
            if (!hash.ContainsKey((byte)1))
            {
                index--;
                correctPrefix = -1;
            }
            hashViews.Clear();
            hashData.Clear();
            for (short i = index; i < hash.Count; i++)
            {
                if (hash[(short)i] is Hashtable add && add != null)
                {
                    if (add.Count != 2)
                    {
                        reason += UI.Log.GetString("invalidKeyArgd", i.ToString(), UI.Log.GetString("invalidParamsCount", add.Count.ToString()));
                        return false;
                    }
                    if (!CheckKey(add, 0, out int view))
                    {
                        reason += UI.Log.GetString("missOrInvalidKeyArgd", i.ToString(), UI.Log.GetString("byteKey", "0"));
                        return false;
                    }
                    if (!CheckKey(add, 1, out object[] store))
                    {
                        reason += UI.Log.GetString("missOrInvalidKeyArgd", i.ToString(), UI.Log.GetString("byteKey", "1"));
                        return false;
                    }
                    hashViews.Enqueue(view);
                    hashData.Enqueue(store);
                    continue;
                }
                reason += UI.Log.GetString("invalidKeyArgd", i.ToString(), UI.Log.GetString("notHashOrNull"));
                return false;
            }
            return true;
        }

        private bool CheckKey<T>(Hashtable src, byte kkey, out T val, bool force = false)
        {
            if (src.ContainsKey(kkey))
            {
                if (src[kkey] is T idk)
                {
                    val = idk;
                    return val != null;
                }
                val = default(T);
                return false;
            }
            val = default(T);
            return force;
        }

        public bool Handle()
        {
            int count = hashData.Count;
            for (int i = 0; i < count; i++)
            {
                PhotonView view = PhotonView.Find(hashViews.Dequeue());
                if (view == null)
                {
                    //Message what view ID not exists
                    //Maybe put spam for invalid viewIDs, like 100 per second, idk
                    return true;
                }
                else if (view.prefix > 0 && correctPrefix != view.prefix)
                {
                    Debug.LogError(string.Concat(new object[] { "Received OnSerialization for view ID ", view.viewID, " with prefix ", correctPrefix, ". Our prefix is ", view.prefix }));
                }
                if (view.observed == null)
                {
                    myReason = $"OSR: Null observed for ViewID[{view.viewID.ToString()}]";
                    return false;
                }
                if (view.observed is MonoBehaviour)
                {
                    PhotonStream pStream = new PhotonStream(false, hashData.Dequeue());
                    PhotonMessageInfo info = new PhotonMessageInfo(sender, sentTime, view);
                    view.ExecuteOnSerialize(pStream, info);
                }
                else if (view.observed is Transform)
                {
                    Transform tf = (Transform)view.observed;
                    object[] arr = hashData.Dequeue();
                    switch (arr.Length)
                    {
                        case 1:
                            {
                                if (arr[0] == null || !(arr[0] is Vector3 vec))
                                {
                                    myReason = "OSR: Null or Invalid value inside data";
                                    return false;
                                }
                                tf.localPosition = vec;
                            }
                            break;

                        case 2:
                            {
                                if (arr[0] == null || !(arr[0] is Vector3 vec))
                                {
                                    myReason = "OSR: Null or Invalid value inside data";
                                    return false;
                                }
                                if (arr[1] == null || !(arr[1] is Quaternion quat))
                                {
                                    myReason = "OSR: Null or Invalid value inside data";
                                    return false;
                                }
                                tf.localPosition = vec;
                                tf.localRotation = quat;
                                break;
                            }

                        case 3:
                        case 4:
                            {
                                if (arr[0] == null || !(arr[0] is Vector3 vec))
                                {
                                    myReason = "OSR: Null or Invalid value inside data";
                                    return false;
                                }
                                if (arr[1] == null || !(arr[1] is Quaternion quat))
                                {
                                    myReason = "OSR: Null or Invalid value inside data";
                                    return false;
                                }
                                if (arr[2] == null || !(arr[2] is Vector3 vec1))
                                {
                                    myReason = "OSR: Null or Invalid value inside data";
                                    return false;
                                }
                                tf.localPosition = vec;
                                tf.localRotation = quat;
                                tf.localScale = vec1;
                                break;
                            }

                        default:
                            myReason = $"OSR:Invalid data length(Tf)" + arr.Length.ToString();
                            return false;
                    }
                }
                else if (view.observed is Rigidbody)
                {
                    Rigidbody rb = (Rigidbody)view.observed;
                    object[] arr = hashData.Dequeue();
                    switch (arr.Length)
                    {
                        case 1:
                            {
                                if (arr[0] == null || !(arr[0] is Vector3 vec))
                                {
                                    myReason = "OSR: Null or Invalid value inside data";
                                    return false;
                                }
                                rb.velocity = vec;
                                break;
                            }

                        case 2:
                            {
                                if (arr[0] == null || !(arr[0] is Vector3 vec))
                                {
                                    myReason = "OSR: Null or Invalid value inside data";
                                    return false;
                                }
                                if (arr[1] == null || !(arr[1] is Vector3 loc))
                                {
                                    myReason = "OSR: Null or Invalid value inside data";
                                    return false;
                                }
                                rb.velocity = vec;
                                rb.angularVelocity = loc;
                                break;
                            }

                        default:
                            myReason = "OSR:Invalid data length(Rb) " + arr.Length.ToString();
                            return false;
                    }
                }
                else
                {
                    //Log.Spam($"Recieved unknown observed type by [{sender.ID.ToString()}]", "UnkOSR", sender.ID);
                    hashData.Dequeue();
                }
            }
            return true;
        }

        public void OnFailedHandle()
        {
            if (myReason != "")
            {
                //Antis.Response(sender, myReason);
            }
            myReason = "";
        }
    }
}