namespace Anarchy.Configuration
{
    public class AnarchyGameModeSetting : GameModeSetting
    {
        public AnarchyGameModeSetting(string key) : this(key, -1, null, null)
        {
        }

        public AnarchyGameModeSetting(string key, int sel) : this(key, sel, null, null)
        {
        }

        public AnarchyGameModeSetting(string key, float[] floats) : this(key, -1, floats, null)
        {
        }

        public AnarchyGameModeSetting(string key, int[] ints) : this(key, -1, null, ints)
        {
        }

        public AnarchyGameModeSetting(string key, int sel, float[] floats) : this(key, sel, floats, null)
        {
        }

        public AnarchyGameModeSetting(string key, int sel, int[] ints) : this(key, sel, null, ints)
        {
        }

        public AnarchyGameModeSetting(string key, int selection, float[] floats, int[] ints) : base(key, selection, floats, ints)
        {
            RemoveChangedCallback(RCSettingCallback);
            AddChangedCallback(AnarchySettingCallback);
        }

        public static void AnarchySettingCallback(GameModeSetting set, bool state, bool received)
        {
            if (PhotonNetwork.IsMasterClient && !received)
            {
                PhotonPlayer[] targets = PhotonPlayer.GetNotAnarchyUsers();
                if (targets.Length <= 0)
                {
                    return;
                }
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", targets, new object[] { set.ToString(), string.Empty });
            }
        }
    }
}