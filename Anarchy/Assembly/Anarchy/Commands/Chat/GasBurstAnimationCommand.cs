using System.Linq;

namespace Anarchy.Commands.Chat
{
    class GasBurstAnimationCommand : ChatCommand
    {
        public GasBurstAnimationCommand() : base("gas", false, false, true)
        {

        }

        public override bool Execute(string[] args)
        {
            if (args.Length <= 0)
            {
                logMessage = Lang["errArg"];
                return false;
            }
            HERO myHero = FengGameManagerMKII.Heroes.FirstOrDefault(x => x.IsLocal);
            if(myHero == null)
            {
                logMessage = "Spawn your HERO first!";
                return false;
            }
            switch(args[0].ToLower())
            {
                case "help":
                    logMessage += "Available options:\ngas, cross(2), meat(2), ";
                    break;
                case "default":
                case "gas":
                    myHero.GasBurstAnimation = "FX/boost_smoke";
                    break;
                case "cross":
                    myHero.GasBurstAnimation = "redCross";
                    break;
                case "cross2":
                    myHero.GasBurstAnimation = "redCross1";
                    break;
                case "meat":
                    myHero.GasBurstAnimation = "hitMeat";
                    break;
                case "meat2":
                    myHero.GasBurstAnimation = "hitMeat2";
                    break;
                //  Will probably be annoying / abusive for weak PC user
                //case "splatter":   
                //    break;
                case "blood":
                    myHero.GasBurstAnimation = "bloodExplore";
                    break;
                //abusive too
                //case "thunder":
                //    myHero.GasBurstAnimation = "FX/Thunder";
                //    break;
                default:
                    logMessage = Lang["errArg"];
                    return false;
            }

            return true;
        }
    }
}
