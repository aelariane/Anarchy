using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public class BTN_FORGET_PASSWORD : MonoBehaviour
{
    private bool invalid;
    public GameObject email;

    public GameObject logincomponent;

    public GameObject output;

    private string DomainMapper(Match match)
    {
        IdnMapping idnMapping = new IdnMapping();
        string text = match.Groups[2].Value;
        try
        {
            text = idnMapping.GetAscii(text);
        }
        catch (ArgumentException)
        {
            this.invalid = true;
        }
        return match.Groups[1].Value + text;
    }

    private void OnClick()
    {
        if (!this.IsValidEmail(this.email.GetComponent<UIInput>().text))
        {
            this.output.GetComponent<UILabel>().text = "This e-mail address is not valid.";
            return;
        }
        this.logincomponent.GetComponent<LoginFengKAI>().resetPassword(this.email.GetComponent<UIInput>().text);
        this.output.GetComponent<UILabel>().text = "please wait...";
    }

    public bool IsValidEmail(string strIn)
    {
        this.invalid = false;
        if (string.IsNullOrEmpty(strIn))
        {
            return false;
        }
        strIn = Regex.Replace(strIn, "(@)(.+)$", new MatchEvaluator(this.DomainMapper));
        return !this.invalid && Regex.IsMatch(strIn, "^(?(\")(\"[^\"]+?\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9]{2,17}))$", RegexOptions.IgnoreCase);
    }
}