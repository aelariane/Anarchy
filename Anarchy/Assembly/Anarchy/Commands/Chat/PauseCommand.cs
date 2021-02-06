namespace Anarchy.Commands.Chat
{
    internal class PauseCommand : ChatCommand
    {
        private readonly bool value;
        private readonly string sendString;

        public PauseCommand(bool pause) : base(pause ? "pause" : "unpause", true, true, false)
        {
            value = pause;
            sendString = English.Get("pause" + value.ToString());
        }

        public override bool Execute(string[] args)
        {
            //if(value && UnityEngine.Time.timeScale <= 0.1f)
            //{
            //    chatMessage = Lang["pauseErr"];
            //    return false;
            //}
            //else if(!value && UnityEngine.Time.timeScale >= 1f)
            //{
            //    chatMessage = Lang["unpauseErr"];
            //    return false;
            //}
            if (value && AnarchyManager.PauseWindow.IsActive)
            {
                chatMessage = Lang["pauseErr"];
                return false;
            }
            else if (!value && !AnarchyManager.PauseWindow.IsActive)
            {
                chatMessage = Lang["unpauseErr"];
                return false;
            }
            chatMessage = Lang["pause" + value.ToString()];
            FengGameManagerMKII.FGM.BasePV.RPC("pauseRPC", PhotonTargets.All, new object[] { value });
            SendLocalizedText("pause" + value.ToString(), null);
            return true;
        }
    }
}