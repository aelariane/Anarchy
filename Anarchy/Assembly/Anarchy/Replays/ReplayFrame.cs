using System.Collections.Generic;

using UnityEngine;

namespace Anarchy.Replays
{
    public class ReplayFrame
    {
        private List<PositionUpdateInformation> positionUpdates = new List<PositionUpdateInformation>();

        public Vector3 CameraPosition { get; private set; }
        public Quaternion CameraRotation { get; private set; }
        public Vector3 CursorLocalPosition { get; private set; }
        public IEnumerable<PositionUpdateInformation> PositionUpdates => positionUpdates;

        public void RegisterCameraPosition(Vector3 cursorLocalPosition, Vector3 cameraPosition, Quaternion cameraRotation)
        {
            CursorLocalPosition = cursorLocalPosition;
            CameraPosition = cameraPosition;
            CameraRotation = cameraRotation;
        }

        public void RegisterPositionUpdate(PositionUpdateInformation info)
        {
            positionUpdates.Add(info);
        }

        ////Serializes all position updates happened in the frame
        //public byte[] Serialize()
        //{
        //    List<byte> bytes = new List<byte>();

        //    byte[] countBytes = new byte[2];
        //    int index = 0;
        //    ExitGames.Client.Photon.Protocol.Serialize((short)positionUpdates.Count, countBytes, ref index);
        //    foreach(var upd in positionUpdates)
        //    {
        //        bytes.AddRange(upd.Serialize());
        //    }

        //    bytes.Add(ConstBytes.FrameCameraUpdate);

        //    byte[] vecBytes = new byte[12];

        //    index = 0;
        //    ExitGames.Client.Photon.Protocol.Serialize(CursorLocalPosition.x, vecBytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Serialize(CursorLocalPosition.y, vecBytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Serialize(CursorLocalPosition.z, vecBytes, ref index);
        //    bytes.AddRange(vecBytes);

        //    index = 0;
        //    ExitGames.Client.Photon.Protocol.Serialize(CameraPosition.x, vecBytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Serialize(CameraPosition.y, vecBytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Serialize(CameraPosition.z, vecBytes, ref index);
        //    bytes.AddRange(vecBytes);

        //    index = 0;
        //    var quatBytes = new byte[16];
        //    ExitGames.Client.Photon.Protocol.Serialize(CameraRotation.x, quatBytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Serialize(CameraRotation.y, quatBytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Serialize(CameraRotation.z, quatBytes, ref index);
        //    ExitGames.Client.Photon.Protocol.Serialize(CameraRotation.w, quatBytes, ref index);
        //    bytes.AddRange(quatBytes);

        //    bytes.Add(ConstBytes.EndOfFrame);

        //    return bytes.ToArray();
        //}

        //public void Process()
        //{
        //    //foreach(var posUp in positionUpdates)
        //    //{
        //    //}
        //}
    }
}