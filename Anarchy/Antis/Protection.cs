using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antis.Protections;

namespace Antis
{
    public static class Protection
    {
        public static IProtection<string> HeroAnimationCheck = new HeroAnimationChecker();
        public static IProtection<string> InstantioationCheck = new EmptyProtection<string>();
        public static IProtection<string> RPCNameCheck = new EmptyProtection<string>();
        public static IProtection<string[]> SkinsCheck = new EmptyProtection<string[]>();
        public static IProtection<string> TitanAnimationCheck = new TitanAnimationChecker();
        public static IProtection<string> URLCheck = new URLChecker();

    }
}
