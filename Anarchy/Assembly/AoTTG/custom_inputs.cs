using UnityEngine;

public class custom_inputs : MonoBehaviour
{
    private float AltInputBox_X = 120f;
    private bool altInputson;
    private float buttonHeight = 20f;
    private float DescBox_X = -320f;
    private float DescriptionBox_X;
    private bool[] inputBool;
    private bool[] inputBool2;
    private float InputBox_X = -100f;
    private float InputBox1_X;
    private float InputBox2_X;
    private KeyCode[] inputKey;
    private KeyCode[] inputKey2;
    private string[] inputString;
    private string[] inputString2;
    private float lastInterval;
    private float resetbuttonLocX = -100f;
    private float resetbuttonX;
    private bool tempbool;
    private bool[] tempjoy1;
    private bool[] tempjoy2;
    private int tempLength;
    public bool allowDuplicates;
    public KeyCode[] alt_default_inputKeys;

    [HideInInspector]
    public float analogFeel_down;

    public float analogFeel_gravity = 0.2f;

    [HideInInspector]
    public float analogFeel_jump;

    [HideInInspector]
    public float analogFeel_left;

    [HideInInspector]
    public float analogFeel_right;

    public float analogFeel_sensitivity = 0.8f;

    [HideInInspector]
    public float analogFeel_up;

    public float Boxes_Y = 300f;
    public float BoxesMargin_Y = 30f;
    public int buttonSize = 200;
    public KeyCode[] default_inputKeys;
    public int DescriptionSize = 200;
    public string[] DescriptionString;

    [HideInInspector]
    public bool[] isInput;

    [HideInInspector]
    public bool[] isInputDown;

    [HideInInspector]
    public bool[] isInputUp;

    [HideInInspector]
    public bool[] joystickActive;

    [HideInInspector]
    public bool[] joystickActive2;

    [HideInInspector]
    public string[] joystickString;

    [HideInInspector]
    public string[] joystickString2;

    public bool menuOn;
    public bool mouseAxisOn;
    public bool mouseButtonsOn = true;
    public GUISkin OurSkin;
    public float resetbuttonLocY = 600f;
    public string resetbuttonText = "Reset to defaults";

    private void checDoubleAxis(string testAxisString, int o, int p)
    {
        if (!this.allowDuplicates)
        {
            for (int i = 0; i < this.DescriptionString.Length; i++)
            {
                if (testAxisString == this.joystickString[i] && (i != o || p == 2))
                {
                    this.inputKey[i] = KeyCode.None;
                    this.inputBool[i] = false;
                    this.inputString[i] = this.inputKey[i].ToString();
                    this.joystickActive[i] = false;
                    this.joystickString[i] = "#";
                    this.saveInputs();
                }
                if (testAxisString == this.joystickString2[i] && (i != o || p == 1))
                {
                    this.inputKey2[i] = KeyCode.None;
                    this.inputBool2[i] = false;
                    this.inputString2[i] = this.inputKey2[i].ToString();
                    this.joystickActive2[i] = false;
                    this.joystickString2[i] = "#";
                    this.saveInputs();
                }
            }
        }
    }

    private void checDoubles(KeyCode testkey, int o, int p)
    {
        if (!this.allowDuplicates)
        {
            for (int i = 0; i < this.DescriptionString.Length; i++)
            {
                if (testkey == this.inputKey[i] && (i != o || p == 2))
                {
                    this.inputKey[i] = KeyCode.None;
                    this.inputBool[i] = false;
                    this.inputString[i] = this.inputKey[i].ToString();
                    this.joystickActive[i] = false;
                    this.joystickString[i] = "#";
                    this.saveInputs();
                }
                if (testkey == this.inputKey2[i] && (i != o || p == 1))
                {
                    this.inputKey2[i] = KeyCode.None;
                    this.inputBool2[i] = false;
                    this.inputString2[i] = this.inputKey2[i].ToString();
                    this.joystickActive2[i] = false;
                    this.joystickString2[i] = "#";
                    this.saveInputs();
                }
            }
        }
    }

