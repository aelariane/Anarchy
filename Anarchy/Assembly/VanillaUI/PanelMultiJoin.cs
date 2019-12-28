using Optimization.Caching;
using System.Collections;
using UnityEngine;

public class PanelMultiJoin : MonoBehaviour
{
    private int currentPage = 1;
    private float elapsedTime = 10f;
    private string filter = string.Empty;
    private ArrayList filterRoom;
    private int totalPage = 1;
    public GameObject[] items;

    private string getServerDataString(RoomInfo room)
    {
        string[] array = room.name.Split(new char[]
        {
            "`"[0]
        });
        return string.Concat(new object[]
        {
            (!(array[5] == string.Empty)) ? "[PWD]" : string.Empty,
            array[0],
            "/",
            array[1],
            "/",
            array[2],
            "/",
            array[4],
            " ",
            room.playerCount,
            "/",
            room.maxPlayers
        });
    }

    private void OnDisable()
    {
    }

    private void OnEnable()
    {
        this.currentPage = 1;
        this.totalPage = 0;
        this.refresh();
    }

    private void OnFilterSubmit(string content)
    {
        this.filter = content;
        this.updateFilterRooms();
        this.showlist();
    }

    private void showlist()
    {
        if (this.filter == string.Empty)
        {
            if (PhotonNetwork.GetRoomList().Length > 0)
            {
                this.totalPage = (PhotonNetwork.GetRoomList().Length - 1) / 10 + 1;
            }
        }
        else
        {
            this.updateFilterRooms();
            if (this.filterRoom.Count > 0)
            {
                this.totalPage = (this.filterRoom.Count - 1) / 10 + 1;
            }
        }
        if (this.currentPage < 1)
        {
            this.currentPage = this.totalPage;
        }
        if (this.currentPage > this.totalPage)
        {
            this.currentPage = 1;
        }
        this.showServerList();
    }

    private void showServerList()
    {
        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            return;
        }
        if (this.filter == string.Empty)
        {
            for (int i = 0; i < 10; i++)
            {
                int num = 10 * (this.currentPage - 1) + i;
                if (num < PhotonNetwork.GetRoomList().Length)
                {
                    this.items[i].SetActive(true);
                    this.items[i].GetComponentInChildren<UILabel>().text = this.getServerDataString(PhotonNetwork.GetRoomList()[num]);
                    this.items[i].GetComponentInChildren<BTN_Connect_To_Server_On_List>().roomName = PhotonNetwork.GetRoomList()[num].name;
                }
                else
                {
                    this.items[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                int num2 = 10 * (this.currentPage - 1) + i;
                if (num2 < this.filterRoom.Count)
                {
                    RoomInfo roomInfo = (RoomInfo)this.filterRoom[num2];
                    this.items[i].SetActive(true);
                    this.items[i].GetComponentInChildren<UILabel>().text = this.getServerDataString(roomInfo);
                    this.items[i].GetComponentInChildren<BTN_Connect_To_Server_On_List>().roomName = roomInfo.name;
                }
                else
                {
                    this.items[i].SetActive(false);
                }
            }
        }
        CacheGameObject.Find("LabelServerListPage").GetComponent<UILabel>().text = this.currentPage + "/" + this.totalPage;
    }

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            this.items[i].SetActive(true);
            this.items[i].GetComponentInChildren<UILabel>().text = string.Empty;
            this.items[i].SetActive(false);
        }
    }

    private void Update()
    {
        this.elapsedTime += Time.deltaTime;
        if (this.elapsedTime > 3f)
        {
            this.elapsedTime = 0f;
            this.showlist();
        }
    }

    private void updateFilterRooms()
    {
        this.filterRoom = new ArrayList();
        if (this.filter == string.Empty)
        {
            return;
        }
        foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
        {
            if (roomInfo.name.ToUpper().Contains(this.filter.ToUpper()))
            {
                this.filterRoom.Add(roomInfo);
            }
        }
    }

    public void connectToIndex(int index, string roomName)
    {
        int i;
        for (i = 0; i < 10; i++)
        {
            this.items[i].SetActive(false);
        }
        i = 10 * (this.currentPage - 1) + index;
        string[] array = roomName.Split(new char[]
        {
            "`"[0]
        });
        if (array[5] != string.Empty)
        {
            PanelMultiJoinPWD.Password = array[5];
            PanelMultiJoinPWD.roomName = roomName;
            NGUITools.SetActive(UIMainReferences.Main.PanelMultiPWD, true);
            NGUITools.SetActive(UIMainReferences.Main.panelMultiROOM, false);
        }
        else
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public void pageDown()
    {
        this.currentPage++;
        if (this.currentPage > this.totalPage)
        {
            this.currentPage = 1;
        }
        this.showServerList();
    }

    public void pageUp()
    {
        this.currentPage--;
        if (this.currentPage < 1)
        {
            this.currentPage = this.totalPage;
        }
        this.showServerList();
    }

    public void refresh()
    {
        this.showlist();
    }
}