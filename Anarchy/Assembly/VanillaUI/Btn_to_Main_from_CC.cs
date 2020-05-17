using Anarchy;
using UnityEngine;

public class Btn_to_Main_from_CC : MonoBehaviour
{
    private void OnClick()
    {
        PhotonNetwork.Disconnect();
        Screen.lockCursor = false;
        Screen.showCursor = true;
        IN_GAME_MAIN_CAMERA.GameType = GameType.Stop;
        FengGameManagerMKII.FGM.gameStart = false;
        InputManager.MenuOn = false;
        UnityEngine.Object.Destroy(FengGameManagerMKII.FGM);
        Application.LoadLevel("menu");
    }
}