    private void drawButtons1()
    {
        float num = this.Boxes_Y;
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        Vector3 point = GUI.matrix.inverse.MultiplyPoint3x4(new Vector3(x, (float)Screen.height - y, 1f));
        GUI.skin = this.OurSkin;
        GUI.Box(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), string.Empty);
        GUI.Box(new Rect(60f, 60f, (float)(Screen.width - 120), (float)(Screen.height - 120)), string.Empty, "window");
        GUI.Label(new Rect(this.DescriptionBox_X, num - 10f, (float)this.DescriptionSize, this.buttonHeight), "name", "textfield");
        GUI.Label(new Rect(this.InputBox1_X, num - 10f, (float)this.DescriptionSize, this.buttonHeight), "input", "textfield");
        GUI.Label(new Rect(this.InputBox2_X, num - 10f, (float)this.DescriptionSize, this.buttonHeight), "alt input", "textfield");
        for (int i = 0; i < this.DescriptionString.Length; i++)
        {
            num += this.BoxesMargin_Y;
            GUI.Label(new Rect(this.DescriptionBox_X, num, (float)this.DescriptionSize, this.buttonHeight), this.DescriptionString[i], "box");
            Rect position = new Rect(this.InputBox1_X, num, (float)this.buttonSize, this.buttonHeight);
            GUI.Button(position, this.inputString[i]);
            if (!this.joystickActive[i] && this.inputKey[i] == KeyCode.None)
            {
                this.joystickString[i] = "#";
            }
            if (this.inputBool[i])
            {
                GUI.Toggle(position, true, string.Empty, this.OurSkin.button);
            }
            if (position.Contains(point) && Input.GetMouseButtonUp(0) && !this.tempbool)
            {
                this.tempbool = true;
                this.inputBool[i] = true;
                this.lastInterval = Time.realtimeSinceStartup;
            }
            if (GUI.Button(new Rect(this.resetbuttonX, this.resetbuttonLocY, (float)this.buttonSize, this.buttonHeight), this.resetbuttonText) && Input.GetMouseButtonUp(0))
            {
                PlayerPrefs.DeleteAll();
                this.reset2defaults();
                this.loadConfig();
                this.saveInputs();
            }
            if (Event.current.type == EventType.KeyDown && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = Event.current.keyCode;
                this.inputBool[i] = false;
                this.inputString[i] = this.inputKey[i].ToString();
                this.tempbool = false;
                this.joystickActive[i] = false;
                this.joystickString[i] = "#";
                this.saveInputs();
                this.checDoubles(this.inputKey[i], i, 1);
            }
            if (this.mouseButtonsOn)
            {
                int num2 = 323;
                for (int j = 0; j < 6; j++)
                {
                    if (Input.GetMouseButton(j) && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
                    {
                        num2 += j;
                        this.inputKey[i] = (KeyCode)num2;
                        this.inputBool[i] = false;
                        this.inputString[i] = this.inputKey[i].ToString();
                        this.joystickActive[i] = false;
                        this.joystickString[i] = "#";
                        this.saveInputs();
                        this.checDoubles(this.inputKey[i], i, 1);
                    }
                }
            }
            for (int k = 350; k < 409; k++)
            {
                if (Input.GetKey((KeyCode)k) && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey[i] = (KeyCode)k;
                    this.inputBool[i] = false;
                    this.inputString[i] = this.inputKey[i].ToString();
                    this.tempbool = false;
                    this.joystickActive[i] = false;
                    this.joystickString[i] = "#";
                    this.saveInputs();
                    this.checDoubles(this.inputKey[i], i, 1);
                }
            }
            if (this.mouseAxisOn)
            {
                if (Input.GetAxis("MouseUp") == 1f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey[i] = KeyCode.None;
                    this.inputBool[i] = false;
                    this.joystickActive[i] = true;
                    this.joystickString[i] = "MouseUp";
                    this.inputString[i] = "Mouse Up";
                    this.tempbool = false;
                    this.saveInputs();
                    this.checDoubleAxis(this.joystickString[i], i, 1);
                }
                if (Input.GetAxis("MouseDown") == 1f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey[i] = KeyCode.None;
                    this.inputBool[i] = false;
                    this.joystickActive[i] = true;
                    this.joystickString[i] = "MouseDown";
                    this.inputString[i] = "Mouse Down";
                    this.tempbool = false;
                    this.saveInputs();
                    this.checDoubleAxis(this.joystickString[i], i, 1);
                }
                if (Input.GetAxis("MouseLeft") == 1f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey[i] = KeyCode.None;
                    this.inputBool[i] = false;
                    this.joystickActive[i] = true;
                    this.joystickString[i] = "MouseLeft";
                    this.inputBool[i] = false;
                    this.inputString[i] = "Mouse Left";
                    this.tempbool = false;
                    this.saveInputs();
                    this.checDoubleAxis(this.joystickString[i], i, 1);
                }
                if (Input.GetAxis("MouseRight") == 1f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey[i] = KeyCode.None;
                    this.inputBool[i] = false;
                    this.joystickActive[i] = true;
                    this.joystickString[i] = "MouseRight";
                    this.inputString[i] = "Mouse Right";
                    this.tempbool = false;
                    this.saveInputs();
                    this.checDoubleAxis(this.joystickString[i], i, 1);
                }
            }
            if (this.mouseButtonsOn)
            {
                if (Input.GetAxis("MouseScrollUp") > 0f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey[i] = KeyCode.None;
                    this.inputBool[i] = false;
                    this.joystickActive[i] = true;
                    this.joystickString[i] = "MouseScrollUp";
                    this.inputBool[i] = false;
                    this.inputString[i] = "Mouse scroll Up";
                    this.tempbool = false;
                    this.saveInputs();
                    this.checDoubleAxis(this.joystickString[i], i, 1);
                }
                if (Input.GetAxis("MouseScrollDown") > 0f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey[i] = KeyCode.None;
                    this.inputBool[i] = false;
                    this.joystickActive[i] = true;
                    this.joystickString[i] = "MouseScrollDown";
                    this.inputBool[i] = false;
                    this.inputString[i] = "Mouse scroll Down";
                    this.tempbool = false;
                    this.saveInputs();
                    this.checDoubleAxis(this.joystickString[i], i, 1);
                }
            }
            if (Input.GetAxis("JoystickUp") > 0.5f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "JoystickUp";
                this.inputString[i] = "Joystick Up";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("JoystickDown") > 0.5f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "JoystickDown";
                this.inputString[i] = "Joystick Down";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("JoystickLeft") > 0.5f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "JoystickLeft";
                this.inputString[i] = "Joystick Left";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("JoystickRight") > 0.5f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "JoystickRight";
                this.inputString[i] = "Joystick Right";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("Joystick_3a") > 0.8f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "Joystick_3a";
                this.inputString[i] = "Joystick Axis 3 +";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("Joystick_3b") > 0.8f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "Joystick_3b";
                this.inputString[i] = "Joystick Axis 3 -";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("Joystick_4a") > 0.8f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "Joystick_4a";
                this.inputString[i] = "Joystick Axis 4 +";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("Joystick_4b") > 0.8f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "Joystick_4b";
                this.inputString[i] = "Joystick Axis 4 -";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("Joystick_5b") > 0.8f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "Joystick_5b";
                this.inputString[i] = "Joystick Axis 5 -";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("Joystick_6b") > 0.8f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "Joystick_6b";
                this.inputString[i] = "Joystick Axis 6 -";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("Joystick_7a") > 0.8f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "Joystick_7a";
                this.inputString[i] = "Joystick Axis 7 +";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("Joystick_7b") > 0.8f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "Joystick_7b";
                this.inputString[i] = "Joystick Axis 7 -";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("Joystick_8a") > 0.8f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "Joystick_8a";
                this.inputString[i] = "Joystick Axis 8 +";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
            if (Input.GetAxis("Joystick_8b") > 0.8f && this.inputBool[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey[i] = KeyCode.None;
                this.inputBool[i] = false;
                this.joystickActive[i] = true;
                this.joystickString[i] = "Joystick_8b";
                this.inputString[i] = "Joystick Axis 8 -";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString[i], i, 1);
            }
        }
    }

    private void drawButtons2()
    {
        float num = this.Boxes_Y;
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        Vector3 point = GUI.matrix.inverse.MultiplyPoint3x4(new Vector3(x, (float)Screen.height - y, 1f));
        GUI.skin = this.OurSkin;
        for (int i = 0; i < this.DescriptionString.Length; i++)
        {
            num += this.BoxesMargin_Y;
            Rect position = new Rect(this.InputBox2_X, num, (float)this.buttonSize, this.buttonHeight);
            GUI.Button(position, this.inputString2[i]);
            if (!this.joystickActive2[i] && this.inputKey2[i] == KeyCode.None)
            {
                this.joystickString2[i] = "#";
            }
            if (this.inputBool2[i])
            {
                GUI.Toggle(position, true, string.Empty, this.OurSkin.button);
            }
            if (position.Contains(point) && Input.GetMouseButtonUp(0) && !this.tempbool)
            {
                this.tempbool = true;
                this.inputBool2[i] = true;
                this.lastInterval = Time.realtimeSinceStartup;
            }
            if (Event.current.type == EventType.KeyDown && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = Event.current.keyCode;
                this.inputBool2[i] = false;
                this.inputString2[i] = this.inputKey2[i].ToString();
                this.tempbool = false;
                this.joystickActive2[i] = false;
                this.joystickString2[i] = "#";
                this.saveInputs();
                this.checDoubles(this.inputKey2[i], i, 2);
            }
            if (this.mouseButtonsOn)
            {
                int num2 = 323;
                for (int j = 0; j < 6; j++)
                {
                    if (Input.GetMouseButton(j) && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
                    {
                        num2 += j;
                        this.inputKey2[i] = (KeyCode)num2;
                        this.inputBool2[i] = false;
                        this.inputString2[i] = this.inputKey2[i].ToString();
                        this.joystickActive2[i] = false;
                        this.joystickString2[i] = "#";
                        this.saveInputs();
                        this.checDoubles(this.inputKey2[i], i, 2);
                    }
                }
            }
            for (int k = 350; k < 409; k++)
            {
                if (Input.GetKey((KeyCode)k) && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey2[i] = (KeyCode)k;
                    this.inputBool2[i] = false;
                    this.inputString2[i] = this.inputKey2[i].ToString();
                    this.tempbool = false;
                    this.joystickActive2[i] = false;
                    this.joystickString2[i] = "#";
                    this.saveInputs();
                    this.checDoubles(this.inputKey2[i], i, 2);
                }
            }
            if (this.mouseAxisOn)
            {
                if (Input.GetAxis("MouseUp") == 1f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey2[i] = KeyCode.None;
                    this.inputBool2[i] = false;
                    this.joystickActive2[i] = true;
                    this.joystickString2[i] = "MouseUp";
                    this.inputString2[i] = "Mouse Up";
                    this.tempbool = false;
                    this.saveInputs();
                    this.checDoubleAxis(this.joystickString2[i], i, 2);
                }
                if (Input.GetAxis("MouseDown") == 1f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey2[i] = KeyCode.None;
                    this.inputBool2[i] = false;
                    this.joystickActive2[i] = true;
                    this.joystickString2[i] = "MouseDown";
                    this.inputString2[i] = "Mouse Down";
                    this.tempbool = false;
                    this.saveInputs();
                    this.checDoubleAxis(this.joystickString2[i], i, 2);
                }
                if (Input.GetAxis("MouseLeft") == 1f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey2[i] = KeyCode.None;
                    this.inputBool2[i] = false;
                    this.joystickActive2[i] = true;
                    this.joystickString2[i] = "MouseLeft";
                    this.inputBool2[i] = false;
                    this.inputString2[i] = "Mouse Left";
                    this.tempbool = false;
                    this.saveInputs();
                    this.checDoubleAxis(this.joystickString2[i], i, 2);
                }
                if (Input.GetAxis("MouseRight") == 1f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey2[i] = KeyCode.None;
                    this.inputBool2[i] = false;
                    this.joystickActive2[i] = true;
                    this.joystickString2[i] = "MouseRight";
                    this.inputString2[i] = "Mouse Right";
                    this.tempbool = false;
                    this.saveInputs();
                    this.checDoubleAxis(this.joystickString2[i], i, 2);
                }
            }
            if (this.mouseButtonsOn)
            {
                if (Input.GetAxis("MouseScrollUp") > 0f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey2[i] = KeyCode.None;
                    this.inputBool2[i] = false;
                    this.joystickActive2[i] = true;
                    this.joystickString2[i] = "MouseScrollUp";
                    this.inputBool2[i] = false;
                    this.inputString2[i] = "Mouse scroll Up";
                    this.tempbool = false;
                    this.saveInputs();
                    this.checDoubleAxis(this.joystickString2[i], i, 2);
                }
                if (Input.GetAxis("MouseScrollDown") > 0f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
                {
                    this.inputKey2[i] = KeyCode.None;
                    this.inputBool2[i] = false;
                    this.joystickActive2[i] = true;
                    this.joystickString2[i] = "MouseScrollDown";
                    this.inputBool2[i] = false;
                    this.inputString2[i] = "Mouse scroll Down";
                    this.tempbool = false;
                    this.saveInputs();
                    this.checDoubleAxis(this.joystickString2[i], i, 2);
                }
            }
            if (Input.GetAxis("JoystickUp") > 0.5f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "JoystickUp";
                this.inputString2[i] = "Joystick Up";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("JoystickDown") > 0.5f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "JoystickDown";
                this.inputString2[i] = "Joystick Down";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("JoystickLeft") > 0.5f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "JoystickLeft";
                this.inputBool2[i] = false;
                this.inputString2[i] = "Joystick Left";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("JoystickRight") > 0.5f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "JoystickRight";
                this.inputString2[i] = "Joystick Right";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("Joystick_3a") > 0.8f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "Joystick_3a";
                this.inputString2[i] = "Joystick Axis 3 +";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("Joystick_3b") > 0.8f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "Joystick_3b";
                this.inputString2[i] = "Joystick Axis 3 -";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("Joystick_4a") > 0.8f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "Joystick_4a";
                this.inputString2[i] = "Joystick Axis 4 +";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("Joystick_4b") > 0.8f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "Joystick_4b";
                this.inputString2[i] = "Joystick Axis 4 -";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("Joystick_5b") > 0.8f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "Joystick_5b";
                this.inputString2[i] = "Joystick Axis 5 -";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("Joystick_6b") > 0.8f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "Joystick_6b";
                this.inputString2[i] = "Joystick Axis 6 -";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("Joystick_7a") > 0.8f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "Joystick_7a";
                this.inputString2[i] = "Joystick Axis 7 +";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("Joystick_7b") > 0.8f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "Joystick_7b";
                this.inputString2[i] = "Joystick Axis 7 -";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("Joystick_8a") > 0.8f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "Joystick_8a";
                this.inputString2[i] = "Joystick Axis 8 +";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
            if (Input.GetAxis("Joystick_8b") > 0.8f && this.inputBool2[i] && Event.current.keyCode != KeyCode.Escape)
            {
                this.inputKey2[i] = KeyCode.None;
                this.inputBool2[i] = false;
                this.joystickActive2[i] = true;
                this.joystickString2[i] = "Joystick_8b";
                this.inputString2[i] = "Joystick Axis 8 -";
                this.tempbool = false;
                this.saveInputs();
                this.checDoubleAxis(this.joystickString2[i], i, 2);
            }
        }
    }

    private void inputSetBools()
    {
        for (int i = 0; i < this.DescriptionString.Length; i++)
        {
            if (Input.GetKey(this.inputKey[i]) || (this.joystickActive[i] && Input.GetAxis(this.joystickString[i]) > 0.95f) || Input.GetKey(this.inputKey2[i]) || (this.joystickActive2[i] && Input.GetAxis(this.joystickString2[i]) > 0.95f))
            {
                this.isInput[i] = true;
            }
            else
            {
                this.isInput[i] = false;
            }
            if (Input.GetKeyDown(this.inputKey[i]) || Input.GetKeyDown(this.inputKey2[i]))
            {
                this.isInputDown[i] = true;
            }
            else
            {
                this.isInputDown[i] = false;
            }
            if ((this.joystickActive[i] && Input.GetAxis(this.joystickString[i]) > 0.95f) || (this.joystickActive2[i] && Input.GetAxis(this.joystickString2[i]) > 0.95f))
            {
                if (!this.tempjoy1[i])
                {
                    this.isInputDown[i] = false;
                }
                if (this.tempjoy1[i])
                {
                    this.isInputDown[i] = true;
                    this.tempjoy1[i] = false;
                }
            }
            if (!this.tempjoy1[i] && this.joystickActive[i] && Input.GetAxis(this.joystickString[i]) < 0.1f && this.joystickActive2[i] && Input.GetAxis(this.joystickString2[i]) < 0.1f)
            {
                this.isInputDown[i] = false;
                this.tempjoy1[i] = true;
            }
            if (!this.tempjoy1[i] && !this.joystickActive[i] && this.joystickActive2[i] && Input.GetAxis(this.joystickString2[i]) < 0.1f)
            {
                this.isInputDown[i] = false;
                this.tempjoy1[i] = true;
            }
            if (!this.tempjoy1[i] && !this.joystickActive2[i] && this.joystickActive[i] && Input.GetAxis(this.joystickString[i]) < 0.1f)
            {
                this.isInputDown[i] = false;
                this.tempjoy1[i] = true;
            }
            if (Input.GetKeyUp(this.inputKey[i]) || Input.GetKeyUp(this.inputKey2[i]))
            {
                this.isInputUp[i] = true;
            }
            else
            {
                this.isInputUp[i] = false;
            }
            if ((this.joystickActive[i] && Input.GetAxis(this.joystickString[i]) > 0.95f) || (this.joystickActive2[i] && Input.GetAxis(this.joystickString2[i]) > 0.95f))
            {
                if (this.tempjoy2[i])
                {
                    this.isInputUp[i] = false;
                }
                if (!this.tempjoy2[i])
                {
                    this.isInputUp[i] = false;
                    this.tempjoy2[i] = true;
                }
            }
            if (this.tempjoy2[i] && this.joystickActive[i] && Input.GetAxis(this.joystickString[i]) < 0.1f && this.joystickActive2[i] && Input.GetAxis(this.joystickString2[i]) < 0.1f)
            {
                this.isInputUp[i] = true;
                this.tempjoy2[i] = false;
            }
            if (this.tempjoy2[i] && !this.joystickActive[i] && this.joystickActive2[i] && Input.GetAxis(this.joystickString2[i]) < 0.1f)
            {
                this.isInputUp[i] = true;
                this.tempjoy2[i] = false;
            }
            if (this.tempjoy2[i] && !this.joystickActive2[i] && this.joystickActive[i] && Input.GetAxis(this.joystickString[i]) < 0.1f)
            {
                this.isInputUp[i] = true;
                this.tempjoy2[i] = false;
            }
        }
    }

    private void loadConfig()
    {
        string @string = PlayerPrefs.GetString("KeyCodes");
        string string2 = PlayerPrefs.GetString("Joystick_input");
        string string3 = PlayerPrefs.GetString("Names_input");
        string string4 = PlayerPrefs.GetString("KeyCodes2");
        string string5 = PlayerPrefs.GetString("Joystick_input2");
        string string6 = PlayerPrefs.GetString("Names_input2");
        string[] array = @string.Split(new char[]
        {
            '*'
        });
        this.joystickString = string2.Split(new char[]
        {
            '*'
        });
        this.inputString = string3.Split(new char[]
        {
            '*'
        });
        string[] array2 = string4.Split(new char[]
        {
            '*'
        });
        this.joystickString2 = string5.Split(new char[]
        {
            '*'
        });
        this.inputString2 = string6.Split(new char[]
        {
            '*'
        });
        for (int i = 0; i < this.DescriptionString.Length; i++)
        {
            int num;
            int.TryParse(array[i], out num);
            this.inputKey[i] = (KeyCode)num;
            int num2;
            int.TryParse(array2[i], out num2);
            this.inputKey2[i] = (KeyCode)num2;
            if (this.joystickString[i] == "#")
            {
                this.joystickActive[i] = false;
            }
            else
            {
                this.joystickActive[i] = true;
            }
            if (this.joystickString2[i] == "#")
            {
                this.joystickActive2[i] = false;
            }
            else
            {
                this.joystickActive2[i] = true;
            }
        }
    }

    private void OnGUI()
    {
        if (Time.realtimeSinceStartup > this.lastInterval + 3f)
        {
            this.tempbool = false;
        }
        if (this.menuOn)
        {
            this.drawButtons1();
            if (this.altInputson)
            {
                this.drawButtons2();
            }
        }
    }

    private void reset2defaults()
    {
        if (this.default_inputKeys.Length != this.DescriptionString.Length)
        {
            this.default_inputKeys = new KeyCode[this.DescriptionString.Length];
        }
        if (this.alt_default_inputKeys.Length != this.default_inputKeys.Length)
        {
            this.alt_default_inputKeys = new KeyCode[this.default_inputKeys.Length];
        }
        string text = string.Empty;
        string text2 = string.Empty;
        string text3 = string.Empty;
        string text4 = string.Empty;
        string text5 = string.Empty;
        string text6 = string.Empty;
        for (int i = this.DescriptionString.Length - 1; i > -1; i--)
        {
            text = (int)this.default_inputKeys[i] + "*" + text;
            text2 += "#*";
            text3 = this.default_inputKeys[i].ToString() + "*" + text3;
            PlayerPrefs.SetString("KeyCodes", text);
            PlayerPrefs.SetString("Joystick_input", text2);
            PlayerPrefs.SetString("Names_input", text3);
            text4 = (int)this.alt_default_inputKeys[i] + "*" + text4;
            text5 += "#*";
            text6 = this.alt_default_inputKeys[i].ToString() + "*" + text6;
            PlayerPrefs.SetString("KeyCodes2", text4);
            PlayerPrefs.SetString("Joystick_input2", text5);
            PlayerPrefs.SetString("Names_input2", text6);
            PlayerPrefs.SetInt("KeyLength", this.DescriptionString.Length);
        }
    }

    private void saveInputs()
    {
        string text = string.Empty;
        string text2 = string.Empty;
        string text3 = string.Empty;
        string text4 = string.Empty;
        string text5 = string.Empty;
        string text6 = string.Empty;
        for (int i = this.DescriptionString.Length - 1; i > -1; i--)
        {
            text = (int)this.inputKey[i] + "*" + text;
            text2 = this.joystickString[i] + "*" + text2;
            text3 = this.inputString[i] + "*" + text3;
            text4 = (int)this.inputKey2[i] + "*" + text4;
            text5 = this.joystickString2[i] + "*" + text5;
            text6 = this.inputString2[i] + "*" + text6;
        }
        PlayerPrefs.SetString("KeyCodes", text);
        PlayerPrefs.SetString("Joystick_input", text2);
        PlayerPrefs.SetString("Names_input", text3);
        PlayerPrefs.SetString("KeyCodes2", text4);
        PlayerPrefs.SetString("Joystick_input2", text5);
        PlayerPrefs.SetString("Names_input2", text6);
        PlayerPrefs.SetInt("KeyLength", this.DescriptionString.Length);
    }

    private void Start()
    {
        if (this.alt_default_inputKeys.Length == this.default_inputKeys.Length)
        {
            this.altInputson = true;
        }
        this.inputBool = new bool[this.DescriptionString.Length];
        this.inputString = new string[this.DescriptionString.Length];
        this.inputKey = new KeyCode[this.DescriptionString.Length];
        this.joystickActive = new bool[this.DescriptionString.Length];
        this.joystickString = new string[this.DescriptionString.Length];
        this.inputBool2 = new bool[this.DescriptionString.Length];
        this.inputString2 = new string[this.DescriptionString.Length];
        this.inputKey2 = new KeyCode[this.DescriptionString.Length];
        this.joystickActive2 = new bool[this.DescriptionString.Length];
        this.joystickString2 = new string[this.DescriptionString.Length];
        this.isInput = new bool[this.DescriptionString.Length];
        this.isInputDown = new bool[this.DescriptionString.Length];
        this.isInputUp = new bool[this.DescriptionString.Length];
        this.tempLength = PlayerPrefs.GetInt("KeyLength");
        this.tempjoy1 = new bool[this.DescriptionString.Length];
        this.tempjoy2 = new bool[this.DescriptionString.Length];
        if (!PlayerPrefs.HasKey("KeyCodes") || !PlayerPrefs.HasKey("KeyCodes2"))
        {
            this.reset2defaults();
        }
        this.tempLength = PlayerPrefs.GetInt("KeyLength");
        if (PlayerPrefs.HasKey("KeyCodes") && this.tempLength == this.DescriptionString.Length)
        {
            this.loadConfig();
        }
        else
        {
            PlayerPrefs.DeleteAll();
            this.reset2defaults();
            this.loadConfig();
            this.saveInputs();
        }
        for (int i = 0; i < this.DescriptionString.Length; i++)
        {
            this.isInput[i] = false;
            this.isInputDown[i] = false;
            this.isInputUp[i] = false;
            this.tempjoy1[i] = true;
            this.tempjoy2[i] = false;
        }
    }

    private void Update()
    {
        this.DescriptionBox_X = (float)(Screen.width / 2) + this.DescBox_X;
        this.InputBox1_X = (float)(Screen.width / 2) + this.InputBox_X;
        this.InputBox2_X = (float)(Screen.width / 2) + this.AltInputBox_X;
        this.resetbuttonX = (float)(Screen.width / 2) + this.resetbuttonLocX;
        if (!this.menuOn)
        {
            this.inputSetBools();
        }
        if (Input.GetKeyDown("escape"))
        {
            if (this.menuOn)
            {
                Time.timeScale = 1f;
                this.tempbool = false;
                this.menuOn = false;
                this.saveInputs();
            }
            else
            {
                Time.timeScale = 0f;
                this.menuOn = true;
                Screen.showCursor = true;
                Screen.lockCursor = false;
            }
        }
    }
}