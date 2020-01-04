namespace Anarchy.Commands.Chat
{
    internal class ASORacingCommand : ChatCommand
    {
        public ASORacingCommand() : base("asoracing", true, true, false)
        {

        }

        public override bool Execute(string[] args)
        {
            if(FengGameManagerMKII.FGM.Logic.Mode != GameMode.RACING)
            {
                chatMessage = Lang["notRacingMode"];
                return false;
            }
            GameModes.ASORacing.State = !GameModes.ASORacing.State;
            chatMessage = Lang["asorace" + GameModes.ASORacing.State.ToString()];
            SendLocalizedText("asorace" + GameModes.ASORacing.State.ToString(), null);
            return true;
        }
    }
}
