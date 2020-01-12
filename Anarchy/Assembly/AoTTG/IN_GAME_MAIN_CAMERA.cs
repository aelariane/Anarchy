using Anarchy;
using Anarchy.Configuration;
using Optimization.Caching;
using UnityEngine;

public class IN_GAME_MAIN_CAMERA : MonoBehaviour
{
    private static GameObject BaseG;
    private static Transform Head;
    private float closestDistance;
    private int currentPeekPlayerIndex;
    private float decay;
    private float distance = 10f;
    private float distanceMulti;
    private float distanceOffsetMulti;
    private float duration;
    private float flashDuration;
    private bool flip;
    private bool hasSnapShot;
    private float heightMulti;
    private bool lockAngle;
    private Vector3 lockCameraPosition;
    private GameObject locker;
    private GameObject lockTarget;
    private bool needSetHUD;
    private float R;
    private Texture2D snapshot1;
    private Texture2D snapshot2;
    private Texture2D snapshot3;
    private int snapShotCount;
    private float snapShotCountDown;
    private int snapShotDmg;
    private float snapShotInterval = 0.02f;
    private float snapShotStartCountDownTime;
    private GameObject snapShotTarget;
    private Vector3 snapShotTargetPosition;
    private bool startSnapShotFrameCount;
    private Vector3 verticalHeightOffset = Vectors.zero;

    private bool liveSpecMode = false;

    public static Camera BaseCamera = Camera.main;

    public static Transform BaseT;

    public static float cameraDistance = 0.6f;
    public static CameraType CameraMode;

    public static int cameraTilt = 1;

    public static int character = 1;

    public static DayLight DayLight = DayLight.Dawn;

    public static int Difficulty;

    public static GameMode GameMode;

    public static GameType GameType = GameType.Stop;

    public static int invertY = 1;

    public static bool isCheating;

    public static bool isPausing;

    public static bool isTyping;

    public static int level;

    public static IN_GAME_MAIN_CAMERA MainCamera;

    public static GameObject mainObject;

    public static float sensitivityMulti = 0.5f;

    public static string singleCharacter;

    public static STEREO_3D_TYPE stereoType;

    public static bool triggerAutoLock;

    public static bool usingTitan;

    public AudioSource bgmusic;

    public bool gameOver;


    public float justHit;

    public int lastScore;

    public float maximumX = 360f;

    public float maximumY = 60f;

    public float minimumX = -360f;

    public float minimumY = -60f;

    public static MouseLook Look;

    public int score;

    public Material skyBoxDAWN;

    public Material skyBoxDAY;

    public Material skyBoxNIGHT;

    public static SpectatorMovement SpecMov;

    public GameObject snapShotCamera;

    public bool spectatorMode;

    public Texture texture;

    public float timer;

    internal static TITAN_EREN MainEREN { get; private set; }

    internal static HERO MainHERO { get; private set; }

    internal static TITAN MainTITAN { get; private set; }

    public static GameObject MainObject
    {
        get => mainObject;
        private set
        {
            mainObject = value;
            MainR = value.rigidbody;
            MainT = value.transform;
        }
    }

    public static Rigidbody MainR { get; private set; }

    public static Transform MainT { get; private set; }

    private void Awake()
    {
        BaseCamera = Camera.main;
        MainCamera = this;
        BaseG = gameObject;
        BaseT = transform;
        name = "MainCamera";
        BaseCamera.useOcclusionCulling = true;
        isTyping = false;
        isPausing = false;
        SpecMov = GetComponent<SpectatorMovement>();
        Look = GetComponent<MouseLook>();
        if (PlayerPrefs.HasKey("GameQuality"))
        {
            if (PlayerPrefs.GetFloat("GameQuality") >= 0.9f)
            {
                base.GetComponent<TiltShift>().enabled = true;
            }
            else
            {
                base.GetComponent<TiltShift>().enabled = false;
            }
        }
        else
        {
            base.GetComponent<TiltShift>().enabled = true;
        }
        CameraMode = (CameraType)Anarchy.Configuration.Settings.CameraMode.Value;
        CreateMinimap();
    }

