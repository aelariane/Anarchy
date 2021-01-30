using Anarchy.Configuration;

namespace Anarchy.VoiceChat
{
    internal static class VCManager
    {
        internal static BoolSetting Disconnected = new BoolSetting("VCDisconnected", false);
        internal static IntSetting BackgroundTransparency = new IntSetting("VCVolumeMultiplier", 0);
        internal static KeySetting PushToTalk = new KeySetting("PushToTalk", UnityEngine.KeyCode.None);
        internal const int Mute = 254;
        internal const int Unmute = 255;

        internal static void SendHasVoice(int ID)
        {
            PhotonNetwork.networkingPeer.OpRaiseEvent(173, new byte[0], true, new RaiseEventOptions { TargetActors = new int[] { ID } });
        }

        internal static void SendMute(int ID)
        {
            PhotonNetwork.networkingPeer.OpRaiseEvent(173, new byte[] { Mute }, true, new RaiseEventOptions { TargetActors = new int[] { ID } });
        }

        internal static void SendUnmute(int ID)
        {
            PhotonNetwork.networkingPeer.OpRaiseEvent(173, new byte[] { Unmute }, true, new RaiseEventOptions { TargetActors = new int[] { ID } });
        }
    }
}