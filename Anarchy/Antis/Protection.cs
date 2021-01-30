using Antis.Protections;

namespace Antis
{
    public static class Protection
    {
        public static IProtection<string> HeroAnimationCheck = new HeroAnimationChecker();
        public static IProtection<string> InstantiationNameCheck = new EmptyProtection<string>();
        public static IProtection<string> InstantiationSpamCheck = new EmptyProtection<string>();
        public static IProtection<string> RPCNameCheck = new EmptyProtection<string>();
        public static IProtection<string> RPCSpamheck = new EmptyProtection<string>();
        public static IProtection<string[]> SkinsCheck = new EmptyProtection<string[]>();
        public static IProtection<string> TitanAnimationCheck = new TitanAnimationChecker();
        public static IProtection<string> URLCheck = new URLChecker();
    }
}