using System;
using UnityEngine;

namespace Anarchy.Custom.Scripts
{
    /// <summary>
    /// Base class for specific Anarchy scripts for custom maps
    /// </summary>
    public abstract class AnarchyCustomScript : MonoBehaviour
    {
        /// <summary>
        /// Identifier of script. Can be used for some interactions, like parenting object to script
        /// </summary>
        public int? ScriptID { get; set; } = null;
        /// <summary>
        /// If script is active
        /// </summary>
        public bool IsActive { get; protected set; } = false;

        /// <summary>
        /// Launches script
        /// </summary>
        public virtual void Launch()
        {
            IsActive = true;
        }

        /// <summary>
        /// Calls every frame
        /// </summary>
        public virtual void OnUpdate()
        {
        }
    }
}
