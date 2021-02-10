using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Anarchy.Custom.Scripts;
using UnityEngine;

namespace Anarchy.Custom.Level
{
    /// <summary>
    /// Special logic for Custom-Anarchy levels
    /// </summary>
    public class CustomAnarchyLevel : Photon.MonoBehaviour
    {
        public static CustomAnarchyLevel Instance { get; private set; }

        public List<AnarchyCustomScript> Scripts { get; } = new List<AnarchyCustomScript>();

        private void Awake()
        {
            Instance = this;
        }
        
        private void OnLevelWasLoaded(int level)
        {
            Scripts.Clear();
        }
        
        private void Update()
        {
            int count = Scripts.Count;
            for (int i = 0; i < count; i++)
            {
                AnarchyCustomScript script = Scripts[i];
                if (script.IsActive)
                {
                    script.OnUpdate();
                }
            }
        }
    }
}
