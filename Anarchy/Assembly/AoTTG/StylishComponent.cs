using Optimization.Caching;
using System.Collections.Generic;
using UnityEngine;

public class StylishComponent : MonoBehaviour
{
    private UISprite barU;
    private GameObject baseG;
    private Transform baseGT;
    private Transform baseT;
    private int chainKillRank;
    private float[] chainRankMultiplier;
    private float chainTime;
    private float duration;
    private Vector3 exitPosition;
    private bool flip;
    private bool hasLostRank;
    private Dictionary<string, UILabel> Labels;
    private Vector3 originalPosition;
    private float R;
    private int styleHits;
    private float stylePoints;
    private int styleRank;
    private int[] styleRankDepletions;
    private int[] styleRankPoints;
    private string[,] styleRankText;
    private int styleTotalDamage;
    public GameObject bar;
    public new bool enabled;
    public GameObject labelChain;
    public GameObject labelHits;
    public GameObject labelS;
    public GameObject labelS1;
    public GameObject labelS2;
    public GameObject labelsub;
    public GameObject labelTotal;

    public StylishComponent()
    {
        string[,] array3 = new string[8, 2];
        array3[0, 0] = "D";
        array3[0, 1] = "eja Vu";
        array3[1, 0] = "C";
        array3[1, 1] = "asual";
        array3[2, 0] = "B";
        array3[2, 1] = "oppin!";
        array3[3, 0] = "A";
        array3[3, 1] = "mazing!";
        array3[4, 0] = "S";
        array3[4, 1] = "ensational!";
        array3[5, 0] = "S";
        array3[5, 1] = "pectacular!!";
        array3[6, 0] = "S";
        array3[6, 1] = "tylish!!!";
        array3[7, 0] = "X";
        array3[7, 1] = "TREEME!!!";
        string[,] array2 = array3;
        this.styleRankText = array2;
        this.chainRankMultiplier = new float[]
        {
            1f,
            1.1f,
            1.2f,
            1.3f,
            1.5f,
            1.7f,
            2f,
            2.3f,
            2.5f
        };
        this.styleRankPoints = new int[]
        {
            350,
            950,
            2450,
            4550,
            7000,
            15000,
            100000
        };
        this.styleRankDepletions = new int[]
        {
            1,
            2,
            5,
            10,
            15,
            20,
            25,
            25
        };
    }

    private void Awake()
    {
        baseG = gameObject;
        baseGT = baseG.transform;
        baseT = transform;
        FengGameManagerMKII.Stylish = this;
        this.Labels = new Dictionary<string, UILabel>();
        this.Labels["LabelS"] = this.labelS.GetComponent<UILabel>();
        this.Labels["LabelS1"] = this.labelS1.GetComponent<UILabel>();
        this.Labels["LabelS2"] = this.labelS2.GetComponent<UILabel>();
        this.Labels["LabelSub"] = this.labelsub.GetComponent<UILabel>();
        this.Labels["LabelHits"] = this.labelHits.GetComponent<UILabel>();
        this.Labels["LabelTotal"] = this.labelTotal.GetComponent<UILabel>();
        this.Labels["LabelChain"] = this.labelChain.GetComponent<UILabel>();
        this.barU = this.bar.GetComponent<UISprite>();
    }

    private int GetRankPercentage()
    {
        int result;
        if (this.styleRank > 0 && this.styleRank < this.styleRankPoints.Length)
        {
            result = (int)((this.stylePoints - (float)this.styleRankPoints[this.styleRank - 1]) * 100f / (float)(this.styleRankPoints[this.styleRank] - this.styleRankPoints[this.styleRank - 1]));
        }
        else if (this.styleRank == 0)
        {
            result = (int)(this.stylePoints * 100f) / this.styleRankPoints[this.styleRank];
        }
        else
        {
            result = 100;
        }
        return result;
    }

    private int GetStyleDepletionRate()
    {
        return this.styleRankDepletions[this.styleRank];
    }

    private void setPosition()
    {
        this.originalPosition = new Vector3((float)((int)((float)Screen.width * 0.5f - 2f)), (float)((int)((float)Screen.height * 0.5f - 150f)), 0f);
        this.exitPosition = new Vector3((float)Screen.width, this.originalPosition.y, this.originalPosition.z);
    }

    private void SetRank()
    {
        int num = this.styleRank;
        int num2 = 0;
        while (num2 < this.styleRankPoints.Length && this.stylePoints > (float)this.styleRankPoints[num2])
        {
            num2++;
        }
        if (num2 < this.styleRankPoints.Length)
        {
            this.styleRank = num2;
        }
        else
        {
            this.styleRank = this.styleRankPoints.Length;
        }
        if (this.styleRank >= num)
        {
            if (this.styleRank > num)
            {
                this.hasLostRank = false;
            }
            return;
        }
        if (this.hasLostRank)
        {
            this.stylePoints = 0f;
            this.styleHits = 0;
            this.styleTotalDamage = 0;
            this.styleRank = 0;
            return;
        }
        this.hasLostRank = true;
    }

