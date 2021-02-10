using System.Collections.Generic;

using UnityEngine;

namespace Anarchy.Custom.Scripts
{
    public class MovingScript : AnarchyCustomScript
    {
        private Transform cachedTransform;
        private Vector3 currentBeginPoint;
        private Quaternion currentBeginRotation;
        private Vector3 currentTargetPoint;
        private Quaternion currentTargetRotation;
        private float lowerBorder;
        private float movementPercentage;
        private float path;
        private float pathMultiplier = 1f;
        private int stage = 1;
        private float upperBorder;
        private float waitTime;

        private float movementMultiplier; //Basically same as your time_multiplier
        private float[] areaMultipliers; //Stores multiplier between stages

        /// <summary>
        /// If object rotates
        /// </summary>
        public bool RotationEnabled { get; set; }

        /// <summary>
        /// List of Points
        /// </summary>
        public List<MovingScriptArea> Areas { get; set; } = new List<MovingScriptArea>();

        private void Awake()
        {
            cachedTransform = GetComponent<Transform>();
        }

        /// <summary>
        /// Initializing
        /// </summary>
        private void Initialize()
        {
            areaMultipliers = new float[Areas.Count];
            for(int i = 0; i < areaMultipliers.Length; i++)
            {
                areaMultipliers[i] = Areas[i].Speed / Vector3.Distance(Areas[i].StartPosition, Areas[i].TargetPosition);
            }
            lowerBorder = 0f;
            upperBorder = 1f;
            stage = 1;
            path = 0f;
            pathMultiplier = 1f;
            movementPercentage = 0f;

            MovingScriptArea area = Areas[0];

            cachedTransform.position = area.StartPosition;
            cachedTransform.rotation = area.StartRotation;
            currentBeginPoint = area.StartPosition;
            currentTargetPoint = area.TargetPosition;
            currentBeginRotation = area.StartRotation;
            currentTargetRotation = area.TargetRotation;
            movementMultiplier = areaMultipliers[0];
            waitTime = area.Delay;
        }

        /// <summary>
        /// Gets position percentage between points
        /// </summary>
        /// <returns></returns>
        private float GetPercentage()
        {
            path += movementMultiplier * Time.deltaTime * pathMultiplier;
            if (path > upperBorder || path < lowerBorder)
            {
                StepToNextStage();
            }
            return path - (stage - 1f);
        }

        /// <summary>
        /// Steps to next "stage"
        /// </summary>
        private void StepToNextStage()
        {
            if (path >= (float)Areas.Count)
            {
                path = (float)Areas.Count - (path - (float)Areas.Count);
                pathMultiplier = -1f;
                return;
            }
            else if (path <= 0f)
            {
                path = Mathf.Abs(path);
                pathMultiplier = 1f;
                return;
            }

            if (Areas.Count > 1)
            {
                upperBorder += pathMultiplier;
                lowerBorder += pathMultiplier;
                stage += (int)pathMultiplier;
            }
            else
            {
                stage = 1;
            }

            MovingScriptArea area = Areas[stage - 1];
            waitTime = area.Delay;

            currentBeginPoint = area.StartPosition;
            currentTargetPoint = area.TargetPosition;
            currentBeginRotation = area.StartRotation;
            currentTargetRotation = area.TargetRotation;
            
            movementMultiplier = areaMultipliers[stage - 1];
        }

        /// <summary>
        /// Launches script
        /// </summary>
        public override void Launch()
        {
            Initialize();
            IsActive = true;
        }

        public override void OnUpdate()
        {
            if(waitTime > 0f)
            {
                waitTime -= Time.deltaTime;
                return;
            }
            movementPercentage = GetPercentage();
            cachedTransform.position = Vector3.Lerp(currentBeginPoint, currentTargetPoint, movementPercentage);
            if (RotationEnabled)
            {
                cachedTransform.rotation = Quaternion.Lerp(currentBeginRotation, currentTargetRotation, movementPercentage);
            }
        }
    }
}