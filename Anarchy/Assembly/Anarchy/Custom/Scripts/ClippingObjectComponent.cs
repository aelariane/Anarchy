namespace Anarchy.Custom.Scripts
{

    /// <summary>
    /// Just an indicator for hooks, if they need to parent this object
    /// </summary>
     public sealed class ClippingObjectComponent : UnityEngine.MonoBehaviour
     {
        private void Awake()
        {
            enabled = false;
        }
     }
}
