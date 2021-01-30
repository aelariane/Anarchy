using RC;
using UnityEngine;

public class RegionTrigger : MonoBehaviour
{
    public string myName;

    public RCEvent playerEventEnter;

    public RCEvent playerEventExit;

    public RCEvent titanEventEnter;

    public RCEvent titanEventExit;

    public void CopyTrigger(RegionTrigger copyTrigger)
    {
        this.playerEventEnter = copyTrigger.playerEventEnter;
        this.titanEventEnter = copyTrigger.titanEventEnter;
        this.playerEventExit = copyTrigger.playerEventExit;
        this.titanEventExit = copyTrigger.titanEventExit;
        this.myName = copyTrigger.myName;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject gameObject = other.transform.gameObject;
        if (gameObject.layer == 8)
        {
            if (this.playerEventEnter != null)
            {
                HERO component = gameObject.GetComponent<HERO>();
                if (component != null)
                {
                    string str = (string)RCManager.RCVariableNames["OnPlayerEnterRegion[" + this.myName + "]"];
                    if (RCManager.playerVariables.ContainsKey(str))
                    {
                        RCManager.playerVariables[str] = component.BasePV.owner;
                    }
                    else
                    {
                        RCManager.playerVariables.Add(str, component.BasePV.owner);
                    }
                    this.playerEventEnter.checkEvent();
                    return;
                }
            }
        }
        else if (gameObject.layer == 11 && this.titanEventEnter != null)
        {
            TITAN titan = gameObject.transform.root.gameObject.GetComponent<TITAN>();
            if (titan != null)
            {
                string str2 = (string)RCManager.RCVariableNames["OnTitanEnterRegion[" + this.myName + "]"];
                if (RCManager.titanVariables.ContainsKey(str2))
                {
                    RCManager.titanVariables[str2] = titan;
                }
                else
                {
                    RCManager.titanVariables.Add(str2, titan);
                }
                this.titanEventEnter.checkEvent();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject gameObject = other.transform.root.gameObject;
        if (gameObject.layer == 8)
        {
            if (this.playerEventExit != null)
            {
                HERO component = gameObject.GetComponent<HERO>();
                if (component != null)
                {
                    string str = (string)RCManager.RCVariableNames["OnPlayerLeaveRegion[" + this.myName + "]"];
                    if (RCManager.playerVariables.ContainsKey(str))
                    {
                        RCManager.playerVariables[str] = component.BasePV.owner;
                        return;
                    }
                    RCManager.playerVariables.Add(str, component.BasePV.owner);
                    return;
                }
            }
        }
        else if (gameObject.layer == 11 && this.titanEventExit != null)
        {
            TITAN titan = gameObject.GetComponent<TITAN>();
            if (titan != null)
            {
                string str2 = (string)RCManager.RCVariableNames["OnTitanLeaveRegion[" + this.myName + "]"];
                if (RCManager.titanVariables.ContainsKey(str2))
                {
                    RCManager.titanVariables[str2] = titan;
                }
                else
                {
                    RCManager.titanVariables.Add(str2, titan);
                }
                this.titanEventExit.checkEvent();
            }
        }
    }
}