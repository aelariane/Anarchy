using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Anarchy.Custom.Scripts
{
    public class RotationScript : AnarchyCustomScript
    {
        private float percentage;
        private float time;
        private float speed;

        public Quaternion BaseRotation { get; set; }
        public Quaternion TargetRotation { get; set; }

        /// <summary>
        /// Elapsed time to reach target rotation
        /// </summary>
        public float Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
                speed = 1f / time;
            }
        }

        private Transform cachedTransform;

        private void Awake()
        {
            cachedTransform = GetComponent<Transform>();
        }

        //By sadico
        public float for_lerp(float value)
        {
            if (value > 1)
                return 2 - value;
            return value;
        }

        //clip the value between 0 and 2 so that it's a go and back animation
        public float get_clipped(float x)
        {
            if (x > 2)
                return get_clipped(x - 2);
            return x;
        }

        public override void Launch()
        {
            base.Launch();
            percentage = 0f;
        }

        public override void OnUpdate()
        {
            percentage = get_clipped(percentage + (UnityEngine.Time.deltaTime * speed));
            cachedTransform.rotation = Quaternion.Lerp(BaseRotation, TargetRotation, for_lerp(percentage));
        }
    }
}
