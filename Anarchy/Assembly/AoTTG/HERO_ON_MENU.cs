using UnityEngine;

public class HERO_ON_MENU : MonoBehaviour
{
    private Animation baseA;
    private Vector3 cameraOffset;
    private Transform cameraPref;
    private Transform head;
    private Transform monoT;
    public int costumeId;
    public float headRotationX;
    public float headRotationY;

    private void Awake()
    {
        baseA = animation;
        monoT = GameObject.Find("MainCamera_Mono").transform;
    }

    private void LateUpdate()
    {
        this.head.rotation = Quaternion.Euler(this.head.rotation.eulerAngles.x + this.headRotationX, this.head.rotation.eulerAngles.y + this.headRotationY, this.head.rotation.eulerAngles.z);
        if (this.costumeId == 9)
        {
            monoT.position = this.cameraPref.position + this.cameraOffset;
        }
    }

    private void Start()
    {
        HeroCostume.Init();
        HERO_SETUP component = gameObject.GetComponent<HERO_SETUP>();
        component.Init();
        component.myCostume = HeroCostume.costume[this.costumeId];
        component.SetCharacterComponent();
        this.head = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/neck/head");
        this.cameraPref = base.transform.Find("Amarture/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R");
        if (this.costumeId == 9)
        {
            this.cameraOffset = monoT.position - this.cameraPref.position;
        }
        if (component.myCostume.sex == Sex.Female)
        {
            baseA.Play("stand");
            baseA["stand"].normalizedTime = UnityEngine.Random.Range(0f, 1f);
        }
        else
        {
            baseA.Play("stand_levi");
            baseA["stand_levi"].normalizedTime = UnityEngine.Random.Range(0f, 1f);
        }
        AnimationState animationState = baseA["stand_levi"];
        float speed = 0.5f;
        baseA["stand"].speed = speed;
        animationState.speed = speed;
    }
}