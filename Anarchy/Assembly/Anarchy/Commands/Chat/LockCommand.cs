using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Commands.Chat
{
    internal class LockCommand : ChatCommand
    {
        internal static bool locker = false;

        public LockCommand() : base("lock", false, false, true)
        {

        }

        public override bool Execute(string[] args)
        {
            locker = !locker;
            return true;
        }
    }
}
