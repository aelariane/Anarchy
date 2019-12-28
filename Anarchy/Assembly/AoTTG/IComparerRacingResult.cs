using System;
using System.Collections;

public class IComparerRacingResult : IComparer
{
    int IComparer.Compare(object x, object y)
    {
        float time = ((RacingResult)x).Time;
        float time2 = ((RacingResult)y).Time;
        if (time == time2 || Math.Abs(time - time2) < 1.401298E-45f)
        {
            return 0;
        }
        if (time < time2)
        {
            return -1;
        }
        return 1;
    }
}