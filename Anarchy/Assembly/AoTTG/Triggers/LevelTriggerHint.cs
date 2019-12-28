using Anarchy;
using UnityEngine;

public class LevelTriggerHint : MonoBehaviour
{
    private bool on;
    public string content;

    public HintType myhint;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            this.on = true;
        }
    }

    private void Start()
    {
        if (!FengGameManagerMKII.Level.Hint)
        {
            base.enabled = false;
        }
        if (this.content != string.Empty)
        {
            return;
        }
        switch (this.myhint)
        {
            case HintType.MOVE:
                this.content = string.Concat(new string[]
                {
                "Hello soldier!\nWelcome to Attack On Titan Tribute Game!\n Press [F7D358]",
                InputManager.Settings[InputCode.Up].ToString(),
                InputManager.Settings[InputCode.Left].ToString(),
                InputManager.Settings[InputCode.Down].ToString(),
                InputManager.Settings[InputCode.Right].ToString(),
                "[-] to Move."
                });
                break;

            case HintType.TELE:
                this.content = "Move to [82FA58]green warp point[-] to proceed.";
                break;

            case HintType.CAMA:
                this.content = string.Concat(new string[]
                {
                "Press [F7D358]",
                InputManager.Settings[InputCode.CameraChange].ToString(),
                "[-] to change camera mode\nPress [F7D358]",
                InputManager.Settings[InputCode.HideCursor].ToString(),
                "[-] to hide or show the cursor."
                });
                break;

            case HintType.JUMP:
                this.content = "Press [F7D358]" + InputManager.Settings[InputCode.Gas] + "[-] to Jump.";
                break;

            case HintType.JUMP2:
                this.content = "Press [F7D358]" + InputManager.Settings[InputCode.Up] + "[-] towards a wall to perform a wall-run.";
                break;

            case HintType.HOOK:
                this.content = string.Concat(new string[]
                {
                "Press and Hold[F7D358] ",
                InputManager.Settings[InputCode.LeftRope].ToString(),
                "[-] or [F7D358]",
                InputManager.Settings[InputCode.RightRope].ToString(),
                "[-] to launch your grapple.\nNow Try hooking to the [>3<] box. "
                });
                break;

            case HintType.HOOK2:
                this.content = string.Concat(new string[]
                {
                "Press and Hold[F7D358] ",
                InputManager.Settings[InputCode.BothRope].ToString(),
                "[-] to launch both of your grapples at the same Time.\n\nNow aim between the two black blocks. \nYou will see the mark '<' and '>' appearing on the blocks. \nThen press ",
                InputManager.Settings[InputCode.BothRope].ToString(),
                " to hook the blocks."
                });
                break;

            case HintType.SUPPLY:
                this.content = "Press [F7D358]" + InputManager.Settings[InputCode.Reload] + "[-] to reload your blades.\n Move to the supply station to refill your gas and blades.";
                break;

            case HintType.DODGE:
                this.content = "Press [F7D358]" + InputManager.Settings[InputCode.Dodge] + "[-] to Dodge.";
                break;

            case HintType.ATTACK:
                this.content = string.Concat(new string[]
                {
                "Press [F7D358]",
                InputManager.Settings[InputCode.Attack0].ToString(),
                "[-] to Attack. \nPress [F7D358]",
                InputManager.Settings[InputCode.Attack1].ToString(),
                "[-] to use special attack.\n***You can only kill a titan by slashing his [FA5858]NAPE[-].***\n\n"
                });
                break;
        }
    }

    private void Update()
    {
        if (this.on)
        {
            FengGameManagerMKII.FGM.ShowHUDInfoCenter(this.content + "\n\n\n\n\n");
            this.on = false;
        }
    }
}