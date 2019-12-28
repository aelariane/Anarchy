using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonStatsGui : MonoBehaviour
{
    public bool buttonsOn;
    public bool healthStatsVisible;
    public bool statsOn = true;
    public Rect statsRect = new Rect(0f, 100f, 200f, 50f);
    public bool statsWindowOn = true;
    public bool trafficStatsOn;
    public int WindowId = 100;

    public void OnGUI()
    {
        if (PhotonNetwork.networkingPeer.TrafficStatsEnabled != this.statsOn)
        {
            PhotonNetwork.networkingPeer.TrafficStatsEnabled = this.statsOn;
        }
        if (!this.statsWindowOn)
        {
            return;
        }
        this.statsRect = GUILayout.Window(this.WindowId, this.statsRect, new GUI.WindowFunction(this.TrafficStatsWindow), "Messages (shift+tab)", new GUILayoutOption[0]);
    }

    public void Start()
    {
        this.statsRect.x = (float)Screen.width - this.statsRect.width;
    }

    public void TrafficStatsWindow(int windowID)
    {
        bool flag = false;
        TrafficStatsGameLevel trafficStatsGameLevel = PhotonNetwork.networkingPeer.TrafficStatsGameLevel;
        long num = PhotonNetwork.networkingPeer.TrafficStatsElapsedMs / 1000L;
        if (num == 0L)
        {
            num = 1L;
        }
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        this.buttonsOn = GUILayout.Toggle(this.buttonsOn, "buttons", new GUILayoutOption[0]);
        this.healthStatsVisible = GUILayout.Toggle(this.healthStatsVisible, "health", new GUILayoutOption[0]);
        this.trafficStatsOn = GUILayout.Toggle(this.trafficStatsOn, "traffic", new GUILayoutOption[0]);
        GUILayout.EndHorizontal();
        string text = string.Format("Out|In|Sum:\t{0,4} | {1,4} | {2,4}", trafficStatsGameLevel.TotalOutgoingMessageCount, trafficStatsGameLevel.TotalIncomingMessageCount, trafficStatsGameLevel.TotalMessageCount);
        string text2 = string.Format("{0}sec average:", num);
        string text3 = string.Format("Out|In|Sum:\t{0,4} | {1,4} | {2,4}", (long)trafficStatsGameLevel.TotalOutgoingMessageCount / num, (long)trafficStatsGameLevel.TotalIncomingMessageCount / num, (long)trafficStatsGameLevel.TotalMessageCount / num);
        GUILayout.Label(text, new GUILayoutOption[0]);
        GUILayout.Label(text2, new GUILayoutOption[0]);
        GUILayout.Label(text3, new GUILayoutOption[0]);
        if (this.buttonsOn)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.statsOn = GUILayout.Toggle(this.statsOn, "stats on", new GUILayoutOption[0]);
            if (GUILayout.Button("Reset", new GUILayoutOption[0]))
            {
                PhotonNetwork.networkingPeer.TrafficStatsReset();
                PhotonNetwork.networkingPeer.TrafficStatsEnabled = true;
            }
            flag = GUILayout.Button("To Log", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
        }
        string text4 = string.Empty;
        string text5 = string.Empty;
        if (this.trafficStatsOn)
        {
            text4 = "Incoming: " + PhotonNetwork.networkingPeer.TrafficStatsIncoming.ToString();
            text5 = "Outgoing: " + PhotonNetwork.networkingPeer.TrafficStatsOutgoing.ToString();
            GUILayout.Label(text4, new GUILayoutOption[0]);
            GUILayout.Label(text5, new GUILayoutOption[0]);
        }
        string text6 = string.Empty;
        if (this.healthStatsVisible)
        {
            text6 = string.Format("ping: {6}[+/-{7}]ms\nlongest delta between\nsend: {0,4}ms disp: {1,4}ms\nlongest time for:\nev({3}):{2,3}ms op({5}):{4,3}ms", new object[]
            {
                trafficStatsGameLevel.LongestDeltaBetweenSending,
                trafficStatsGameLevel.LongestDeltaBetweenDispatching,
                trafficStatsGameLevel.LongestEventCallback,
                trafficStatsGameLevel.LongestEventCallbackCode,
                trafficStatsGameLevel.LongestOpResponseCallback,
                trafficStatsGameLevel.LongestOpResponseCallbackOpCode,
                PhotonNetwork.networkingPeer.RoundTripTime,
                PhotonNetwork.networkingPeer.RoundTripTimeVariance
            });
            GUILayout.Label(text6, new GUILayoutOption[0]);
        }
        if (flag)
        {
            string message = string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", new object[]
            {
                text,
                text2,
                text3,
                text4,
                text5,
                text6
            });
            Debug.Log(message);
        }
        if (GUI.changed)
        {
            this.statsRect.height = 100f;
        }
        GUI.DragWindow();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            this.statsWindowOn = !this.statsWindowOn;
            this.statsOn = true;
        }
    }
}