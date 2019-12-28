using Optimization.Caching;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ShowInfoOfPlayer : Photon.MonoBehaviour
{
    private const int FontSize3D = 0;
    private GameObject textGo;
    private TextMesh tm;
    public bool DisableOnOwnObjects;
    public Font font;

    private void OnDisable()
    {
        if (this.textGo != null)
        {
            this.textGo.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (this.textGo != null)
        {
            this.textGo.SetActive(true);
        }
    }

    private void Start()
    {
        if (this.font == null)
        {
            this.font = (Font)Resources.FindObjectsOfTypeAll(typeof(Font))[0];
            Debug.LogWarning("No font defined. Found font: " + this.font);
        }
        if (this.tm == null)
        {
            this.textGo = new GameObject("3d text");
            this.textGo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            this.textGo.transform.parent = base.gameObject.transform;
            this.textGo.transform.localPosition = Vectors.zero;
            MeshRenderer meshRenderer = this.textGo.AddComponent<MeshRenderer>();
            meshRenderer.material = this.font.material;
            this.tm = this.textGo.AddComponent<TextMesh>();
            this.tm.font = this.font;
            this.tm.fontSize = 0;
            this.tm.anchor = TextAnchor.MiddleCenter;
        }
        if (!this.DisableOnOwnObjects && BasePV.IsMine)
        {
            base.enabled = false;
        }
    }

    private void Update()
    {
        if (this.DisableOnOwnObjects)
        {
            base.enabled = false;
            if (this.textGo != null)
            {
                this.textGo.SetActive(false);
            }
            return;
        }
        PhotonPlayer owner = BasePV.owner;
        if (owner != null)
        {
            this.tm.text = ((!string.IsNullOrEmpty(owner.Name)) ? owner.Name : "n/a");
        }
        else if (BasePV.isSceneView)
        {
            if (!this.DisableOnOwnObjects && BasePV.IsMine)
            {
                base.enabled = false;
                this.textGo.SetActive(false);
                return;
            }
            this.tm.text = "scn";
        }
        else
        {
            this.tm.text = "n/a";
        }
    }
}