    private void CameraMovement()
    {
        this.distanceOffsetMulti = (cameraDistance * (200f - BaseCamera.fieldOfView)) / 150f;
        BaseT.position = (Head ?? MainT).position + (Vectors.up * (this.heightMulti - ((0.6f - cameraDistance) * 2f)));
        if (!AnarchyManager.Pause.Active && !AnarchyManager.PauseWindow.Active && !AnarchyManager.SettingsPanel.Active)
        {
            switch (CameraMode)
            {
                case CameraType.ORIGINAL:
                    if (Input.mousePosition.x < (Screen.width * 0.4f))
                    {
                        BaseT.RotateAround(BaseT.position, Vectors.up, (-((((Screen.width * 0.4f) - Input.mousePosition.x) / (Screen.width)) * 0.4f) * this.getSensitivityMultiWithDeltaTime()) * 150f);
                    }
                    else if (Input.mousePosition.x > (Screen.width * 0.6f))
                    {
                        BaseT.RotateAround(BaseT.position, Vectors.up, ((((Input.mousePosition.x - (Screen.width * 0.6f)) / (Screen.width)) * 0.4f) * this.getSensitivityMultiWithDeltaTime()) * 150f);
                    }
                    float x = ((140f * ((Screen.height * 0.6f) - Input.mousePosition.y)) / ((float)Screen.height)) * 0.5f;
                    BaseT.rotation = Quaternion.Euler(x, BaseT.rotation.eulerAngles.y, BaseT.rotation.eulerAngles.z);
                    break;

                case CameraType.WOW:
                    if (Input.GetKey(KeyCode.Mouse1))
                    {
                        BaseT.RotateAround(BaseT.position, Vectors.up, (Input.GetAxis("Mouse X") * 10f) * sensitivityMulti);
                        BaseT.RotateAround(BaseT.position, BaseT.Right(), ((-Input.GetAxis("Mouse Y") * 10f) * sensitivityMulti) * invertY);
                    }
                    break;

                case CameraType.TPS:
                    float num6 = ((-Input.GetAxis("Mouse Y") * 10f) * sensitivityMulti) * invertY;
                    BaseT.RotateAround(BaseT.position, Vectors.up, (Input.GetAxis("Mouse X") * 10f) * sensitivityMulti);
                    float num7 = BaseT.rotation.eulerAngles.x % 360f;
                    float num8 = num7 + num6;
                    if (((num6 <= 0f) || (((num7 >= 260f) || (num8 <= 260f)) && ((num7 >= 80f) || (num8 <= 80f)))) && ((num6 >= 0f) || (((num7 <= 280f) || (num8 >= 280f)) && ((num7 <= 100f) || (num8 >= 100f)))))
                    {
                        BaseT.RotateAround(BaseT.position, BaseT.Right(), num6);
                    }
                    break;

            }
        }
        BaseT.position -= (((BaseT.Forward() * this.distance) * this.distanceMulti) * this.distanceOffsetMulti);
        if (cameraDistance >= 0.65f) return;
        BaseT.position += (BaseT.Right() * Mathf.Max(((0.6f - cameraDistance) * 2f), 0.65f));
    }

    public void CameraMovementLive(HERO hero)
    {
        float magnitude = hero.rigidbody.velocity.magnitude;
        if (magnitude > 10f)
        {
            BaseCamera.fieldOfView = Mathf.Lerp(BaseCamera.fieldOfView, Mathf.Min(100f, magnitude + 40f), 0.1f);
        }
        else
        {
            BaseCamera.fieldOfView = Mathf.Lerp(BaseCamera.fieldOfView, 50f, 0.1f);
        }
        float num2 = hero.CameraMultiplier * (200f - BaseCamera.fieldOfView) / 150f;
        BaseT.position = (Head.position + Vectors.up * this.heightMulti - Vectors.up * (0.6f - cameraDistance) * 2f) - (BaseT.Forward() * this.distance * this.distanceMulti * num2);
        if (hero.CameraMultiplier < 0.65f)
        {
            BaseT.position += BaseT.Right() * Mathf.Max((0.6f - hero.CameraMultiplier) * 2f, 0.65f);
        }
        BaseT.rotation = Quaternion.Lerp(BaseT.rotation, hero.GetComponent<SmoothSyncMovement>().CorrectCameraRot, Time.deltaTime * 5f);
    }

