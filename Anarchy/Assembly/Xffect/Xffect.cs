using Optimization.Caching;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Xffect")]
public class Xffect : MonoBehaviour
{
    private List<EffectLayer> EflList = new List<EffectLayer>();
    private Dictionary<string, VertexPool> MatDic = new Dictionary<string, VertexPool>();
    protected float ElapsedTime;
    public float LifeTime = -1f;

    private void Awake()
    {
        this.Initialize();
    }

    private void LateUpdate()
    {
        foreach (KeyValuePair<string, VertexPool> keyValuePair in this.MatDic)
        {
            keyValuePair.Value.LateUpdate();
        }
        if (this.ElapsedTime > this.LifeTime && this.LifeTime >= 0f)
        {
            foreach (EffectLayer effectLayer in this.EflList)
            {
                effectLayer.Reset();
            }
            this.DeActive();
            this.ElapsedTime = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
    }

    private void Start()
    {
        base.transform.position = Vectors.zero;
        base.transform.rotation = Quaternion.identity;
        base.transform.localScale = Vectors.one;
        foreach (object obj in base.transform)
        {
            Transform transform = (Transform)obj;
            transform.transform.position = Vectors.zero;
            transform.transform.rotation = Quaternion.identity;
            transform.transform.localScale = Vectors.one;
        }
        foreach (EffectLayer effectLayer in this.EflList)
        {
            effectLayer.StartCustom();
        }
    }

    private void Update()
    {
        this.ElapsedTime += Time.deltaTime;
        foreach (EffectLayer effectLayer in this.EflList)
        {
            if (this.ElapsedTime > effectLayer.StartTime)
            {
                effectLayer.FixedUpdateCustom();
            }
        }
    }

    public void Active()
    {
        foreach (object obj in base.transform)
        {
            Transform transform = (Transform)obj;
            transform.gameObject.SetActive(true);
        }
        base.gameObject.SetActive(true);
        this.ElapsedTime = 0f;
    }

    public void DeActive()
    {
        foreach (object obj in base.transform)
        {
            Transform transform = (Transform)obj;
            transform.gameObject.SetActive(false);
        }
        base.gameObject.SetActive(false);
    }

    public void Initialize()
    {
        if (this.EflList.Count > 0)
        {
            return;
        }
        foreach (object obj in base.transform)
        {
            Transform transform = (Transform)obj;
            EffectLayer effectLayer = (EffectLayer)transform.GetComponent(typeof(EffectLayer));
            if (!(effectLayer == null) && !(effectLayer.Material == null))
            {
                Material material = effectLayer.Material;
                this.EflList.Add(effectLayer);
                Transform transform2 = base.transform.Find("mesh " + material.name);
                if (transform2 != null)
                {
                    MeshFilter meshFilter = (MeshFilter)transform2.GetComponent(typeof(MeshFilter));
                    MeshRenderer meshRenderer = (MeshRenderer)transform2.GetComponent(typeof(MeshRenderer));
                    meshFilter.mesh.Clear();
                    this.MatDic[material.name] = new VertexPool(meshFilter.mesh, material);
                }
                if (!this.MatDic.ContainsKey(material.name))
                {
                    GameObject gameObject = new GameObject("mesh " + material.name);
                    gameObject.transform.parent = base.transform;
                    gameObject.AddComponent("MeshFilter");
                    gameObject.AddComponent("MeshRenderer");
                    MeshFilter meshFilter = (MeshFilter)gameObject.GetComponent(typeof(MeshFilter));
                    MeshRenderer meshRenderer = (MeshRenderer)gameObject.GetComponent(typeof(MeshRenderer));
                    meshRenderer.castShadows = false;
                    meshRenderer.receiveShadows = false;
                    meshRenderer.renderer.material = material;
                    this.MatDic[material.name] = new VertexPool(meshFilter.mesh, material);
                }
            }
        }
        foreach (EffectLayer effectLayer2 in this.EflList)
        {
            effectLayer2.Vertexpool = this.MatDic[effectLayer2.Material.name];
        }
    }

    public void SetClient(Transform client)
    {
        foreach (EffectLayer effectLayer in this.EflList)
        {
            effectLayer.ClientTransform = client;
        }
    }

    public void SetDirectionAxis(Vector3 axis)
    {
        foreach (EffectLayer effectLayer in this.EflList)
        {
            effectLayer.OriVelocityAxis = axis;
        }
    }

    public void SetEmitPosition(Vector3 pos)
    {
        foreach (EffectLayer effectLayer in this.EflList)
        {
            effectLayer.EmitPoint = pos;
        }
    }
}