using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Antis.Spam;
using Anarchy.UI;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Anarchy.Network.Events
{
    public class EventRPC : INetworkEvent
    {

        private string name = null;
        private object[] parameters;
        private short prefix;
        private PhotonPlayer sender;
        private int sTime;
        private int viewID;

        public byte Code { get; }

        public EventRPC()
        {
            Code = 200;
            NetworkManager.RegisterEvent(this);
        }


        public bool CheckData(EventData data, PhotonPlayer sender, out string reason)
        {
            reason = "";
            this.sender = sender;
            Hashtable hash = data[245] as Hashtable;
            if (hash == null)
            {
                reason += Log.GetString("notHashOrNull");
                return false;
            }
            if(hash.Count < 3 || hash.Count > 5)
            {
                reason += Log.GetString("invalidParamsCount", hash.Count.ToString());
                return false;
            }
            if(!CheckKey(hash, 0, out viewID))
            {
                reason += Log.GetString("missOrInvalidKey", "0");
                return false;
            }
            if(!CheckKey(hash, 2, out sTime))
            {
                reason += Log.GetString("missOrInvalidKey", "2");
                return false;
            }
            if(hash.ContainsKey((byte)3) && hash.ContainsKey((byte)5))
            {
                reason += Log.GetString("bothNameKeys");
                return false;
            }
            if(!CheckKey(hash, 3, out name))
            {
                if(CheckKey(hash, 5, out byte byteVal))
                {
                    if (PhotonNetwork.PhotonServerSettings.RpcList.Count <= byteVal)
                    {
                        reason += Log.GetString("outOfListRPC", "5");
                        return false;
                    }
                    name = PhotonNetwork.PhotonServerSettings.RpcList[byteVal];
                }
                else
                {
                    reason += Log.GetString("missRpcName");
                    return false;
                }
            }
            if(name == null)
            {
                reason += Log.GetString("nullRpcName");
                return false;
            }
            sender.RPCSpam.Count(name);
            if (!CheckKey(hash, 1, out prefix, true))
            {
                reason += Log.GetString("invalidKey", "1");
                return false;
            }
            if (hash.ContainsKey((byte)4))
            {
                if (!(hash[(byte)4] is object[]))
                {

                    reason += Log.GetString("invalidKey", "4");
                    return false;
                }
                parameters = (object[])hash[(byte)4];
            }
            else parameters = new object[0];
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
            PhotonView photonView = PhotonView.Find(viewID);
            if(photonView == null)
            {
                return true;
            }
            if ((photonView.group != 0) && !PhotonNetwork.networkingPeer.allowedReceivingGroups.Contains(photonView.group))
            {
                return true;
            }
            System.Type[] callParameterTypes = new System.Type[0];
            if (parameters.Length > 0)
            {
                callParameterTypes = new System.Type[parameters.Length];
                int index = 0;
                for (int i = 0; i < parameters.Length; i++)
                {
                    object obj2 = parameters[i];
                    if (obj2 == null)
                    {
                        callParameterTypes[index] = null;
                    }
                    else
                    {
                        callParameterTypes[index] = obj2.GetType();
                    }
                    index++;
                }
            }
            int num7 = 0;
            int num8 = 0;
            foreach (MonoBehaviour behaviour in photonView.GetComponents<MonoBehaviour>())
            {
                if (behaviour == null)
                {
                    Debug.LogError("ERROR You have missing MonoBehaviours on your gameobjects!");
                }
                else
                {
                    System.Type key = behaviour.GetType();
                    List<MethodInfo> list = null;
                    if (PhotonNetwork.networkingPeer.monoRPCMethodsCache.ContainsKey(key))
                    {
                        list = PhotonNetwork.networkingPeer.monoRPCMethodsCache[key];
                    }
                    if (list == null)
                    {
                        List<MethodInfo> methods = SupportClass.GetMethods(key, typeof(UnityEngine.RPC));
                        PhotonNetwork.networkingPeer.monoRPCMethodsCache[key] = methods;
                        list = methods;
                    }
                    if (list != null)
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            MethodInfo info = list[j];
                            if (info.Name == name)
                            {
                                num8++;
                                ParameterInfo[] methodParameters = info.GetParameters();
                                if (methodParameters.Length == callParameterTypes.Length)
                                {
                                    if (PhotonNetwork.networkingPeer.CheckTypeMatch(methodParameters, callParameterTypes))
                                    {
                                        num7++;
                                        object obj3 = info.Invoke(behaviour, parameters);
                                        if (info.ReturnType == typeof(IEnumerator))
                                        {
                                            behaviour.StartCoroutine((IEnumerator)obj3);
                                        }
                                    }
                                    return true;
                                }
                                else if ((methodParameters.Length - 1) == callParameterTypes.Length)
                                {
                                    if (PhotonNetwork.networkingPeer.CheckTypeMatch(methodParameters, callParameterTypes) && (methodParameters[methodParameters.Length - 1].ParameterType == typeof(PhotonMessageInfo)))
                                    {
                                        num7++;
                                        object[] array = new object[parameters.Length + 1];
                                        parameters.CopyTo(array, 0);
                                        array[array.Length - 1] = new PhotonMessageInfo(sender, sTime, photonView);
                                        object obj4 = info.Invoke(behaviour, array);
                                        if (info.ReturnType == typeof(IEnumerator))
                                        {
                                            behaviour.StartCoroutine((IEnumerator)obj4);
                                        }
                                    }
                                    return true;
                                }
                                else if ((methodParameters.Length == 1) && methodParameters[0].ParameterType.IsArray)
                                {
                                    num7++;
                                    object[] objArray5 = new object[] { parameters };
                                    object obj5 = info.Invoke(behaviour, objArray5);
                                    if (info.ReturnType == typeof(IEnumerator))
                                    {
                                        behaviour.StartCoroutine((IEnumerator)obj5);
                                    }
                                    return true;
                                }
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void OnFailedHandle()
        {
        }
    }
}
