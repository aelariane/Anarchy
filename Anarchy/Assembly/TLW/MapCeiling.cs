using System;
using UnityEngine;
using Anarchy.Configuration;

namespace TLW
{
    /// <summary>
    /// Class contains reference to Camera.main and creates and holds references to instances of barrier and killcuboid objects
    /// whose transform properties of position, rotation, and localscale are set to this MonoBehaviours transform properties on
    /// creation.
    /// If reference to _barrierRef is lost, any subsequent calls to update the transparency will
    /// no longer do anything.
    /// Client should pass these references when they are made available.
    /// </summary>
    public class MapCeiling : MonoBehaviour
    {
        private GameObject _barrierRef;
        private Color _color;
        private float _minAlpha = 0f;
        private float _maxAlpha = 0.6f;
        private float _minimumHeight = 3;

        public static BoolSetting UseFade = new BoolSetting("CeilingFade", true);

        /// <summary>
        /// Instantiates a barrier and kill cuboid prefab and sets the transform
        /// properties of position, rotation, and localscale equal to this monobehaviours
        /// transform properties.
        /// </summary>
        private void Start()
        {
            GameObject bombCeiling = (GameObject)UnityEngine.Object.Instantiate(Optimization.Caching.CacheResources.RCLoad("barrier"), new Vector3(0f, 280f, 0f), Quaternion.identity);
            bombCeiling.transform.position = this.transform.position;
            bombCeiling.transform.rotation = this.transform.rotation;
            bombCeiling.transform.localScale = this.transform.localScale;

            _barrierRef = (GameObject)UnityEngine.Object.Instantiate(Optimization.Caching.CacheResources.RCLoad("killcuboid"), new Vector3(0f, 280f, 0f), Quaternion.identity); //name of object and xyz position, its 280 height
            _barrierRef.transform.position = this.transform.position;
            _barrierRef.transform.rotation = this.transform.rotation;
            _barrierRef.transform.localScale = this.transform.localScale;
            _color = new Color(1, 0, 0, _maxAlpha);

            // check if we want to enable barrier fade.
            if (UseFade)
            {
                UpdateTransparency();
            }
            else
            {
                // disable monobehavior "behaviors" (update will not be called).
                enabled = false;
            }
        }

        private void Update()
        {
            UpdateTransparency();
        }

        /// <summary>
        /// Gets and sets minimum alpha
        /// </summary>
        public float MinimumAlpha
        {
            get => _minAlpha;
            set
            {
                if (value > 1f || value < 0f)
                {
                    throw new Exception("MinimumAlpha must in range (0 <= value <= 1)");
                }
                _minAlpha = value;
            }
        }

        public float MaximumAlpha
        {
            get => _maxAlpha;
            set
            {
                if (value > 1f || value < 0f)
                {
                    throw new Exception("MaximumAlpha must in range (0 <= value <= 1)");
                }
                _maxAlpha = value;
            }
        }

        /// <summary>
        /// Changes the transparency of the barrier
        /// depending on how close the player is to it.
        /// </summary>
        public void UpdateTransparency()
        {
            if (IN_GAME_MAIN_CAMERA.BaseCamera != null && _barrierRef != null)
            {
                if (_barrierRef.renderer != null)
                {
                    float newAlpha = _maxAlpha;
                    try
                    {
                        float startHeight = _barrierRef.transform.position.y / _minimumHeight;
                        // convert player position between floor and ceiling to a value x, (0 <= x <= 1)
                        // given that they are above a specific height.
                        if (IN_GAME_MAIN_CAMERA.MainT.position.y < startHeight)
                        {
                            newAlpha = _minAlpha;
                        }
                        else
                        {
                            newAlpha = Map(IN_GAME_MAIN_CAMERA.MainT.position.y, startHeight, _barrierRef.transform.position.y, _minAlpha, _maxAlpha);
                        }

                        // use mapped player position with a function that adds exponential growth but is still bounded, (0 <= x' <= 1)
                        newAlpha = FadeByGradient(newAlpha);
                    }
                    catch
                    {

                    }
                    _color.a = newAlpha;
                    _barrierRef.renderer.material.color = _color;
                }
            }

        }

        /// <summary>
        /// just using a quadratic function with
        /// a scaled gradient now and it looks fine
        /// </summary>
        /// <param name="x">floating point input</param>
        /// <returns>The quadratic of gradient * x * x clamped between _minAlpha and _maxAlpha</returns>
        public float FadeByGradient(float x)
        {
            float gradient = 10f;
            float result = gradient * x * x;
            return Mathf.Clamp(result, _minAlpha, _maxAlpha);
        }

        /// <summary>
        /// Linearly interpolates a target value with a known range into a a new range.
        /// Ex: (0 <= x <= 100) -> (0 <= x' <= 255)
        ///     x' will be proportionally equal to x
        ///     
        /// If x is not inside the input range (inMin <= x <= inMax),
        /// an Exception will be thrown.
        /// </summary>
        /// <param name="x">input value to be interpolated</param>
        /// <param name="inMin">input minimum value</param>
        /// <param name="inMax">input maximum value</param>
        /// <param name="outMin">output minimum value</param>
        /// <param name="outMax">output maximum value</param>
        /// <returns>interpolated floating point value of x</returns>
        public float Map(float x, float inMin, float inMax, float outMin, float outMax)
        {
            if (x > inMax || x < inMin)
            {
                throw new Exception("Error,\npublic float map(float x, float inMin, float inMax, float outMin, float outMax)\nis not defined for values (x > inMax || x < inMin)");
            }
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

    }
}