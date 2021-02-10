using Optimization.Caching;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Photon.MonoBehaviour
{
    private GameObject baseG;
    private Transform baseGT;
    internal Transform baseT;
    private Vector3 heightOffSet = Vectors.up * 0.48f;
    private bool isdestroying;
    private float killTime;
    private float killTime2;
    private Vector3 launchOffSet = Vectors.zero;
    private bool left = true;
    private LineRenderer lineRenderer;
    private HERO master;
    private GameObject myRef;
    private Transform myRefT;
    private List<Vector3> nodes = new List<Vector3>();
    private int phase;
    private GameObject rope;
    private int spiralcount;
    private List<Vector3> spiralNodes;
    private Vector3 velocity = Vectors.zero;
    private Vector3 velocity2 = Vectors.zero;
    public bool leviMode;
    public float leviShootTime;
    public TITAN MyTitan;

    //Used by anarchy trap
    private float hookHoldTimer;
    private float trapKillTimer;
    private bool isOnTrap;

    private void Awake()
    {
        baseG = gameObject;
        baseGT = baseG.transform;
        baseT = transform;
    }

    private void CheckTitan()
    {
        if (master != null && master == IN_GAME_MAIN_CAMERA.MainHERO)
        {
            if (Physics.Raycast(baseT.position, velocity, out RaycastHit hit, 10f, Layers.PlayerAttackBox.value))
            {
                Collider hitCollider = hit.collider;
                if (hitCollider.name.Contains("PlayerDetectorRC"))
                {
                    TITAN titan = hitCollider.transform.root.gameObject.GetComponent<TITAN>();
                    if (titan != null)
                    {
                        if (MyTitan == null)
                        {
                            this.MyTitan = titan;
                            this.MyTitan.IsHooked = true;
                        }
                        else
                        {
                            if (MyTitan != titan)
                            {
                                MyTitan.IsHooked = false;
                                MyTitan = titan;
                                MyTitan.IsHooked = true;
                            }
                        }
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if ((this.phase == 2 || this.phase == 1) && this.leviMode)
        {
            this.spiralcount++;
            if (this.spiralcount >= 60)
            {
                this.isdestroying = true;
                this.RemoveMe();
                return;
            }
        }
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single || BasePV.IsMine)
        {
            if (phase != 0)
            {
                return;
            }
            CheckTitan();
            baseGT.position += (this.velocity * 50f + this.velocity2) * Time.fixedDeltaTime;
            bool flag = false;
            if (Physics.Linecast(nodes.Count > 1 ? nodes[this.nodes.Count - 2] : nodes[this.nodes.Count - 1], baseGT.position, out RaycastHit raycastHit, Layers.EnemyGroundNetwork.value))
            {
                bool flag3 = true;
                Transform tf = raycastHit.collider.transform;
                switch (tf.gameObject.layer)
                {
                    case Layers.EnemyBoxN:
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
                        {
                            BasePV.RPC("tieMeToOBJ", PhotonTargets.Others, new object[] { raycastHit.collider.transform.root.gameObject.GetPhotonView().viewID });
                        }
                        master.LastHook = this;
                        baseT.parent = tf;
                        break;

                    case Layers.GroundN:
                        master.LastHook = null;
                        if (FengGameManagerMKII.Level.Name.StartsWith("Custom-Anarchy"))
                        {
                            if (tf.gameObject.GetComponent<Anarchy.Custom.Scripts.ClippingObjectComponent>() != null)
                            {
                                baseT.parent = tf;
                            }
                            if (tf.gameObject.GetComponent<Anarchy.Custom.Scripts.TrapComponent>() != null)
                            {
                                isOnTrap = true;
                                var trap = tf.gameObject.GetComponent<Anarchy.Custom.Scripts.TrapComponent>();
                                if (trap.Type == Anarchy.Custom.Scripts.TrapType.Both || trap.Type == Anarchy.Custom.Scripts.TrapType.GasUsage)
                                {
                                    master.GasMultiplier = trap.GasUsageMultiplier;
                                }
                                if(trap.Type == Anarchy.Custom.Scripts.TrapType.Both || trap.Type == Anarchy.Custom.Scripts.TrapType.Kill)
                                {
                                    trapKillTimer = trap.KillTime;
                                }
                            }
                        }
                        break;

                    case Layers.NetworkObjectN:
                        if (!tf.gameObject.CompareTag("Player") || leviMode)
                        {
                            break;
                        }

                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
                        {
                            BasePV.RPC("tieMeToOBJ", PhotonTargets.Others, new object[] { tf.root.gameObject.GetPhotonView().viewID });
                        }
                        master.HookToHuman(tf.root.gameObject, baseT.position);
                        baseT.parent = tf;
                        master.LastHook = null;
                        break;

                    default:
                        flag3 = false;
                        break;
                }
                if (flag3)
                {
                    if (master.State != HeroState.Grab)
                    {
                        master.Launch(raycastHit.point, this.left, this.leviMode);
                    }
                    baseT.position = raycastHit.point;
                    if (this.phase != 2)
                    {
                        this.phase = 1;
                        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
                        {
                            BasePV.RPC("setPhase", PhotonTargets.Others, new object[] { 1 });
                            BasePV.RPC("tieMeTo", PhotonTargets.Others, new object[] { baseT.position });
                        }
                        if (this.leviMode)
                        {
                            this.GetSpiral(master.baseT.position, master.baseT.rotation.eulerAngles);
                        }
                        flag = true;
                    }
                }
            }
            this.nodes.Add(new Vector3(baseGT.position.x, baseGT.position.y, baseGT.position.z));
            if (flag)
            {
                return;
            }

            this.killTime2 += Time.fixedDeltaTime;
            if (this.killTime2 > 0.8f)
            {
                this.phase = 4;
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
                {
                    BasePV.RPC("setPhase", PhotonTargets.Others, new object[] { 4 });
                }
                return;
            }
            return;
        }
        if (this.phase == 0)
        {
            baseGT.position += (this.velocity * 50f + this.velocity2) * Time.fixedDeltaTime;
            nodes.Add(new Vector3(baseGT.position.x, baseGT.position.y, baseGT.position.z));
        }
    }

    private void GetSpiral(Vector3 masterposition, Vector3 masterrotation)
    {
        float num = 30f;
        float num2 = 0.5f;
        float num3 = 0.05f + (float)this.spiralcount * 0.03f;
        float num5;
        if (this.spiralcount < 5)
        {
            float num4 = Vector2.Distance(new Vector2(masterposition.x, masterposition.z), new Vector2(baseGT.position.x, baseGT.position.z));
            num5 = num4;
        }
        else
        {
            num5 = 1.2f + (float)(60 - this.spiralcount) * 0.1f;
        }
        num2 -= (float)this.spiralcount * 0.06f;
        float num6 = num5 / num;
        float num7 = num3 / num;
        float num8 = num7 * 2f * 3.14159274f;
        num2 *= 6.28318548f;
        this.spiralNodes = new List<Vector3>();
        int num9 = 1;
        while ((float)num9 <= num)
        {
            float num10 = (float)num9 * num6 * (1f + 0.05f * (float)num9);
            float f = (float)num9 * num8 + num2 + 1.2566371f + masterrotation.y * 0.0173f;
            float x = Mathf.Cos(f) * num10;
            float z = -Mathf.Sin(f) * num10;
            this.spiralNodes.Add(new Vector3(x, 0f, z));
            num9++;
        }
    }

    [RPC]
    private void killObject(PhotonMessageInfo info)
    {
        Anarchy.UI.Log.AddLineRaw($"Bullet.killObjectRPC by ID {info.Sender.ID}", Anarchy.UI.MsgType.Warning);
    }

    [RPC]
    private void myMasterIs(int id, string launcherRef)
    {
        PhotonView pv = PhotonView.Find(id);
        if (pv == null)
        {
            return;
        }
        if (pv.gameObject != null)
        {
            master = pv.gameObject.GetComponent<HERO>();
        }
        switch (launcherRef)
        {
            case "hookRefL1":
                this.myRef = this.master.hookRefL1;
                break;

            case "hookRefL2":
                this.myRef = this.master.hookRefL2;
                break;

            case "hookRefR1":
                this.myRef = this.master.hookRefR1;
                break;

            case "hookRefR2":
                this.myRef = this.master.hookRefR2;
                break;
        }
        if (myRef != null && myRef)
        {
            myRefT = myRef.transform;
        }
    }

    [RPC]
    private void netLaunch(Vector3 newPosition)
    {
        this.nodes = new List<Vector3>();
        this.nodes.Add(newPosition);
    }

    [RPC]
    private void netUpdateLeviSpiral(Vector3 newPosition, Vector3 masterPosition, Vector3 masterrotation)
    {
        this.phase = 2;
        this.leviMode = true;
        this.GetSpiral(masterPosition, masterrotation);
        Vector3 b = masterPosition - spiralNodes[0];
        this.lineRenderer.SetVertexCount((int)((float)this.spiralNodes.Count - (float)this.spiralcount * 0.5f));
        int num = 0;
        while ((float)num <= (float)(this.spiralNodes.Count - 1) - (float)this.spiralcount * 0.5f)
        {
            if (this.spiralcount < 5)
            {
                Vector3 position = spiralNodes[num] + b;
                float num2 = (float)(this.spiralNodes.Count - 1) - (float)this.spiralcount * 0.5f;
                position = new Vector3(position.x, position.y * ((num2 - (float)num) / num2) + newPosition.y * ((float)num / num2), position.z);
                this.lineRenderer.SetPosition(num, position);
            }
            else
            {
                this.lineRenderer.SetPosition(num, spiralNodes[num] + b);
            }
            num++;
        }
    }

    [RPC]
    private void netUpdatePhase1(Vector3 newPosition, Vector3 masterPosition)
    {
        this.lineRenderer.SetVertexCount(2);
        this.lineRenderer.SetPosition(0, newPosition);
        this.lineRenderer.SetPosition(1, masterPosition);
        baseT.position = newPosition;
    }

    private void OnDestroy()
    {
        if (FengGameManagerMKII.FGM != null)
        {
            FengGameManagerMKII.FGM.RemoveHook(this);
        }
        if (MyTitan != null)
        {
            MyTitan.IsHooked = false;
        }
        UnityEngine.Object.Destroy(this.rope);
    }

    private void SetLinePhase()
    {
        if (this.master == null)
        {
            UnityEngine.Object.Destroy(this.rope);
            UnityEngine.Object.Destroy(baseG);
            return;
        }
        if (this.nodes.Count <= 0)
        {
            return;
        }
        Vector3 a = myRefT.position - nodes[0];
        this.lineRenderer.SetVertexCount(this.nodes.Count);
        for (int i = 0; i <= this.nodes.Count - 1; i++)
        {
            this.lineRenderer.SetPosition(i, nodes[i] + a * Mathf.Pow(0.75f, (float)i));
        }
        if (this.nodes.Count > 1)
        {
            this.lineRenderer.SetPosition(1, myRefT.position);
        }
    }

    [RPC]
    private void setPhase(int value)
    {
        this.phase = value;
    }

    [RPC]
    private void setVelocityAndLeft(Vector3 value, Vector3 v2, bool l)
    {
        this.velocity = value;
        this.velocity2 = v2;
        this.left = l;
        baseT.rotation = Quaternion.LookRotation(value.normalized);
    }

    private void Start()
    {
        this.rope = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("rope"));
        this.lineRenderer = this.rope.GetComponent<LineRenderer>();
        FengGameManagerMKII.FGM.AddHook(this);
    }

    [RPC]
    private void tieMeTo(Vector3 p)
    {
        baseT.position = p;
    }

    [RPC]
    private void tieMeToOBJ(int id)
    {
        PhotonView view = PhotonView.Find(id);
        if (view != null && view.gameObject != null && view.gameObject.transform != null)
        {
            baseT.parent = PhotonView.Find(id).gameObject.transform;
        }
    }

    public void Disable()
    {
        if (isOnTrap)
        {
            if (master != null)
            {
                master.GasMultiplier = 1f;
                isOnTrap = false;
            }
        }
        this.phase = 2;
        this.killTime = 0f;
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
        {
            BasePV.RPC("setPhase", PhotonTargets.Others, new object[] { 2 });
        }
    }

    public bool IsHooked()
    {
        return this.phase == 1;
    }

    public void Launch(Vector3 v, Vector3 v2, string launcher_ref, bool isLeft, HERO hero, bool leviMode = false)
    {
        if (this.phase == 2)
        {
            return;
        }
        this.master = hero;
        this.velocity = v;
        float f = Mathf.Acos(Vector3.Dot(v.normalized, v2.normalized)) * 57.29578f;
        if (Mathf.Abs(f) > 90f)
        {
            this.velocity2 = Vectors.zero;
        }
        else
        {
            this.velocity2 = Vector3.Project(v2, v);
        }
        switch (launcher_ref)
        {
            case "hookRefL1":
                this.myRef = hero.hookRefL1;
                break;

            case "hookRefL2":
                this.myRef = hero.hookRefL2;
                break;

            case "hookRefR1":
                this.myRef = hero.hookRefR1;
                break;

            case "hookRefR2":
                this.myRef = hero.hookRefR2;
                break;
        }
        myRefT = myRef.transform;
        this.nodes = new List<Vector3>();
        this.nodes.Add(myRefT.position);
        this.phase = 0;
        this.leviMode = leviMode;
        this.left = isLeft;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
        {
            BasePV.RPC("myMasterIs", PhotonTargets.Others, new object[] { hero.BasePV.viewID, launcher_ref });
            BasePV.RPC("setVelocityAndLeft", PhotonTargets.Others, new object[] { v, this.velocity2, this.left });
        }
        baseT.position = myRefT.position;
        baseT.rotation = Quaternion.LookRotation(v.normalized);
    }

    public void RemoveMe()
    {
        if (isOnTrap)
        {
            if (master != null)
            {
                master.GasMultiplier = 1f;
                isOnTrap = false;
            }
        }
        this.isdestroying = true;
        if (IN_GAME_MAIN_CAMERA.GameType != GameType.Single && BasePV.IsMine)
        {
            PhotonNetwork.Destroy(BasePV);
            PhotonNetwork.RemoveRPCs(BasePV);
        }
        else if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            UnityEngine.Object.Destroy(this.rope);
            UnityEngine.Object.Destroy(baseG);
        }
    }

    public void update()
    {
        if (this.isdestroying)
        {
            return;
        }
        if (this.master == null)
        {
            this.RemoveMe();
            return;
        }
        if (this.leviMode)
        {
            this.leviShootTime += Time.deltaTime;
            if (this.leviShootTime > 0.4f)
            {
                this.phase = 2;
                baseG.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        if (this.phase == 0)
        {
            this.SetLinePhase();
        }
        else if (this.phase == 1)
        {
            Vector3 a = baseT.position - myRefT.position;
            Vector3 vector = baseT.position + myRefT.position;
            Vector3 a2 = master.baseR.velocity;
            float magnitude = a2.magnitude;
            float magnitude2 = a.magnitude;
            int num = (int)((magnitude2 + magnitude) / 5f);
            num = Mathf.Clamp(num, 2, 6);
            this.lineRenderer.SetVertexCount(num);
            this.lineRenderer.SetPosition(0, myRefT.position);
            int i = 1;
            float num2 = Mathf.Pow(magnitude2, 0.3f);
            while (i < num)
            {
                int num3 = num / 2;
                float num4 = (float)Mathf.Abs(i - num3);
                float num5 = ((float)num3 - num4) / (float)num3;
                num5 = Mathf.Pow(num5, 0.5f);
                float num6 = (num2 + magnitude) * 0.0015f * num5;
                this.lineRenderer.SetPosition(i, new Vector3(UnityEngine.Random.Range(-num6, num6), UnityEngine.Random.Range(-num6, num6), UnityEngine.Random.Range(-num6, num6)) + myRefT.position + a * ((float)i / (float)num) - Vectors.up * num2 * 0.05f * num5 - a2 * 0.001f * num5 * num2);
                i++;
            }
            this.lineRenderer.SetPosition(num - 1, baseT.position);

            if(isOnTrap && trapKillTimer > 0f)
            {
                hookHoldTimer += Time.deltaTime;
                if(hookHoldTimer > trapKillTimer)
                {
                    if(master != null)
                    {
                        Anarchy.Abuse.Kill(PhotonNetwork.player, "Trap");
                    }
                }
            }
        }
        else if (this.phase == 2)
        {
            if (this.leviMode)
            {
                this.GetSpiral(master.baseT.position, master.baseT.rotation.eulerAngles);
                Vector3 b = myRefT.position - spiralNodes[0];
                this.lineRenderer.SetVertexCount((int)((float)this.spiralNodes.Count - (float)this.spiralcount * 0.5f));
                int num7 = 0;
                while ((float)num7 <= (float)(this.spiralNodes.Count - 1) - (float)this.spiralcount * 0.5f)
                {
                    if (this.spiralcount < 5)
                    {
                        Vector3 position = spiralNodes[num7] + b;
                        float num8 = (float)(this.spiralNodes.Count - 1) - (float)this.spiralcount * 0.5f;
                        position = new Vector3(position.x, position.y * ((num8 - (float)num7) / num8) + baseGT.position.y * ((float)num7 / num8), position.z);
                        this.lineRenderer.SetPosition(num7, position);
                    }
                    else
                    {
                        this.lineRenderer.SetPosition(num7, spiralNodes[num7] + b);
                    }
                    num7++;
                }
            }
            else
            {
                this.lineRenderer.SetVertexCount(2);
                this.lineRenderer.SetPosition(0, baseT.position);
                this.lineRenderer.SetPosition(1, myRefT.position);
                this.killTime += Time.deltaTime * (Time.fixedDeltaTime * 10f);
                this.lineRenderer.SetWidth(0.1f - this.killTime, 0.1f - this.killTime);
                if (this.killTime > 0.1f)
                {
                    this.RemoveMe();
                    return;
                }
            }
        }
        else if (this.phase == 4)
        {
            baseGT.position += this.velocity + (this.velocity2 * Time.deltaTime);
            this.nodes.Add(new Vector3(baseGT.position.x, baseGT.position.y, baseGT.position.z));
            Vector3 a3 = myRefT.position - nodes[0];
            for (int j = 0; j <= this.nodes.Count - 1; j++)
            {
                this.lineRenderer.SetVertexCount(this.nodes.Count);
                this.lineRenderer.SetPosition(j, nodes[j] + a3 * Mathf.Pow(0.5f, (float)j));
            }
            this.killTime2 += Time.deltaTime;
            if (this.killTime2 > 0.8f)
            {
                this.killTime += Time.deltaTime * (Time.fixedDeltaTime * 10f);
                this.lineRenderer.SetWidth(0.1f - this.killTime, 0.1f - this.killTime);
                if (this.killTime > 0.1f)
                {
                    this.RemoveMe();
                    return;
                }
            }
        }
    }
}