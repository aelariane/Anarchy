using RC;
using System.Collections.Generic;

public class RCEvent
{
    private RCCondition condition;

    private RCAction elseAction;

    private int eventClass;

    private int eventType;

    public string foreachVariableName;

    public List<RCAction> trueActions;

    public RCEvent(RCCondition sentCondition, List<RCAction> sentTrueActions, int sentClass, int sentType)
    {
        this.condition = sentCondition;
        this.trueActions = sentTrueActions;
        this.eventClass = sentClass;
        this.eventType = sentType;
    }

    public void checkEvent()
    {
        switch (this.eventClass)
        {
            case 0:
                for (int num2 = 0; num2 < this.trueActions.Count; num2++)
                {
                    this.trueActions[num2].doAction();
                }
                return;

            case 1:
                if (this.condition.checkCondition())
                {
                    for (int num3 = 0; num3 < this.trueActions.Count; num3++)
                    {
                        this.trueActions[num3].doAction();
                    }
                    return;
                }
                if (this.elseAction != null)
                {
                    this.elseAction.doAction();
                    return;
                }
                return;

            case 2:
                {
                    int i = this.eventType;
                    if (i == 0)
                    {
                        foreach (TITAN titan2 in FengGameManagerMKII.Titans)
                        {
                            TITAN titan = (TITAN)titan2;
                            if (RCManager.titanVariables.ContainsKey(this.foreachVariableName))
                            {
                                RCManager.titanVariables[this.foreachVariableName] = titan;
                            }
                            else
                            {
                                RCManager.titanVariables.Add(this.foreachVariableName, titan);
                            }
                            foreach (RCAction rcaction in this.trueActions)
                            {
                                rcaction.doAction();
                            }
                        }
                        return;
                    }
                    if (i != 1)
                    {
                        return;
                    }
                    foreach (PhotonPlayer player in PhotonNetwork.playerList)
                    {
                        if (RCManager.playerVariables.ContainsKey(this.foreachVariableName))
                        {
                            RCManager.playerVariables[this.foreachVariableName] = player;
                        }
                        else
                        {
                            RCManager.titanVariables.Add(this.foreachVariableName, player);
                        }
                        foreach (RCAction rcaction2 in this.trueActions)
                        {
                            rcaction2.doAction();
                        }
                    }
                    return;
                }
            case 3:
                while (this.condition.checkCondition())
                {
                    foreach (RCAction rcaction3 in this.trueActions)
                    {
                        rcaction3.doAction();
                    }
                }
                return;

            default:
                return;
        }
    }

    public void setElse(RCAction sentElse)
    {
        this.elseAction = sentElse;
    }

    public enum foreachType
    {
        titan,
        player
    }

    public enum loopType
    {
        noLoop,
        ifLoop,
        foreachLoop,
        whileLoop
    }
}