using ExitGames.Client.Photon;
using UnityEngine;

public class PhotonLagSimulationGui : MonoBehaviour
{
    public bool Visible = true;
    public int WindowId = 101;
    public Rect WindowRect = new Rect(0f, 100f, 120f, 100f);
    public PhotonPeer Peer { get; set; }

    private void NetSimHasNoPeerWindow(int windowId)
    {
        GUILayout.Label("No peer to communicate with. ", new GUILayoutOption[0]);
    }

    private void NetSimWindow(int windowId)
    {
        GUILayout.Label(string.Format("Rtt:{0,4} +/-{1,3}", this.Peer.RoundTripTime, this.Peer.RoundTripTimeVariance), new GUILayoutOption[0]);
        bool isSimulationEnabled = this.Peer.IsSimulationEnabled;
        bool flag = GUILayout.Toggle(isSimulationEnabled, "Simulate", new GUILayoutOption[0]);
        if (flag != isSimulationEnabled)
        {
            this.Peer.IsSimulationEnabled = flag;
        }
        float num = (float)this.Peer.NetworkSimulationSettings.IncomingLag;
        GUILayout.Label("Lag " + num, new GUILayoutOption[0]);
        num = GUILayout.HorizontalSlider(num, 0f, 500f, new GUILayoutOption[0]);
        this.Peer.NetworkSimulationSettings.IncomingLag = (int)num;
        this.Peer.NetworkSimulationSettings.OutgoingLag = (int)num;
        float num2 = (float)this.Peer.NetworkSimulationSettings.IncomingJitter;
        GUILayout.Label("Jit " + num2, new GUILayoutOption[0]);
        num2 = GUILayout.HorizontalSlider(num2, 0f, 100f, new GUILayoutOption[0]);
        this.Peer.NetworkSimulationSettings.IncomingJitter = (int)num2;
        this.Peer.NetworkSimulationSettings.OutgoingJitter = (int)num2;
        float num3 = (float)this.Peer.NetworkSimulationSettings.IncomingLossPercentage;
        GUILayout.Label("Loss " + num3, new GUILayoutOption[0]);
        num3 = GUILayout.HorizontalSlider(num3, 0f, 10f, new GUILayoutOption[0]);
        this.Peer.NetworkSimulationSettings.IncomingLossPercentage = (int)num3;
        this.Peer.NetworkSimulationSettings.OutgoingLossPercentage = (int)num3;
        if (GUI.changed)
        {
            this.WindowRect.height = 100f;
        }
        GUI.DragWindow();
    }

    public void OnGUI()
    {
        if (!this.Visible)
        {
            return;
        }
        if (this.Peer == null)
        {
            this.WindowRect = GUILayout.Window(this.WindowId, this.WindowRect, new GUI.WindowFunction(this.NetSimHasNoPeerWindow), "Netw. Sim.", new GUILayoutOption[0]);
        }
        else
        {
            this.WindowRect = GUILayout.Window(this.WindowId, this.WindowRect, new GUI.WindowFunction(this.NetSimWindow), "Netw. Sim.", new GUILayoutOption[0]);
        }
    }

    public void Start()
    {
        this.Peer = PhotonNetwork.networkingPeer;
    }
}