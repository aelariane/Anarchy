namespace Anarchy.Commands.Chat
{
    public class ASORacingCommand : ChatCommand
    {
        public ASORacingCommand() : base("asoracing", true, true, false)
        {
        }

        public override bool Execute(string[] args)
        {
            if (FengGameManagerMKII.FGM.logic.Mode != GameMode.Racing)
            {
                chatMessage = Lang["notRacingMode"];
                return false;
            }
            GameModes.AsoRacing.State = !GameModes.AsoRacing.State;
            chatMessage = Lang["asorace" + GameModes.AsoRacing.State.ToString()];
            SendLocalizedText("asorace" + GameModes.AsoRacing.State.ToString(), null);
            return true;
        }
    }
}