    private void setRankText()
    {
        this.Labels["LabelS"].text = this.styleRankText[this.styleRank, 0];
        this.Labels["LabelS2"].text = string.Empty;
        this.Labels["LabelS1"].text = string.Empty;
        switch (this.styleRank)
        {
            case 0:
                this.Labels["LabelS1"].text = string.Empty;
                this.Labels["LabelS"].text = "[" + ColorSet.color_D + "]D";
                break;

            case 1:
                this.Labels["LabelS1"].text = string.Empty;
                this.Labels["LabelS"].text = "[" + ColorSet.color_C + "]C";
                break;

            case 2:
                this.Labels["LabelS1"].text = string.Empty;
                this.Labels["LabelS"].text = "[" + ColorSet.color_B + "]B";
                break;

            case 3:
                this.Labels["LabelS1"].text = string.Empty;
                this.Labels["LabelS"].text = "[" + ColorSet.color_A + "]A";
                break;

            case 4:
                this.Labels["LabelS1"].text = string.Empty;
                this.Labels["LabelS"].text = "[" + ColorSet.color_S + "]S";
                break;

            case 5:
                this.Labels["LabelS2"].text = "[" + ColorSet.color_SS + "]S";
                this.Labels["LabelS"].text = "[" + ColorSet.color_SS + "]S";
                break;

            case 6:
                this.Labels["LabelS1"].text = "[" + ColorSet.color_SSS + "]S";
                this.Labels["LabelS2"].text = "[" + ColorSet.color_SSS + "]S";
                this.Labels["LabelS"].text = "[" + ColorSet.color_SSS + "]S";
                break;

            case 7:
                this.Labels["LabelS1"].text = string.Empty;
                this.Labels["LabelS"].text = "[" + ColorSet.color_X + "]X";
                break;
        }
        this.Labels["LabelSub"].text = this.styleRankText[this.styleRank, 1];
    }

    private void shakeUpdate(float dt)
    {
        if (this.duration > 0f)
        {
            this.duration -= dt;
            if (this.flip)
            {
                baseGT.localPosition = this.originalPosition + Vectors.up * this.R;
            }
            else
            {
                baseGT.localPosition = this.originalPosition - Vectors.up * this.R;
            }
            this.flip = !this.flip;
            if (this.duration <= 0f)
            {
                baseGT.localPosition = this.originalPosition;
            }
        }
    }

    private void Start()
    {
        this.setPosition();
        baseT.localPosition = this.exitPosition;
    }

    private void Update()
    {
        if (!IN_GAME_MAIN_CAMERA.isPausing)
        {
            var dt = Time.deltaTime;
            if (this.stylePoints > 0f)
            {
                this.setRankText();
                this.barU.fillAmount = (float)this.GetRankPercentage() * 0.01f;
                this.stylePoints -= (float)this.GetStyleDepletionRate() * dt * 10f;
                this.SetRank();
            }
            if (this.stylePoints <= 0f)
            {
                baseT.localPosition = Vector3.Lerp(baseT.localPosition, this.exitPosition, dt * 3f);
            }
            if (this.chainTime > 0f)
            {
                this.chainTime -= dt;
            }
            else
            {
                this.chainTime = 0f;
                this.chainKillRank = 0;
            }
            this.shakeUpdate(dt);
        }
    }

    public void reset()
    {
        this.styleTotalDamage = 0;
        this.chainKillRank = 0;
        this.chainTime = 0f;
        this.styleRank = 0;
        this.stylePoints = 0f;
        this.styleHits = 0;
    }

    public void startShake(int R, float duration)
    {
        if (this.duration < duration)
        {
            this.R = (float)R;
            this.duration = duration;
        }
    }

    public void Style(int damage)
    {
        if (damage != -1)
        {
            this.stylePoints += (float)((int)((float)(damage + 200) * this.chainRankMultiplier[this.chainKillRank]));
            this.styleTotalDamage += damage;
            this.chainKillRank = ((this.chainKillRank >= this.chainRankMultiplier.Length - 1) ? this.chainKillRank : (this.chainKillRank + 1));
            this.chainTime = 5f;
            this.styleHits++;
            this.SetRank();
        }
        else if (this.stylePoints == 0f)
        {
            this.stylePoints += 1f;
            this.SetRank();
        }
        this.startShake(5, 0.3f);
        this.setPosition();
        this.Labels["LabelTotal"].text = ((int)this.stylePoints).ToString();
        this.Labels["LabelHits"].text = this.styleHits.ToString() + ((this.styleHits <= 1) ? "Hit" : "Hits");
        if (this.chainKillRank == 0)
        {
            this.Labels["LabelChain"].text = string.Empty;
            return;
        }
        this.Labels["LabelChain"].text = "x" + this.chainRankMultiplier[this.chainKillRank].ToString() + "!";
    }
}