    private void CreateMinimap()
    {
        LevelInfo info = FengGameManagerMKII.Level;
        if (info != null)
        {
            Minimap minimap = base.gameObject.AddComponent<Minimap>();
            if (Minimap.instance.myCam == null)
            {
                Minimap.instance.myCam = new GameObject().AddComponent<Camera>();
                Minimap.instance.myCam.nearClipPlane = 0.3f;
                Minimap.instance.myCam.farClipPlane = 1000f;
                Minimap.instance.myCam.enabled = false;
            }
            minimap.CreateMinimap(Minimap.instance.myCam, 512, 0.3f, info.minimapPreset);
            if(!Settings.Minimap.Value || GameModes.MinimapDisable.Enabled)
            {
                minimap.SetEnabled(false);
            }
        }
    }

    private GameObject findNearestTitan()
    {
        TITAN result = null;
        float num = this.closestDistance = float.PositiveInfinity;
        Vector3 position = MainT.position;
        foreach (TITAN tit in FengGameManagerMKII.Titans)
        {
            float magnitude = (tit.Neck.position - position).magnitude;
            if (magnitude < num)
            {
                if (!tit.hasDie)
                {
                    result = tit;
                    num = magnitude;
                    this.closestDistance = num;
                }
            }
        }
        return result.baseG;
    }

    private int getReverse()
    {
        return invertY;
    }

    private float getSensitivityMulti()
    {
        return sensitivityMulti;
    }

    private float getSensitivityMultiWithDeltaTime()
    {
        return sensitivityMulti * Time.deltaTime * 62f;
    }

    private void reset()
    {
        if (GameType != GameType.Single)
        {
            return;
        }
        FengGameManagerMKII.FGM.RestartGameSingle();
    }

