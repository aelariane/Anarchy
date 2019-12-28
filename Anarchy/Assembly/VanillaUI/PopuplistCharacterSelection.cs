using UnityEngine;

public class PopuplistCharacterSelection : MonoBehaviour
{
    public GameObject ACL;
    public GameObject BLA;
    public GameObject GAS;
    public GameObject SPD;

    private void onCharacterChange()
    {
        string selection = base.GetComponent<UIPopupList>().selection;
        HeroStat heroStat;
        if (selection == "Set 1" || selection == "Set 2" || selection == "Set 3")
        {
            HeroCostume heroCostume = CostumeConeveter.LocalDataToHeroCostume(selection.ToUpper());
            if (heroCostume == null)
            {
                heroStat = new HeroStat();
            }
            else
            {
                heroStat = heroCostume.stat;
            }
        }
        else
        {
            heroStat = HeroStat.getInfo(base.GetComponent<UIPopupList>().selection);
        }
        this.SPD.transform.localScale = new Vector3((float)heroStat.Spd, 20f, 0f);
        this.GAS.transform.localScale = new Vector3((float)heroStat.Gas, 20f, 0f);
        this.BLA.transform.localScale = new Vector3((float)heroStat.Bla, 20f, 0f);
        this.ACL.transform.localScale = new Vector3((float)heroStat.Acl, 20f, 0f);
    }
}