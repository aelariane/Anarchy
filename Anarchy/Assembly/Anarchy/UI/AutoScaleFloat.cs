using System;

namespace Anarchy.UI
{
    /// <summary>
    /// Automatically scalable float. Scales with <seealso cref="UIManager.HUDScaleGUI"/> value
    /// </summary>
    /// <remarks>
    /// Created for simplify adapting UI for different screens
    /// </remarks>
    public struct AutoScaleFloat
    {
        /// <summary>
        /// Scaled value
        /// </summary>
        public float Value { get; }
        /// <summary>
        /// Value before scale was done
        /// </summary>
        public float RawValue { get; }

        public AutoScaleFloat(float value)
        {
            Value = value;
            RawValue = value;
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