
namespace Anarchy.Custom.Scripts
{
    /// <summary>
    /// Serves for indicating Trap object
    /// </summary>
    public class TrapComponent : UnityEngine.MonoBehaviour
    {
        public TrapType Type { get; set; }
        public float GasUsageMultiplier { get; set; }
        public float KillTime { get; set; }

        private void Awake()
        {
            enabled = false;
        }
    }
}
