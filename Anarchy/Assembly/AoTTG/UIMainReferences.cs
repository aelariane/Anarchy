using UnityEngine;

public sealed class UIMainReferences : MonoBehaviour
{
    private static bool isGAMEFirstLaunch = true;
    public const string VersionShow = "Anarchy mod(Custom) (Beta 7.8.0)";
    public static string ConnectField = "01042015";
    public static UIMainReferences Main;
    public GameObject panelCredits;
    public GameObject PanelDisconnect;
    public GameObject panelMain;
    public GameObject PanelMultiJoinPrivate;
    public GameObject PanelMultiPWD;
    public GameObject panelMultiROOM;
    public GameObject panelMultiSet;
    public GameObject panelMultiStart;
    public GameObject PanelMultiWait;
    public GameObject panelOption;
    public GameObject panelSingleSet;
    public GameObject PanelSnapShot;

    private void Awake()
    {
        Main = this;
    }

    private void Start()
    {
        GameObject.Find("VERSION").GetComponent<UILabel>().text = "Loading...";
        NGUITools.SetActive(this.panelMain, false);
        if (isGAMEFirstLaunch)
        {
            isGAMEFirstLaunch = false;
            GameObject input = (GameObject)Resources.Load("InputManagerController");
            input.GetComponent<FengCustomInputs>().enabled = false;
            input.AddComponent<Anarchy.InputManager>();
            var inputs = (GameObject)Instantiate(input);
            inputs.name = "InputManagerController";
            DontDestroyOnLoad(inputs);
            new GameObject("AnarchyManager").AddComponent<Anarchy.AnarchyManager>();
        }
        Anarchy.Network.NetworkManager.TryRejoin();
    }
}
