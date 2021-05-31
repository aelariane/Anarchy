using Anarchy.UI;
using Optimization;
using Optimization.Caching;
using System.Collections;
using UnityEngine;

public class PVPcheckPoint : Photon.MonoBehaviour
{
    private bool annie;
    private float getPtsInterval = 20f;
    private float getPtsTimer;
    private float hitTestR = 15f;
    private bool playerOn;
    private float spawnTitanTimer;
    private GameObject supply;
    private float syncInterval = 0.6f;
    private float syncTimer;
    private bool titanOn;
    public static ArrayList chkPts;
    public GameObject[] chkPtNextArr;
    public GameObject[] chkPtPreviousArr;
    public bool hasAnnie;
    public GameObject humanCyc;
    public float humanPt;
    public float humanPtMax = 40f;
    public int id;
    public bool isBase;
    public int normalTitanRate = 70;
    public float size = 1f;
    public CheckPointState state;
    public GameObject titanCyc;
    public float titanInterval = 30f;
    public float titanPt;
    public float titanPtMax = 40f;
    private float _lastTitanPt;
    private float _lastHumanPt;

    private void Awake()
    {
        NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnPhotonPlayerConnected, OnPhotonPlayerConnected);
    }

    private void OnDestroy()
    {
        NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnPhotonPlayerConnected, OnPhotonPlayerConnected);
    }

    public GameObject chkPtNext
    {
        get
        {
            if (this.chkPtNextArr.Length <= 0)
            {
                return null;
            }
            return this.chkPtNextArr[UnityEngine.Random.Range(0, this.chkPtNextArr.Length)];
        }
    }

    public GameObject chkPtPrevious
    {
        get
        {
            if (this.chkPtPreviousArr.Length <= 0)
            {
                return null;
            }
            return this.chkPtPreviousArr[UnityEngine.Random.Range(0, this.chkPtPreviousArr.Length)];
        }
    }

    [RPC]
    private void changeHumanPt(float pt)
    {
        this.humanPt = pt;
    }

    [RPC]
    private void changeState(int num)
    {
        if (num == 0)
        {
            this.state = CheckPointState.Non;
        }
        if (num == 1)
        {
            this.state = CheckPointState.Human;
        }
        if (num == 2)
        {
            this.state = CheckPointState.Titan;
        }
    }

    [RPC]
    private void changeTitanPt(float pt)
    {
        this.titanPt = pt;
    }

    private void checkIfBeingCapture()
    {
        this.playerOn = false;
        this.titanOn = false;
        GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] array2 = GameObject.FindGameObjectsWithTag("titan");
        for (int i = 0; i < array.Length; i++)
        {
            if (Vector3.Distance(array[i].transform.position, base.transform.position) < this.hitTestR)
            {
                this.playerOn = true;
                if (this.state == CheckPointState.Human && array[i].GetPhotonView().IsMine)
                {
                    if (FengGameManagerMKII.FGM.checkpoint != base.gameObject)
                    {
                        FengGameManagerMKII.FGM.checkpoint = base.gameObject;
                        Chat.Add("<color=#A8FF24>Respawn point changed to point" + this.id + "</color>");
                    }
                    break;
                }
            }
        }
        for (int i = 0; i < array2.Length; i++)
        {
            if (Vector3.Distance(array2[i].transform.position, base.transform.position) < this.hitTestR + 5f)
            {
                if (!array2[i].GetComponent<TITAN>() || !array2[i].GetComponent<TITAN>().hasDie)
                {
                    this.titanOn = true;
                    if (this.state == CheckPointState.Titan && array2[i].GetPhotonView().IsMine && array2[i].GetComponent<TITAN>() && array2[i].GetComponent<TITAN>().nonAI)
                    {
                        if (FengGameManagerMKII.FGM.checkpoint != base.gameObject)
                        {
                            FengGameManagerMKII.FGM.checkpoint = base.gameObject;
                            Chat.Add("<color=#A8FF24>Respawn point changed to point" + this.id + "</color>");
                        }
                        break;
                    }
                }
            }
        }
    }

    private bool checkIfHumanWins()
    {
        for (int i = 0; i < PVPcheckPoint.chkPts.Count; i++)
        {
            if ((PVPcheckPoint.chkPts[i] as PVPcheckPoint).state != CheckPointState.Human)
            {
                return false;
            }
        }
        return true;
    }

    private bool checkIfTitanWins()
    {
        for (int i = 0; i < PVPcheckPoint.chkPts.Count; i++)
        {
            if ((PVPcheckPoint.chkPts[i] as PVPcheckPoint).state != CheckPointState.Titan)
            {
                return false;
            }
        }
        return true;
    }

    private float getHeight(Vector3 pt)
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(pt, -Vectors.up, out raycastHit, 1000f, Layers.Ground.value))
        {
            return raycastHit.point.y;
        }
        return 0f;
    }

    private void humanGetsPoint()
    {
        if (this.humanPt >= this.humanPtMax)
        {
            this.humanPt = this.humanPtMax;
            this.titanPt = 0f;
            this.syncPts();
            this.state = CheckPointState.Human;
            BasePV.RPC("changeState", PhotonTargets.All, new object[]
            {
                1
            });
            if (FengGameManagerMKII.Level.MapName != "The City I")
            {
                this.supply = Optimization.Caching.Pool.NetworkEnable("aot_supply", base.transform.position - Vectors.up * (base.transform.position.y - this.getHeight(base.transform.position)), base.transform.rotation, 0);
            }
            (FengGameManagerMKII.FGM.logic as GameLogic.PVPCaptureLogic).PVPHumanScore += 2;
            FengGameManagerMKII.FGM.CheckPVPpts();
            if (this.checkIfHumanWins())
            {
                FengGameManagerMKII.FGM.GameWin();
            }
        }
        else
        {
            this.humanPt += Time.deltaTime;
        }
    }

    private void humanLosePoint()
    {
        if (this.humanPt > 0f)
        {
            this.humanPt -= Time.deltaTime * 3f;
            if (this.humanPt <= 0f)
            {
                this.humanPt = 0f;
                this.syncPts();
                if (this.state != CheckPointState.Titan)
                {
                    this.state = CheckPointState.Non;
                    BasePV.RPC("changeState", PhotonTargets.Others, new object[]
                    {
                        0
                    });
                }
            }
        }
    }

    private void newTitan()
    {
        TITAN tit = FengGameManagerMKII.FGM.SpawnTitan(this.normalTitanRate, base.transform.position - Vectors.up * (base.transform.position.y - this.getHeight(base.transform.position)), base.transform.rotation, false);
        if (FengGameManagerMKII.Level.MapName == "The City I")
        {
            tit.chaseDistance = 120f;
        }
        else
        {
            tit.chaseDistance = 200f;
        }
        tit.PVPfromCheckPt = this;
    }

    private void Start()
    {
        if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
        {
            UnityEngine.Object.Destroy(base.gameObject);
            return;
        }
        if (IN_GAME_MAIN_CAMERA.GameMode != GameMode.PVP_CAPTURE)
        {
            if (BasePV.IsMine)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
            UnityEngine.Object.Destroy(base.gameObject);
            return;
        }
        PVPcheckPoint.chkPts.Add(this);
        IComparer comparer = new IComparerPVPchkPtID();
        PVPcheckPoint.chkPts.Sort(comparer);
        if (this.humanPt == this.humanPtMax)
        {
            this.state = CheckPointState.Human;
            if (BasePV.IsMine && FengGameManagerMKII.Level.MapName != "The City I")
            {
                this.supply = Optimization.Caching.Pool.NetworkEnable("aot_supply", base.transform.position - Vectors.up * (base.transform.position.y - this.getHeight(base.transform.position)), base.transform.rotation, 0);
            }
        }
        else if (BasePV.IsMine && !this.hasAnnie)
        {
            if (UnityEngine.Random.Range(0, 100) < 50)
            {
                int num = UnityEngine.Random.Range(1, 2);
                for (int i = 0; i < num; i++)
                {
                    this.newTitan();
                }
            }
            if (this.isBase)
            {
                this.newTitan();
            }
        }
        if (this.titanPt == this.titanPtMax)
        {
            this.state = CheckPointState.Titan;
        }
        this.hitTestR = 15f * this.size;
        base.transform.localScale = new Vector3(this.size, this.size, this.size);
    }

    private void syncPts()
    {
        if (titanPt != _lastTitanPt)
        {
            BasePV.RPC("changeTitanPt", PhotonTargets.Others, new object[] { this.titanPt });
            _lastTitanPt = titanPt;
        }
        if (humanPt != _lastHumanPt)
        {
            BasePV.RPC("changeHumanPt", PhotonTargets.Others, new object[] { this.humanPt });
            _lastHumanPt = humanPt;
        }
    }

    public void OnPhotonPlayerConnected(AOTEventArgs args)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            BasePV.RPC("changeTitanPt", args.Player, new object[] { this.titanPt });
            BasePV.RPC("changeHumanPt", args.Player, new object[] { this.humanPt });
        }
    }

    private void titanGetsPoint()
    {
        if (this.titanPt >= this.titanPtMax)
        {
            this.titanPt = this.titanPtMax;
            this.humanPt = 0f;
            this.syncPts();
            if (this.state == CheckPointState.Human && this.supply != null)
            {
                PhotonNetwork.Destroy(this.supply);
            }
            this.state = CheckPointState.Titan;
            BasePV.RPC("changeState", PhotonTargets.All, new object[]
            {
                2
            });
            (FengGameManagerMKII.FGM.logic as GameLogic.PVPCaptureLogic).PVPTitanScore += 2;
            FengGameManagerMKII.FGM.CheckPVPpts();
            if (this.checkIfTitanWins())
            {
                FengGameManagerMKII.FGM.GameLose();
            }
            if (this.hasAnnie)
            {
                if (!this.annie)
                {
                    this.annie = true;
                    Optimization.Caching.Pool.NetworkEnable("FEMALE_TITAN", base.transform.position - Vectors.up * (base.transform.position.y - this.getHeight(base.transform.position)), base.transform.rotation, 0);
                }
                else
                {
                    this.newTitan();
                }
            }
            else
            {
                this.newTitan();
            }
        }
        else
        {
            this.titanPt += Time.deltaTime;
        }
    }

    private void titanLosePoint()
    {
        if (this.titanPt > 0f)
        {
            this.titanPt -= Time.deltaTime * 3f;
            if (this.titanPt <= 0f)
            {
                this.titanPt = 0f;
                this.syncPts();
                if (this.state != CheckPointState.Human)
                {
                    this.state = CheckPointState.Non;
                    BasePV.RPC("changeState", PhotonTargets.All, new object[]
                    {
                        0
                    });
                }
            }
        }
    }

    private void Update()
    {
        float num = this.humanPt / this.humanPtMax;
        float num2 = this.titanPt / this.titanPtMax;
        if (!BasePV.IsMine)
        {
            num = this.humanPt / this.humanPtMax;
            num2 = this.titanPt / this.titanPtMax;
            this.humanCyc.transform.localScale = new Vector3(num, num, 1f);
            this.titanCyc.transform.localScale = new Vector3(num2, num2, 1f);
            this.syncTimer += Time.deltaTime;
            if (this.syncTimer > this.syncInterval)
            {
                this.syncTimer = 0f;
                this.checkIfBeingCapture();
            }
            return;
        }
        if (this.state == CheckPointState.Non)
        {
            if (this.playerOn && !this.titanOn)
            {
                this.humanGetsPoint();
                this.titanLosePoint();
            }
            else if (this.titanOn && !this.playerOn)
            {
                this.titanGetsPoint();
                this.humanLosePoint();
            }
            else
            {
                this.humanLosePoint();
                this.titanLosePoint();
            }
        }
        else if (this.state == CheckPointState.Human)
        {
            if (this.titanOn && !this.playerOn)
            {
                this.titanGetsPoint();
            }
            else
            {
                this.titanLosePoint();
            }
            this.getPtsTimer += Time.deltaTime;
            if (this.getPtsTimer > this.getPtsInterval)
            {
                this.getPtsTimer = 0f;
                if (!this.isBase)
                {
                    (FengGameManagerMKII.FGM.logic as GameLogic.PVPCaptureLogic).PVPTitanScore++;
                }
                FengGameManagerMKII.FGM.CheckPVPpts();
            }
        }
        else if (this.state == CheckPointState.Titan)
        {
            if (this.playerOn && !this.titanOn)
            {
                this.humanGetsPoint();
            }
            else
            {
                this.humanLosePoint();
            }
            this.getPtsTimer += Time.deltaTime;
            if (this.getPtsTimer > this.getPtsInterval)
            {
                this.getPtsTimer = 0f;
                if (!this.isBase)
                {
                    (FengGameManagerMKII.FGM.logic as GameLogic.PVPCaptureLogic).PVPTitanScore++;
                }
                FengGameManagerMKII.FGM.CheckPVPpts();
            }
            this.spawnTitanTimer += Time.deltaTime;
            if (this.spawnTitanTimer > this.titanInterval)
            {
                this.spawnTitanTimer = 0f;
                if (FengGameManagerMKII.Level.MapName == "The City I")
                {
                    if (GameObject.FindGameObjectsWithTag("titan").Length < 12)
                    {
                        this.newTitan();
                    }
                }
                else if (GameObject.FindGameObjectsWithTag("titan").Length < 20)
                {
                    this.newTitan();
                }
            }
        }
        this.syncTimer += Time.deltaTime;
        if (this.syncTimer > this.syncInterval)
        {
            this.syncTimer = 0f;
            this.checkIfBeingCapture();
            this.syncPts();
        }
        num = this.humanPt / this.humanPtMax;
        num2 = this.titanPt / this.titanPtMax;
        this.humanCyc.transform.localScale = new Vector3(num, num, 1f);
        this.titanCyc.transform.localScale = new Vector3(num2, num2, 1f);
    }

    public string getStateString()
    {
        if (this.state == CheckPointState.Human)
        {
            return "[" + ColorSet.color_human + "]H[-]";
        }
        if (this.state == CheckPointState.Titan)
        {
            return "[" + ColorSet.color_titan_player + "]T[-]";
        }
        return "[" + ColorSet.color_D + "]_[-]";
    }
}