using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class DownloadTexture : MonoBehaviour
{
    private Material mMat;
    private Texture2D mTex;
    public string url = "http://www.tasharen.com/misc/logo.png";

    private void OnDestroy()
    {
        if (this.mMat != null)
        {
            UnityEngine.Object.Destroy(this.mMat);
        }
        if (this.mTex != null)
        {
            UnityEngine.Object.Destroy(this.mTex);
        }
    }

    private IEnumerator Start()
    {
        WWW www = new WWW(this.url);
        yield return www;
        this.mTex = www.texture;
        if (this.mTex != null)
        {
            UITexture ut = base.GetComponent<UITexture>();
            if (ut.material == null)
            {
                this.mMat = new Material(Shader.Find("Unlit/Transparent Colored"));
            }
            else
            {
                this.mMat = new Material(ut.material);
            }
            ut.material = this.mMat;
            this.mMat.mainTexture = this.mTex;
            ut.MakePixelPerfect();
        }
        www.Dispose();
        yield break;
    }
}