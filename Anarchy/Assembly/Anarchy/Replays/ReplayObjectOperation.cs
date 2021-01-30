using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.Replays
{
    public enum ReplayObjectOperation : byte
    {
        SpawnHero,
        SpawnHook,
        SpawnTitan,
        SpawnEffect,
        SpawnObject,
        DestroyObject,
        SetHero,
        SetHook,
        SetTitan
    }
}
