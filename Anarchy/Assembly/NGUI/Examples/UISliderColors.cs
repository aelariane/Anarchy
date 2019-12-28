using UnityEngine;

[RequireComponent(typeof(UISlider))]
[AddComponentMenu("NGUI/Examples/Slider Colors")]
[ExecuteInEditMode]
public class UISliderColors : MonoBehaviour
{
    private UISlider mSlider;

    public Color[] colors = new Color[]
        {
        Color.red,
        Color.yellow,
        Color.green
    };

    public UISprite sprite;

    private void Start()
    {
        this.mSlider = base.GetComponent<UISlider>();
        this.Update();
    }

    private void Update()
    {
        if (this.sprite == null || this.colors.Length == 0)
        {
            return;
        }
        float num = this.mSlider.sliderValue;
        num *= (float)(this.colors.Length - 1);
        int num2 = Mathf.FloorToInt(num);
        Color color = this.colors[0];
        if (num2 >= 0)
        {
            if (num2 + 1 < this.colors.Length)
            {
                float t = num - (float)num2;
                color = Color.Lerp(this.colors[num2], this.colors[num2 + 1], t);
            }
            else if (num2 < this.colors.Length)
            {
                color = this.colors[num2];
            }
            else
            {
                color = this.colors[this.colors.Length - 1];
            }
        }
        color.a = this.sprite.color.a;
        this.sprite.color = color;
    }
}