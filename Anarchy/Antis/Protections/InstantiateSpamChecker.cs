using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Antis.Protections
{
    public class InstantiateSpamChecker : IProtection<string>
    {
        public bool Check(string rpcName)
        {
            return true;
        }

        bool IProtection.Check(object rpcName)
        {
            return true;
        }
    }
}
