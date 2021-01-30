using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Replays
{
    public struct ObjectOperationInformation
    {
        public ReplayObjectOperation OperationType { get; }
        public object[] Arguments { get; }
        public float TimeStamp { get; }

        public ObjectOperationInformation(ReplayObjectOperation opType, object[] args, float time)
        {
            OperationType = opType;
            Arguments = args;
            TimeStamp = time;
        }
    }
}
