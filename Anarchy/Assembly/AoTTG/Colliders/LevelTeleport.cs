using UnityEngine;

public class LevelTeleport : MonoBehaviour
{
    public string levelname = string.Empty;
    public GameObject link;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (this.levelname != string.Empty)
            {
                Application.LoadLevel(this.levelname);
            }
            else
            {
                other.gameObject.transform.position = this.link.transform.position;
            }
        }
    }
}