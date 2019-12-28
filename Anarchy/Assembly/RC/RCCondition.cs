using System;

public class RCCondition
{
    private int operand;

    private RCActionHelper parameter1;

    private RCActionHelper parameter2;

    private int type;

    public RCCondition(int sentOperand, int sentType, RCActionHelper sentParam1, RCActionHelper sentParam2)
    {
        this.operand = sentOperand;
        this.type = sentType;
        this.parameter1 = sentParam1;
        this.parameter2 = sentParam2;
    }

    private bool boolCompare(bool baseBool, bool compareBool)
    {
        int num = this.operand;
        if (num != 2)
        {
            return num == 5 && baseBool != compareBool;
        }
        return baseBool == compareBool;
    }

    public bool checkCondition()
    {
        switch (this.type)
        {
            case 0:
                return this.intCompare(this.parameter1.returnInt(null), this.parameter2.returnInt(null));
            case 1:
                return this.boolCompare(this.parameter1.returnBool(null), this.parameter2.returnBool(null));
            case 2:
                return this.stringCompare(this.parameter1.returnString(null), this.parameter2.returnString(null));
            case 3:
                return this.floatCompare(this.parameter1.returnFloat(null), this.parameter2.returnFloat(null));
            case 4:
                return this.playerCompare(this.parameter1.returnPlayer(null), this.parameter2.returnPlayer(null));
            case 5:
                return this.titanCompare(this.parameter1.returnTitan(null), this.parameter2.returnTitan(null));
            default:
                return false;
        }
    }

    private bool floatCompare(float baseFloat, float compareFloat)
    {
        switch (this.operand)
        {
            case 0:
                return baseFloat < compareFloat;
            case 1:
                return baseFloat <= compareFloat;
            case 2:
                return baseFloat == compareFloat;
            case 3:
                return baseFloat >= compareFloat;
            case 4:
                return baseFloat > compareFloat;
            case 5:
                return baseFloat != compareFloat;
            default:
                return false;
        }
    }

    private bool intCompare(int baseInt, int compareInt)
    {
        switch (this.operand)
        {
            case 0:
                return baseInt < compareInt;
            case 1:
                return baseInt <= compareInt;
            case 2:
                return baseInt == compareInt;
            case 3:
                return baseInt >= compareInt;
            case 4:
                return baseInt > compareInt;
            case 5:
                return baseInt != compareInt;
            default:
                return false;
        }
    }

    private bool playerCompare(PhotonPlayer basePlayer, PhotonPlayer comparePlayer)
    {
        int num = this.operand;
        if (num != 2)
        {
            return num == 5 && basePlayer != comparePlayer;
        }
        return basePlayer == comparePlayer;
    }

    private bool stringCompare(string baseString, string compareString)
    {
        switch (this.operand)
        {
            case 0:
                return baseString == compareString;
            case 1:
                return baseString != compareString;
            case 2:
                return baseString.Contains(compareString);
            case 3:
                return !baseString.Contains(compareString);
            case 4:
                return baseString.StartsWith(compareString);
            case 5:
                return !baseString.StartsWith(compareString);
            case 6:
                return baseString.EndsWith(compareString);
            case 7:
                return !baseString.EndsWith(compareString);
            default:
                return false;
        }
    }

    private bool titanCompare(TITAN baseTitan, TITAN compareTitan)
    {
        int num = this.operand;
        if (num != 2)
        {
            return num == 5 && baseTitan != compareTitan;
        }
        return baseTitan == compareTitan;
    }

    public enum castTypes
    {
        typeInt,
        typeBool,
        typeString,
        typeFloat,
        typePlayer,
        typeTitan
    }

    public enum operands
    {
        lt,
        lte,
        e,
        gte,
        gt,
        ne
    }

    public enum stringOperands
    {
        equals,
        notEquals,
        contains,
        notContains,
        startsWith,
        notStartsWith,
        endsWith,
        notEndsWith
    }
}
