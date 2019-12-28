using System;
using System.Collections;

public class IComparerPVPchkPtID : IComparer
{
    int IComparer.Compare(object x, object y)
    {
        float num = (float)((PVPcheckPoint)x).id;
        float num2 = (float)((PVPcheckPoint)y).id;
        if (num == num2 || Math.Abs(num - num2) < 1.401298E-45f)
        {
            return 0;
        }
        if (num < num2)
        {
            return -1;
        }
        return 1;
    }
}