    private Texture2D RTImage(Camera cam)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        Texture2D texture2D = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        int num = (int)((float)cam.targetTexture.width * 0.04f);
        int num2 = (int)((float)cam.targetTexture.width * 0.02f);
        texture2D.ReadPixels(new Rect((float)num, (float)num, (float)(cam.targetTexture.width - num), (float)(cam.targetTexture.height - num)), num2, num2);
        texture2D.Apply();
        RenderTexture.active = active;
        return texture2D;
    }

    private void shakeUpdate()
    {
        if (this.duration > 0f)
        {
            this.duration -= Time.deltaTime;
            if (this.flip)
            {
                BaseG.transform.position += Vectors.up * this.R;
            }
            else
            {
                BaseG.transform.position -= Vectors.up * this.R;
            }
            this.flip = !this.flip;
            this.R *= this.decay;
        }
    }

    private void Start()
    {
        FengGameManagerMKII.FGM.AddCamera(this);
        isPausing = false;
        this.setDayLight(DayLight);
        this.locker = CacheGameObject.Find("locker");
        this.createSnapShotRT();
        if(Anarchy.AnarchyManager.Background != null)
        {
            Settings.Apply();
            VideoSettings.Apply();
        }
    }

    public void createSnapShotRT()
    {
        if (this.snapShotCamera.GetComponent<Camera>().targetTexture != null)
        {
            this.snapShotCamera.GetComponent<Camera>().targetTexture.Release();
        }
        if (QualitySettings.GetQualityLevel() > 3)
        {
            this.snapShotCamera.GetComponent<Camera>().targetTexture = new RenderTexture((int)((float)Screen.width * 0.8f), (int)((float)Screen.height * 0.8f), 24);
        }
        else
        {
            this.snapShotCamera.GetComponent<Camera>().targetTexture = new RenderTexture((int)((float)Screen.width * 0.4f), (int)((float)Screen.height * 0.4f), 24);
        }
    }

    public void flashBlind()
    {
        GameObject gameObject = CacheGameObject.Find("flash");
        gameObject.GetComponent<UISprite>().alpha = 1f;
        this.flashDuration = 2f;
    }

    public void setDayLight(DayLight val)
    {
        DayLight = val;
        if (DayLight == DayLight.Night)
        {
            GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(CacheResources.Load("flashlight"));
            gameObject.transform.parent = BaseT;
            gameObject.transform.position = BaseT.position;
            gameObject.transform.rotation = Quaternion.Euler(353f, 0f, 0f);
            RenderSettings.ambientLight = FengColor.nightAmbientLight;
            CacheGameObject.Find("mainLight").GetComponent<Light>().color = FengColor.nightLight;
            BaseG.GetComponent<Skybox>().material = this.skyBoxNIGHT;
        }
        if (DayLight == DayLight.Day)
        {
            RenderSettings.ambientLight = FengColor.dayAmbientLight;
            CacheGameObject.Find("mainLight").GetComponent<Light>().color = FengColor.dayLight;
            BaseG.GetComponent<Skybox>().material = this.skyBoxDAY;
        }
        if (DayLight == DayLight.Dawn)
        {
            RenderSettings.ambientLight = FengColor.dawnAmbientLight;
            CacheGameObject.Find("mainLight").GetComponent<Light>().color = FengColor.dawnAmbientLight;
            BaseG.GetComponent<Skybox>().material = this.skyBoxDAWN;
        }
        this.snapShotCamera.gameObject.GetComponent<Skybox>().material = BaseG.GetComponent<Skybox>().material;
    }

    public void setHUDposition()
    {
        CacheGameObject.Find("Flare").transform.localPosition = new Vector3((float)(((int)(-Screen.width * 0.5f)) + 14), (float)((int)(-Screen.height * 0.5f)), 0f);
        var obj2 = CacheGameObject.Find("LabelInfoBottomRight");
        obj2.transform.localPosition = new Vector3((float)((int)(Screen.width * 0.5f)), (float)((int)(-Screen.height * 0.5f)), 0f);
        Optimization.Labels.BottomRight = "Pause : " + InputManager.Settings[InputCode.Pause] + " ";
        GameObject.Find("LabelInfoTopCenter").transform.localPosition = new Vector3(0f, (float)((int)(Screen.height * 0.5f)), 0f);
        GameObject.Find("LabelInfoTopRight").transform.localPosition = new Vector3((float)((int)(Screen.width * 0.5f)), (float)((int)(Screen.height * 0.5f)), 0f);
        GameObject.Find("LabelNetworkStatus").transform.localPosition = new Vector3((float)((int)(-Screen.width * 0.5f)), (float)((int)(Screen.height * 0.5f)), 0f);
        GameObject.Find("LabelInfoTopLeft").transform.localPosition = new Vector3((float)((int)(-Screen.width * 0.5f)), (float)((int)((Screen.height * 0.5f) - 20f)), 0f);
        //if (InRoomChat.Chat != null)
        //{
        //    InRoomChat.Chat.transform.localPosition = new Vector3((float)((int)(-Screen.width * 0.5f)), (float)((int)(-Screen.height * 0.5f)), 0f);
        //    InRoomChat.Chat.SetPosition();
        //}
        if (!usingTitan || GameType == GameType.Single)
        {
            CacheGameObject.Find("skill_cd_bottom").transform.localPosition = new Vector3(0f, (float)((int)((-Screen.height * 0.5f) + 5f)), 0f);
            CacheGameObject.Find("GasUI").transform.localPosition = CacheGameObject.Find("skill_cd_bottom").transform.localPosition;
            CacheGameObject.Find("stamina_titan").transform.localPosition = new Vector3(0f, 9999f, 0f);
            CacheGameObject.Find("stamina_titan_bottom").transform.localPosition = new Vector3(0f, 9999f, 0f);

        }
        else
        {
            Vector3 vector = new Vector3(0f, 9999f, 0f);
            CacheGameObject.Find("skill_cd_bottom").transform.localPosition = vector;
            CacheGameObject.Find("skill_cd_armin").transform.localPosition = vector;
            CacheGameObject.Find("skill_cd_eren").transform.localPosition = vector;
            CacheGameObject.Find("skill_cd_jean").transform.localPosition = vector;
            CacheGameObject.Find("skill_cd_levi").transform.localPosition = vector;
            CacheGameObject.Find("skill_cd_marco").transform.localPosition = vector;
            CacheGameObject.Find("skill_cd_mikasa").transform.localPosition = vector;
            CacheGameObject.Find("skill_cd_petra").transform.localPosition = vector;
            CacheGameObject.Find("skill_cd_sasha").transform.localPosition = vector;
            CacheGameObject.Find("GasUI").transform.localPosition = vector;
            CacheGameObject.Find("stamina_titan").transform.localPosition = new Vector3(-160f, (float)((int)((-Screen.height * 0.5f) + 15f)), 0f);
            CacheGameObject.Find("stamina_titan_bottom").transform.localPosition = new Vector3(-160f, (float)((int)((-Screen.height * 0.5f) + 15f)), 0f);
        }
        if ((MainObject != null) && (MainHERO != null))
        {
            MainHERO.setSkillHUDPosition();
        }
        this.createSnapShotRT();
    }

    public GameObject SetMainObject(object obj, bool resetRotation = true, bool lockAngle = false)
    {
        if (obj == null)
        {
            Head = null;
            this.distanceMulti = this.heightMulti = 1f;
        }
        else if (obj is HERO)
        {
            usingTitan = false;
            MainHERO = (HERO)obj;
            MainTITAN = null;
            MainObject = (obj as HERO).baseG;
            Head = ((HERO)obj).baseT.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
            this.distanceMulti = this.heightMulti = 0.64f;
            if (resetRotation)
            {
                BaseT.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        else if (obj is TITAN)
        {
            usingTitan = true;
            MainTITAN = obj as TITAN;
            MainHERO = null;
            MainEREN = null;
            MainObject = MainTITAN.baseG;
            Head = MainT.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
            this.distanceMulti = (Head != null) ? (Vector3.Distance(Head.position, MainT.position) * 0.4f) : 1f;
            this.heightMulti = (Head != null) ? (Vector3.Distance(Head.position, MainT.position) * 0.45f) : 1f;
            if (resetRotation)
            {
                BaseT.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        else if (obj is TITAN_EREN)
        {
            MainEREN = obj as TITAN_EREN;
            MainTITAN = null;
            MainObject = MainEREN.gameObject;
            Head = MainT.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck/head");
            this.distanceMulti = (Head != null) ? (Vector3.Distance(Head.position, MainT.position) * 0.2f) : 1f;
            this.heightMulti = (Head != null) ? (Vector3.Distance(Head.position, MainT.position) * 0.33f) : 1f;
            if (resetRotation)
            {
                BaseT.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        else
        {
            Head = null;
            this.distanceMulti = this.heightMulti = 1f;
            if (resetRotation)
            {
                BaseT.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            if (obj is GameObject obje)
            {
                MainObject = obje;
            }
        }
        this.lockAngle = lockAngle;
        return MainObject;
    }

    public void setSpectorMode(bool val)
    {
        this.spectatorMode = val;
        IN_GAME_MAIN_CAMERA.SpecMov.disable = !val;
        IN_GAME_MAIN_CAMERA.Look.disable = !val;
    }

    public void snapShot2(int index)
    {
        this.snapShotCamera.transform.position = ((!(Head != null)) ? MainT.position : Head.position);
        this.snapShotCamera.transform.position += Vectors.up * this.heightMulti;
        this.snapShotCamera.transform.position -= Vectors.up * 1.1f;
        Vector3 vector;
        Vector3 b = vector = this.snapShotCamera.transform.position;
        Vector3 vector2 = (vector + this.snapShotTargetPosition) * 0.5f;
        this.snapShotCamera.transform.position = vector2;
        vector = vector2;
        this.snapShotCamera.transform.LookAt(this.snapShotTargetPosition);
        if (index == 3)
        {
            this.snapShotCamera.transform.RotateAround(BaseT.position, Vectors.up, UnityEngine.Random.Range(-180f, 180f));
        }
        else
        {
            this.snapShotCamera.transform.RotateAround(BaseT.position, Vectors.up, UnityEngine.Random.Range(-20f, 20f));
        }
        this.snapShotCamera.transform.LookAt(vector);
        this.snapShotCamera.transform.RotateAround(vector, BaseT.right, UnityEngine.Random.Range(-20f, 20f));
        float num = Vector3.Distance(this.snapShotTargetPosition, b);
        if (this.snapShotTarget != null && this.snapShotTarget.GetComponent<TITAN>())
        {
            num += (float)(index - 1) * this.snapShotTarget.transform.localScale.x * 10f;
        }
        this.snapShotCamera.transform.position -= this.snapShotCamera.transform.Forward() * UnityEngine.Random.Range(num + 3f, num + 10f);
        this.snapShotCamera.transform.LookAt(vector);
        this.snapShotCamera.transform.RotateAround(vector, BaseT.forward, UnityEngine.Random.Range(-30f, 30f));
        Vector3 vector3 = (!(Head != null)) ? MainT.position : Head.position;
        Vector3 vector4 = ((!(Head != null)) ? MainT.position : Head.position) - this.snapShotCamera.transform.position;
        vector3 -= vector4;
        RaycastHit raycastHit;
        if (Head != null)
        {
            if (Physics.Linecast(Head.position, vector3, out raycastHit, Layers.Ground))
            {
                this.snapShotCamera.transform.position = raycastHit.point;
            }
            else if (Physics.Linecast(Head.position - vector4 * this.distanceMulti * 3f, vector3, out raycastHit, Layers.EnemyGround))
            {
                this.snapShotCamera.transform.position = raycastHit.point;
            }
        }
        else if (Physics.Linecast(MainT.position + Vectors.up, vector3, out raycastHit, Layers.EnemyGround))
        {
            this.snapShotCamera.transform.position = raycastHit.point;
        }
        switch (index)
        {
            case 1:
                this.snapshot1 = this.RTImage(this.snapShotCamera.GetComponent<Camera>());
                SnapShotSaves.addIMG(this.snapshot1, this.snapShotDmg);
                break;

            case 2:
                this.snapshot2 = this.RTImage(this.snapShotCamera.GetComponent<Camera>());
                SnapShotSaves.addIMG(this.snapshot2, this.snapShotDmg);
                break;

            case 3:
                this.snapshot3 = this.RTImage(this.snapShotCamera.GetComponent<Camera>());
                SnapShotSaves.addIMG(this.snapshot3, this.snapShotDmg);
                break;
        }
        this.snapShotCount = index;
        this.hasSnapShot = true;
        this.snapShotCountDown = 2f;
        if (index == 1)
        {
            FengGameManagerMKII.UIRefer.panels[0].transform.Find("snapshot1").GetComponent<UITexture>().mainTexture = this.snapshot1;
            FengGameManagerMKII.UIRefer.panels[0].transform.Find("snapshot1").GetComponent<UITexture>().transform.localScale = new Vector3((float)Screen.width * 0.4f, (float)Screen.height * 0.4f, 1f);
            FengGameManagerMKII.UIRefer.panels[0].transform.Find("snapshot1").GetComponent<UITexture>().transform.localPosition = new Vector3((float)(-(float)Screen.width) * 0.225f, (float)Screen.height * 0.225f, 0f);
            FengGameManagerMKII.UIRefer.panels[0].transform.Find("snapshot1").GetComponent<UITexture>().transform.rotation = Quaternion.Euler(0f, 0f, 10f);
            if (Settings.SnapshotsInGame.Value)
            {
                FengGameManagerMKII.UIRefer.panels[0].transform.Find("snapshot1").GetComponent<UITexture>().enabled = true;
            }
            else
            {
                FengGameManagerMKII.UIRefer.panels[0].transform.Find("snapshot1").GetComponent<UITexture>().enabled = false;
            }
        }
    }

    public void snapShotUpdate()
    {
        if (this.startSnapShotFrameCount)
        {
            this.snapShotStartCountDownTime -= Time.deltaTime;
            if (this.snapShotStartCountDownTime <= 0f)
            {
                this.snapShot2(1);
                this.startSnapShotFrameCount = false;
            }
        }
        if (this.hasSnapShot)
        {
            this.snapShotCountDown -= Time.deltaTime;
            if (this.snapShotCountDown <= 0f)
            {
                FengGameManagerMKII.UIRefer.panels[0].transform.Find("snapshot1").GetComponent<UITexture>().enabled = false;
                this.hasSnapShot = false;
                this.snapShotCountDown = 0f;
            }
            else if (this.snapShotCountDown < 1f)
            {
                FengGameManagerMKII.UIRefer.panels[0].transform.Find("snapshot1").GetComponent<UITexture>().mainTexture = this.snapshot3;
            }
            else if (this.snapShotCountDown < 1.5f)
            {
                FengGameManagerMKII.UIRefer.panels[0].transform.Find("snapshot1").GetComponent<UITexture>().mainTexture = this.snapshot2;
            }
            if (this.snapShotCount < 3)
            {
                this.snapShotInterval -= Time.deltaTime;
                if (this.snapShotInterval <= 0f)
                {
                    this.snapShotInterval = 0.05f;
                    this.snapShotCount++;
                    this.snapShot2(this.snapShotCount);
                }
            }
        }
    }

    public void startShake(float R, float duration, float decay = 0.95f)
    {
        if (this.duration < duration)
        {
            this.R = R;
            this.duration = duration;
            this.decay = decay;
        }
    }

    public void startSnapShot(Vector3 p, int dmg, GameObject target = null, float startTime = 0.02f)
    {
        this.snapShotCount = 1;
        this.startSnapShotFrameCount = true;
        this.snapShotTargetPosition = p;
        this.snapShotTarget = target;
        this.snapShotStartCountDownTime = startTime;
        this.snapShotInterval = 0.05f + UnityEngine.Random.Range(0f, 0.03f);
        this.snapShotDmg = dmg;
    }

    public void update()
    {
        if (GameType == GameType.Stop)
        {
            Screen.showCursor = true;
            Screen.lockCursor = false;
            return;
        }
        if (this.flashDuration > 0f)
        {
            this.flashDuration -= Time.deltaTime;
            if (this.flashDuration <= 0f)
            {
                this.flashDuration = 0f;
            }
            GameObject gameObject = CacheGameObject.Find("flash");
            gameObject.GetComponent<UISprite>().alpha = this.flashDuration * 0.5f;
        }
        if (this.gameOver && GameType != GameType.Single)
        {
            if (InputManager.IsInputDown[InputCode.Attack1])
            {
                if (this.spectatorMode)
                {
                    this.setSpectorMode(false);
                }
                else
                {
                    this.setSpectorMode(true);
                }
            }
            if (InputManager.IsInputDown[InputCode.Flare1])
            {
                this.currentPeekPlayerIndex++;
                int num = FengGameManagerMKII.Heroes.Count;
                if (this.currentPeekPlayerIndex >= num)
                {
                    this.currentPeekPlayerIndex = 0;
                }
                if (num > 0)
                {
                    this.SetMainObject(FengGameManagerMKII.Heroes[this.currentPeekPlayerIndex], true, false);
                    this.setSpectorMode(false);
                    this.lockAngle = false;
                }
            }
            if (InputManager.IsInputDown[InputCode.Flare2])
            {
                this.currentPeekPlayerIndex--;
                int num2 = FengGameManagerMKII.Heroes.Count;
                if (this.currentPeekPlayerIndex >= num2)
                {
                    this.currentPeekPlayerIndex = 0;
                }
                if (this.currentPeekPlayerIndex < 0)
                {
                    this.currentPeekPlayerIndex = num2 - 1;
                }
                if (num2 > 0)
                {
                    this.SetMainObject(FengGameManagerMKII.Heroes[this.currentPeekPlayerIndex], true, false);
                    this.setSpectorMode(false);
                    this.lockAngle = false;
                }
            }
            if (InputManager.IsInputRebind((int)Anarchy.InputPos.InputRebinds.LiveSpectate))
            {
                if (mainObject != null)
                    liveSpecMode = !liveSpecMode;
            }
            if (this.spectatorMode)
            {
                return;
            }
        }
        if (InputManager.IsInputDown[InputCode.Pause])
        {
            if (isPausing)
            {
                if (mainObject != null)
                {
                    Vector3 a = BaseT.position;
                    a = ((!(Head != null)) ? MainT.position : Head.position);
                    a += Vectors.up * this.heightMulti;
                    BaseT.position = Vector3.Lerp(BaseT.position, a - BaseT.forward * 5f, 0.2f);
                }
                return;
            }
            isPausing = !isPausing;
            if (isPausing)
            {
                if (GameType == GameType.Single)
                {
                    Time.timeScale = 0f;
                }
                AnarchyManager.Pause.Enable();
                InputManager.MenuOn = true;
                Screen.showCursor = true;
                Screen.lockCursor = false;
            }
        }
        if (this.needSetHUD)
        {
            this.needSetHUD = false;
            this.setHUDposition();
        }
        if (InputManager.IsInputDown[InputCode.FullScreen])
        {
            Screen.fullScreen = !Screen.fullScreen;
            if (Screen.fullScreen)
            {
                Screen.SetResolution(960, 600, false);
            }
            else
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            }
            this.needSetHUD = true;
            Anarchy.UI.UIManager.UpdateGUIScaling();
            Minimap.OnScreenResolutionChanged();
        }
        if (InputManager.IsInputDown[InputCode.Restart])
        {
            this.reset();
        }
        if (mainObject == null)
        {
            return;
        }
        if (InputManager.IsInputDown[InputCode.CameraChange])
        {
            if (CameraMode == CameraType.ORIGINAL)
            {
                CameraMode = CameraType.WOW;
                Screen.lockCursor = false;
            }
            else if (CameraMode == CameraType.WOW)
            {
                CameraMode = CameraType.TPS;
                Screen.lockCursor = true;
            }
            else if (CameraMode == CameraType.TPS)
            {
                CameraMode = CameraType.ORIGINAL;
                Screen.lockCursor = false;
            }
            //else if(CameraMode == CameraType.oldTPS)
            //{
            //    CameraMode = CameraType.ORIGINAL;
            //    Screen.lockCursor = false;
            //}
            Anarchy.Configuration.Settings.CameraMode.Value = (int)CameraMode;
        }
        if (InputManager.IsInputDown[InputCode.HideCursor])
        {
            Screen.showCursor = !Screen.showCursor;
        }
        if (InputManager.IsInputDown[InputCode.Focus])
        {
            triggerAutoLock = !triggerAutoLock;
            if (triggerAutoLock)
            {
                this.lockTarget = this.findNearestTitan();
                if (this.closestDistance >= 150f)
                {
                    this.lockTarget = null;
                    triggerAutoLock = false;
                }
            }
        }
        if (mainObject != null)
        {
            if (this.gameOver)
            {
                if (liveSpecMode && MainHERO && MainHERO.GetComponent<SmoothSyncMovement>().enabled && MainHERO.IsPhotonCamera)
                {
                    CameraMovementLive(MainHERO);
                }
                else if (this.lockAngle)
                {
                    BaseT.rotation = Quaternion.Lerp(BaseT.rotation, MainT.rotation, 0.2f);
                    BaseT.position = Vector3.Lerp(BaseT.position, MainT.position - MainT.forward * 5f, 0.2f);
                }
                else
                {
                    this.CameraMovement();
                }
            }
            else
            {
                 this.CameraMovement();
            }
            if (triggerAutoLock && this.lockTarget != null)
            {
                float z = BaseT.eulerAngles.z;
                Transform transform = this.lockTarget.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/neck");
                Vector3 a2 = transform.position - ((!(Head != null)) ? MainT.position : Head.position);
                a2.Normalize();
                this.lockCameraPosition = ((!(Head != null)) ? MainT.position : Head.position);
                this.lockCameraPosition -= a2 * this.distance * this.distanceMulti * this.distanceOffsetMulti;
                this.lockCameraPosition += Vectors.up * 3f * this.heightMulti * this.distanceOffsetMulti;
                BaseT.position = Vector3.Lerp(BaseT.position, this.lockCameraPosition, Time.deltaTime * 4f);
                if (Head != null)
                {
                    BaseT.LookAt(Head.position * 0.8f + transform.position * 0.2f);
                }
                else
                {
                    BaseT.LookAt(MainT.position * 0.8f + transform.position * 0.2f);
                }
                BaseT.localEulerAngles = new Vector3(BaseT.eulerAngles.x, BaseT.eulerAngles.y, z);
                Vector2 vector = BaseCamera.WorldToScreenPoint(transform.position - transform.Forward() * this.lockTarget.transform.localScale.x);
                this.locker.transform.localPosition = new Vector3(vector.x - (float)Screen.width * 0.5f, vector.y - (float)Screen.height * 0.5f, 0f);
                if (this.lockTarget.GetComponent<TITAN>() && this.lockTarget.GetComponent<TITAN>().hasDie)
                {
                    this.lockTarget = null;
                }
            }
            else
            {
                this.locker.transform.localPosition = new Vector3(0f, (float)(-(float)Screen.height) * 0.5f - 50f, 0f);
            }
            this.shakeUpdate();
        }
        Vector3 vector2 = (Head ?? MainT).position;
        Vector3 normalized = ((Head ?? MainT).position - BaseT.position).normalized;
        vector2 -= this.distance * normalized * this.distanceMulti;
        RaycastHit raycastHit;
        if (Head != null)
        {
            if (Physics.Linecast(Head.position, vector2, out raycastHit, Layers.Ground))
            {
                BaseT.position = raycastHit.point;
            }
            else if (Physics.Linecast(Head.position - normalized * this.distanceMulti * 3f, vector2, out raycastHit, Layers.EnemyBox))
            {
                BaseT.position = raycastHit.point;
            }
        }
        else if (Physics.Linecast(MainT.position + Vectors.up, vector2, out raycastHit, Layers.EnemyGround))
        {
            BaseT.position = raycastHit.point;
        }
    }
}