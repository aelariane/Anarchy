using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Anarchy.Replays
{
    public struct PositionUpdateInformation
    {
        public int Id { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public PositionUpdateInformation(int id, Vector3 pos, Quaternion rot)
        {
            Id = id;
            Position = pos;
            Rotation = rot;
        }

        //public byte[] Serialize()
        //{
        //    var positionBytes = new byte[32];
        //    int index = 0;

        //    ExitGames.Client.Photon.Protocol.Serialize(Id, positionBytes, ref index);

        //    ExitGames.Client.Photon.Protocol.Serialize(Position.x, positionBytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Serialize(Position.y, positionBytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Serialize(Position.z, positionBytes, ref index);

        //    ExitGames.Client.Photon.Protocol.Serialize(Rotation.x, positionBytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Serialize(Rotation.y, positionBytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Serialize(Rotation.z, positionBytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Serialize(Rotation.w, positionBytes, ref index);

        //    return positionBytes;
        //}

        //public static PositionUpdateInformation FromBytes(byte[] bytes, ref int index)
        //{
        //    int id;
        //    Vector3 vec;
        //    Quaternion rotation;

        //    ExitGames.Client.Photon.Protocol.Deserialize(out id, bytes, ref index);

        //    ExitGames.Client.Photon.Protocol.Deserialize(out vec.x, bytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Deserialize(out vec.y, bytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Deserialize(out vec.z, bytes, ref index);

        //    ExitGames.Client.Photon.Protocol.Deserialize(out rotation.x, bytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Deserialize(out rotation.y, bytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Deserialize(out rotation.z, bytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Deserialize(out rotation.w, bytes, ref index);

        //    return new PositionUpdateInformation(id, vec, rotation);
        //}
    }
}
