using System;

namespace Anarchy.UI
{
    public struct AutoScaleFloat
    {
        public float Value { get; set; }

        public AutoScaleFloat(float value)
        {
            Value = value;
            if (UIManager.HUDAutoScaleGUI.Value)
            {
                Value = (float)Math.Round(value * UIManager.HUDScaleGUI.Value, 2);
            }
        }

        public override string ToString()
        {
            return Value.ToString("F2");
        }

        public static implicit operator float(AutoScaleFloat val)
        {
            return val.Value;
        }

        public static implicit operator AutoScaleFloat(float val)
        {
            return new AutoScaleFloat(val);
        }
    }
}