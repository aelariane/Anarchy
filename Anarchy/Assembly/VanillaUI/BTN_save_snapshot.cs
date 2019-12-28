using Optimization.Caching;
using System;
using System.Collections;
using UnityEngine;

public class BTN_save_snapshot : MonoBehaviour
{
    public GameObject info;
    public GameObject targetTexture;
    public GameObject[] thingsNeedToHide;

    private void OnClick()
    {
        foreach (GameObject gameObject in this.thingsNeedToHide)
        {
            gameObject.transform.position += Vectors.up * 10000f;
        }
        base.StartCoroutine(this.ScreenshotEncode());
        this.info.GetComponent<UILabel>().text = "trying..";
    }

    private IEnumerator ScreenshotEncode()
    {
        yield return new WaitForEndOfFrame();
        float r = (float)Screen.height / 600f;
        Texture2D texture = new Texture2D((int)(r * this.targetTexture.transform.localScale.x), (int)(r * this.targetTexture.transform.localScale.y), TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect((float)Screen.width * 0.5f - (float)texture.width * 0.5f, (float)Screen.height * 0.5f - (float)texture.height * 0.5f - r * 0f, (float)texture.width, (float)texture.height), 0, 0);
        texture.Apply();
        yield return 0;
        foreach (GameObject go in this.thingsNeedToHide)
        {
            go.transform.position -= Vectors.up * 10000f;
        }
        string img_name = string.Concat(new string[]
        {
            "aottg_ss-",
            DateTime.Today.Month.ToString(),
            "_",
            DateTime.Today.Day.ToString(),
            "_",
            DateTime.Today.Year.ToString(),
            "-",
            DateTime.Now.Hour.ToString(),
            "_",
            DateTime.Now.Minute.ToString(),
            "_",
            DateTime.Now.Second.ToString(),
            ".png"
        });
        Application.ExternalCall("SaveImg", new object[]
        {
            img_name,
            texture.width,
            texture.height,
            Convert.ToBase64String(texture.EncodeToPNG())
        });
        UnityEngine.Object.DestroyObject(texture);
        yield break;
    }
}