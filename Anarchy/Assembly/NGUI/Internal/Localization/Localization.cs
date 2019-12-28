using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Localization")]
public class Localization : MonoBehaviour
{
    private static Localization mInstance;
    private Dictionary<string, string> mDictionary = new Dictionary<string, string>();
    private string mLanguage;
    public TextAsset[] languages;
    public string startingLanguage = "English";

    public static Localization instance
    {
        get
        {
            if (Localization.mInstance == null)
            {
                Localization.mInstance = (UnityEngine.Object.FindObjectOfType(typeof(Localization)) as Localization);
                if (Localization.mInstance == null)
                {
                    GameObject gameObject = new GameObject("_Localization");
                    UnityEngine.Object.DontDestroyOnLoad(gameObject);
                    Localization.mInstance = gameObject.AddComponent<Localization>();
                }
            }
            return Localization.mInstance;
        }
    }

    public static bool isActive
    {
        get
        {
            return Localization.mInstance != null;
        }
    }

    public string currentLanguage
    {
        get
        {
            return this.mLanguage;
        }
        set
        {
            if (this.mLanguage != value)
            {
                this.startingLanguage = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if (this.languages != null)
                    {
                        int i = 0;
                        int num = this.languages.Length;
                        while (i < num)
                        {
                            TextAsset textAsset = this.languages[i];
                            if (textAsset != null && textAsset.name == value)
                            {
                                this.Load(textAsset);
                                return;
                            }
                            i++;
                        }
                    }
                    TextAsset textAsset2 = Resources.Load(value, typeof(TextAsset)) as TextAsset;
                    if (textAsset2 != null)
                    {
                        this.Load(textAsset2);
                        return;
                    }
                }
                this.mDictionary.Clear();
                PlayerPrefs.DeleteKey("Language");
            }
        }
    }

    private void Awake()
    {
        if (Localization.mInstance == null)
        {
            Localization.mInstance = this;
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
            this.currentLanguage = PlayerPrefs.GetString("Language", this.startingLanguage);
            if (string.IsNullOrEmpty(this.mLanguage) && this.languages != null && this.languages.Length > 0)
            {
                this.currentLanguage = this.languages[0].name;
            }
        }
        else
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    private void Load(TextAsset asset)
    {
        this.mLanguage = asset.name;
        PlayerPrefs.SetString("Language", this.mLanguage);
        ByteReader byteReader = new ByteReader(asset);
        this.mDictionary = byteReader.ReadDictionary();
        UIRoot.Broadcast("OnLocalize", this);
    }

    private void OnDestroy()
    {
        if (Localization.mInstance == this)
        {
            Localization.mInstance = null;
        }
    }

    private void OnEnable()
    {
        if (Localization.mInstance == null)
        {
            Localization.mInstance = this;
        }
    }

    public static string Localize(string key)
    {
        return (!(Localization.instance != null)) ? key : Localization.instance.Get(key);
    }

    public string Get(string key)
    {
        string text;
        return (!this.mDictionary.TryGetValue(key, out text)) ? key : text;
    }
}