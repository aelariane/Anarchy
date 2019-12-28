using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Saved Option")]
public class UISavedOption : MonoBehaviour
{
    private UICheckbox mCheck;
    private UIPopupList mList;
    public string keyName;

    private string key
    {
        get
        {
            return (!string.IsNullOrEmpty(this.keyName)) ? this.keyName : ("NGUI State: " + base.name);
        }
    }

    private void Awake()
    {
        this.mList = base.GetComponent<UIPopupList>();
        this.mCheck = base.GetComponent<UICheckbox>();
        if (this.mList != null)
        {
            UIPopupList uipopupList = this.mList;
            uipopupList.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Combine(uipopupList.onSelectionChange, new UIPopupList.OnSelectionChange(this.SaveSelection));
        }
        if (this.mCheck != null)
        {
            UICheckbox uicheckbox = this.mCheck;
            uicheckbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Combine(uicheckbox.onStateChange, new UICheckbox.OnStateChange(this.SaveState));
        }
    }

    private void OnDestroy()
    {
        if (this.mCheck != null)
        {
            UICheckbox uicheckbox = this.mCheck;
            uicheckbox.onStateChange = (UICheckbox.OnStateChange)Delegate.Remove(uicheckbox.onStateChange, new UICheckbox.OnStateChange(this.SaveState));
        }
        if (this.mList != null)
        {
            UIPopupList uipopupList = this.mList;
            uipopupList.onSelectionChange = (UIPopupList.OnSelectionChange)Delegate.Remove(uipopupList.onSelectionChange, new UIPopupList.OnSelectionChange(this.SaveSelection));
        }
    }

    private void OnDisable()
    {
        if (this.mCheck == null && this.mList == null)
        {
            UICheckbox[] componentsInChildren = base.GetComponentsInChildren<UICheckbox>(true);
            int i = 0;
            int num = componentsInChildren.Length;
            while (i < num)
            {
                UICheckbox uicheckbox = componentsInChildren[i];
                if (uicheckbox.isChecked)
                {
                    this.SaveSelection(uicheckbox.name);
                    break;
                }
                i++;
            }
        }
    }

    private void OnEnable()
    {
        if (this.mList != null)
        {
            string @string = PlayerPrefs.GetString(this.key);
            if (!string.IsNullOrEmpty(@string))
            {
                this.mList.selection = @string;
            }
            return;
        }
        if (this.mCheck != null)
        {
            this.mCheck.isChecked = (PlayerPrefs.GetInt(this.key, 1) != 0);
        }
        else
        {
            string string2 = PlayerPrefs.GetString(this.key);
            UICheckbox[] componentsInChildren = base.GetComponentsInChildren<UICheckbox>(true);
            int i = 0;
            int num = componentsInChildren.Length;
            while (i < num)
            {
                UICheckbox uicheckbox = componentsInChildren[i];
                uicheckbox.isChecked = (uicheckbox.name == string2);
                i++;
            }
        }
    }

    private void SaveSelection(string selection)
    {
        PlayerPrefs.SetString(this.key, selection);
    }

    private void SaveState(bool state)
    {
        PlayerPrefs.SetInt(this.key, (!state) ? 0 : 1);
    }
}