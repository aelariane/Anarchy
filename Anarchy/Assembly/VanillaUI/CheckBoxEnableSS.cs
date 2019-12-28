using UnityEngine;

public class CheckBoxEnableSS : MonoBehaviour
{
    private bool init;

    private void OnActivate(bool yes)
    {
        if (!this.init)
        {
            return;
        }
        if (yes)
        {
            PlayerPrefs.SetInt("EnableSS", 1);
        }
        else
        {
            PlayerPrefs.SetInt("EnableSS", 0);
        }
    }

    private void Start()
    {
        this.init = true;
        if (PlayerPrefs.HasKey("EnableSS"))
        {
            if (PlayerPrefs.GetInt("EnableSS") == 1)
            {
                base.GetComponent<UICheckbox>().isChecked = true;
            }
            else
            {
                base.GetComponent<UICheckbox>().isChecked = false;
            }
        }
        else
        {
            base.GetComponent<UICheckbox>().isChecked = true;
            PlayerPrefs.SetInt("EnableSS", 1);
        }
    }
}