using RC;
using System;
using System.Collections;
using UnityEngine;

public class CannonPropRegion : Photon.MonoBehaviour
{
    public bool destroyed;
    public bool disabled;
    public string settings;
    public HERO storedHero;

    public void OnDestroy()
    {
        if (storedHero == null)
        {
            return;
        }
        this.storedHero.myCannonRegion = null;
        this.storedHero.ClearPopup();
    }

    public void OnTriggerEnter(Collider collider)
    {
        HERO hero = collider.transform.root.gameObject.GetComponent<HERO>();
        if (hero != null && hero.baseG.layer == 8 && hero.IsLocal && !hero.isCannon)
        {
            if (hero.myCannonRegion != null)
            {
                hero.myCannonRegion.storedHero = null;
            }
            hero.myCannonRegion = this;
            storedHero = hero;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        HERO hero = collider.transform.root.gameObject.GetComponent<HERO>();
        if (hero != null && hero.baseG.layer == 8 && hero.IsLocal && storedHero != null && hero == storedHero)
        {
            hero.myCannonRegion = null;
            hero.ClearPopup();
            storedHero = null;
        }
    }

    [RPC]
    public void RequestControlRPC(int viewID, PhotonMessageInfo info)
    {
        if (BasePV.IsMine && PhotonNetwork.IsMasterClient && !this.disabled)
        {
            HERO component = PhotonView.Find(viewID).gameObject.GetComponent<HERO>();
            bool flag2 = component != null && component.BasePV.owner == info.Sender && !RCManager.allowedToCannon.ContainsKey(info.Sender.ID);
            if (flag2)
            {
                this.disabled = true;
                base.StartCoroutine(this.WaitAndEnable());
                RCManager.allowedToCannon.Add(info.Sender.ID, new CannonValues(BasePV.viewID, this.settings));
                component.BasePV.RPC("SpawnCannonRPC", info.Sender, new object[]
                {
                    this.settings
                });
            }
        }
    }

    [RPC]
    public void SetSize(string settings, PhotonMessageInfo info)
    {
        if (info.Sender.IsMasterClient)
        {
            string[] array = settings.Split(new char[]
            {
                ','
            });
            bool flag = array.Length > 15;
            if (flag)
            {
                float a = 1f;
                GameObject gameObject = base.gameObject;
                bool flag2 = array[2] != "default";
                if (flag2)
                {
                    bool flag3 = array[2].StartsWith("transparent");
                    if (flag3)
                    {
                        float num;
                        bool flag4 = float.TryParse(array[2].Substring(11), out num);
                        if (flag4)
                        {
                            a = num;
                        }
                        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
                        {
                            renderer.material = (Material)RCManager.Load("transparent");
                            bool flag5 = Convert.ToSingle(array[10]) != 1f || Convert.ToSingle(array[11]) != 1f;
                            if (flag5)
                            {
                                renderer.material.mainTextureScale = new Vector2(renderer.material.mainTextureScale.x * Convert.ToSingle(array[10]), renderer.material.mainTextureScale.y * Convert.ToSingle(array[11]));
                            }
                        }
                    }
                    else
                    {
                        foreach (Renderer renderer2 in gameObject.GetComponentsInChildren<Renderer>())
                        {
                            renderer2.material = (Material)RCManager.Load(array[2]);
                            bool flag6 = Convert.ToSingle(array[10]) != 1f || Convert.ToSingle(array[11]) != 1f;
                            if (flag6)
                            {
                                renderer2.material.mainTextureScale = new Vector2(renderer2.material.mainTextureScale.x * Convert.ToSingle(array[10]), renderer2.material.mainTextureScale.y * Convert.ToSingle(array[11]));
                            }
                        }
                    }
                }
                float num2 = gameObject.transform.localScale.x * Convert.ToSingle(array[3]);
                num2 -= 0.001f;
                float y = gameObject.transform.localScale.y * Convert.ToSingle(array[4]);
                float z = gameObject.transform.localScale.z * Convert.ToSingle(array[5]);
                gameObject.transform.localScale = new Vector3(num2, y, z);
                bool flag7 = array[6] != "0";
                if (flag7)
                {
                    Color color = new Color(Convert.ToSingle(array[7]), Convert.ToSingle(array[8]), Convert.ToSingle(array[9]), a);
                    foreach (MeshFilter meshFilter in gameObject.GetComponentsInChildren<MeshFilter>())
                    {
                        Mesh mesh = meshFilter.mesh;
                        Color[] array2 = new Color[mesh.vertexCount];
                        for (int l = 0; l < mesh.vertexCount; l++)
                        {
                            array2[l] = color;
                        }
                        mesh.colors = array2;
                    }
                }
            }
        }
    }

    public IEnumerator WaitAndEnable()
    {
        yield return new WaitForSeconds(5f);
        if (!destroyed)
        {
            disabled = false;
        }
        yield break;
    }
}