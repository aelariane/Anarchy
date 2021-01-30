using UnityEngine;

public class supplyCheck : MonoBehaviour
{
    private Transform baseT;
    private float elapsedTime;
    private float stepTime = 1f;

    private void Awake()
    {
        baseT = transform;
    }

    private void Start()
    {
        Minimap.TrackGameObjectOnMinimap(base.gameObject, Color.white, false, true, Minimap.IconStyle.Supply);
    }

    private void Update()
    {
        this.elapsedTime += Time.deltaTime;
        if (this.elapsedTime > this.stepTime)
        {
            this.elapsedTime -= this.stepTime;
            foreach (HERO hero in FengGameManagerMKII.Heroes)
            {
                if (hero != null && hero.IsLocal && Vector3.Distance(hero.baseT.position, baseT.position) < 1.5f)
                {
                    hero.GetSupply();
                }
            }
        }
    }
}