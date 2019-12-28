using ExitGames.Client.Photon;
using System;

namespace Optimization
{
    public class AOTEventArgs : EventArgs
    {
        public static new readonly AOTEventArgs Empty = new AOTEventArgs();
        public readonly object[] Data;

        public string DebugMessage
        {
            get
            {
                foreach (object obj in Data)
                {
                    if (obj is string msg)
                        return msg;
                }
                return string.Empty;
            }
        }

        public DisconnectCause DisconnectCause
        {
            get
            {
                foreach (object obj in Data)
                {
                    if (obj is DisconnectCause cause)
                        return cause;
                }
                return DisconnectCause.DisconnectByServerLogic;
            }
        }

        public Hashtable Hashtable
        {

            get
            {
                foreach (object obj in Data)
                {
                    if (obj is Hashtable hash)
                        return hash;
                }
                return null;
            }
        }

        public OperationResponse OpResponse
        {
            get
            {
                foreach(object obj in Data)
                {
                    if (obj is OperationResponse op)
                        return op;
                }
                return null;
            }
        }

        public PhotonPlayer Player
        {
            get
            {
                foreach(object obj in Data)
                {
                    if (obj is PhotonPlayer player)
                        return player;
                }
                return null;
            }
        }

        public short ReturnCode
        {
            get
            {
                foreach(object obj in Data)
                {
                    if(obj is short sh)
                    {
                        return sh;
                    }
                }
                return 0;
            }
        }

        public AOTEventArgs()
        {
            Data = new object[0];
        }

        public AOTEventArgs(object[] data)
        {
            Data = data;
        }
    }
}
